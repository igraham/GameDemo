using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {
	
	//how close to waypoints (including goal) do we need to get?
	public float threshhold = 1f;
	
	class WayPoint
	{
		public string name;
		public Vector3 position;
		public List<WayPoint> neighbors;
		
		public WayPoint(string n, Vector3 p)
		{
			name = n;
			position = p;
			neighbors = new List<WayPoint>();
		}
	}
	
	static GameObject[] wayPointGOs = null;//Game Objects
	static List<WayPoint> wayPoints;
	
	Hashtable cameFrom = null;
	WayPoint goal = null;
	WayPoint start = null;
	List<WayPoint> path;
	
	//public bool debugLines = true;
	
	void Start ()
	{
		//only do this once, no matter how many AStarPathFinder instances there are.
		if(wayPointGOs==null)
		{
			wayPointGOs = GameObject.FindGameObjectsWithTag("WayPoint");
			wayPoints = new List<WayPoint>(wayPointGOs.Length);
			//build WayPoints Array
			foreach (GameObject wpgo in wayPointGOs)
			{
				WayPoint wp = new WayPoint(wpgo.name, wpgo.transform.position);
				wayPoints.Add(wp);
			}
			//Calculate neighbors.
			for(int i = 0; i<wayPoints.Count; i++)
			{
				for(int j=i+1; j<wayPoints.Count; j++)
				{
					if(lineOfSight(wayPoints[i].position, wayPoints[j].position))
					{
						wayPoints[i].neighbors.Add(wayPoints[j]);
						wayPoints[j].neighbors.Add(wayPoints[i]);
					}
				}
			}
		}
	}
	
	bool aStarSearch()
	{
		List<WayPoint> closedSet = new List<WayPoint>();
		List<WayPoint> openSet = new List<WayPoint>();
		openSet.Add(start);
		cameFrom = new Hashtable();
		Hashtable g_score = new Hashtable();
		g_score[start] = 0f;
		Hashtable f_score = new Hashtable();
		f_score[start] =  (float) g_score[start] + distance(start, goal);
		WayPoint current;
		float temp_score;
		while(openSet.Count>0)
		{
			current = findMinScore(f_score, openSet);
			if(current == goal)
				return true;
			
			openSet.Remove(current);
			closedSet.Add(current);
			foreach(WayPoint neighbor in current.neighbors)
			{
				if(closedSet.Contains(neighbor))
					continue;
				temp_score = (float) g_score[current] + distance(current,neighbor);
				bool openSetDoesNotHaveNeighbor = !openSet.Contains(neighbor);
				if(openSetDoesNotHaveNeighbor || temp_score < (float) g_score[neighbor])
				{
					cameFrom[neighbor] = current;
					g_score[neighbor] = temp_score;
					f_score[neighbor] = (float) g_score[neighbor] + distance(neighbor, goal);
					if(openSetDoesNotHaveNeighbor)
						openSet.Add(neighbor);
				}
			}
		}
		return false;
	}
	
	void reconstructPath(WayPoint goal)
	{
		if (cameFrom.ContainsKey(goal))
		{
			reconstructPath((WayPoint) cameFrom[goal]);
		}
		path.Add(goal);
	}
	
	WayPoint findMinScore(Hashtable f_score, List<WayPoint> openSet)
	{
		WayPoint minWP = openSet[0];
		float minScore = (float) f_score[minWP];
		float score;
		foreach(WayPoint wp in openSet)
		{
			score = (float) f_score[wp];
			if(score < minScore)
			{
				minWP = wp;
			}
		}
		return minWP;
	}
	
	float distance(WayPoint origin, WayPoint destination)
	{
		return Vector3.Distance(origin.position, destination.position);
	}
	
	public static bool lineOfSight(Vector3 origin, Vector3 destination)
	{
		bool r = Physics.Linecast(origin, destination);
		return !r;
	}
	
	void resetPath()
	{
		foreach(WayPoint wp in start.neighbors)
			wp.neighbors.Remove(start);
		foreach(WayPoint wp in goal.neighbors)
			wp.neighbors.Remove(goal);
		wayPoints.Remove(start);
		wayPoints.Remove(goal);
		start=null;
		goal=null;
		path=null;
	}
	
	void buildStartAndGoal(Vector3 origin, Vector3 destination)
	{
		//print ("Start Location: "+origin);
		//print ("Destination: "+destination);
		if(goal!=null)
			resetPath();
		//create start and end waypoints
		start = new WayPoint("Start", origin);
		goal = new WayPoint("Goal", destination);
		//neighbors.
		foreach (WayPoint wp in wayPoints)
		{
			if(lineOfSight(wp.position, start.position))
			{
				wp.neighbors.Add(start);
				start.neighbors.Add(wp);
			}
			if(lineOfSight(wp.position, goal.position))
			{
				wp.neighbors.Add(goal);
				goal.neighbors.Add(wp);
			}
		}
		checkWayPointVision();
		wayPoints.Add(start);
		wayPoints.Add(goal);

		path = new List<WayPoint>();
		if(aStarSearch()==false)
			throw new System.Exception("No WayPoint path can be determined");
		reconstructPath(goal);
		// foreach(WayPoint wp in path) print(wp.name);	
	}
	
	void checkWayPointVision()
	{
		if(start.neighbors.Count==0)
			throw new System.Exception("No WayPoint can see Origin: " + start.position);
		if(goal.neighbors.Count==0)
			throw new System.Exception("No WayPoint can see Destination: " + goal.position);
	}
	
	public bool EnemiesSeeWayPoints()
	{
		return !(start.neighbors.Count==0);
	}
	
	public bool WayPointsSeeTarget()
	{
		return !(goal.neighbors.Count==0);
	}
	
	public List<Vector3> getPath(Vector3 origin, Vector3 destination)
	{
		List<Vector3> pathPoints = new List<Vector3>();
		if(lineOfSight(origin, destination))
		{
			pathPoints.Add(destination);
			return pathPoints;
		}
		buildStartAndGoal(origin, destination);
		path.Remove(start);
		path.Remove(goal);
		foreach(WayPoint p in path)
		{
			pathPoints.Add(p.position);
		}
		return pathPoints;
	}
}
