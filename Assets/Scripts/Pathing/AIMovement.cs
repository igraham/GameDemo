using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour
{
	
	public float speed = 7f;
	PathFinder pathFinder;
	public Vector3 hoverForceApplied = Vector3.zero;
	public Vector3 currentDestination = Vector3.zero;
	public Vector3 nextDestination = Vector3.zero;
	public Transform finalDestination; //sounds like a movie name
	List<GameObject> targetList = new List<GameObject>();
	public List<Vector3> path = new List<Vector3>();
	int targetIndex = -1;
	bool stopCode = false;
	public Vector3 finalDestPosition;
	
	void Start()
	{
		pathFinder = GetComponent<PathFinder>();
		if(gameObject.name.Equals("Enemy(Clone)"))
		{
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("HasDrones"))
			{
				targetList.Add(obj);
			}
			foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
			{
				targetList.Add(obj);
			}
			//if no targets are found, blow up using detonator insanity.
			if(targetList.Count == 0)
			{
				stopCode = true;
				gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
				return;
			}
			else
			{
				setRandomTargetFromList();
				path = pathFinder.getPath(transform.position+new Vector3(0, 2.5f, -1f), 
					finalDestination.position);
				//set initial current and next destination
				if(path.Count > 1)
				{
					currentDestination = path[0];
					nextDestination = path[1];
				}
				else
				{
					stopCode = true;
					gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
				}
			}
		}
		else if(gameObject.name.Equals("Drone(Clone)"))
		{
			//set the target manually
			
			//may be an issue, we get the target by being next to it on the tank.
			//however, finding said target is difficult, and this code is executed in Start.
			//additionally, resources need a "spawn point" next to it to avoid spawning
			//drones inside the terrain.
		}
	}
	
	//used when finding a new random target (i.e, target was destroyed before enemy could reach it,
	//player disconnected, connection lost, etc.)
	public void setRandomTargetFromList()
	{
		if(targetList.Count > 1)
		{
			targetIndex = Random.Range(0,targetList.Count);
			finalDestination = targetList[targetIndex].transform;
		}
		else if(targetList.Count == 1)
		{
			targetIndex = 0;
			finalDestination = targetList[targetIndex].transform;
		}
		else
		{
			stopCode = true;
			gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
		}
	}
	
	//use this to change the transform being used as the final destination
	//when it is changed the model with this script attached will begin to follow the new target.
	public void setTarget(Transform target)
	{
		finalDestination = target;
	}
	
	void FixedUpdate ()
	{
		if(!stopCode)
		{
			//before anything else, determine whether there are any targets to follow.
			if(targetList.Count == 0 
				|| !(pathFinder.EnemiesSeeWayPoints()) 
				|| !(pathFinder.WayPointsSeeTarget()))
			{
				stopCode = true;
				gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
				return;
			}
			//check to see whether the final destination is no longer a valid target
			if(targetIndex > -1 && ((!finalDestination && targetList.Count > 0)
				|| (finalDestination.name == "Resource" && finalDestination.tag != "HasDrones")))
			{
				//if it is not, then generate a new target.
				//for enemies, remove the old target from the targetList and generate a new random target
				//removing a destroyed object works because the list only has a reference
				targetList.RemoveAt(targetIndex);
				setRandomTargetFromList();
				if(stopCode){return;}
			}
			//check if path is null, if path is empty (and can't see the destination currently), 
			//or if the last waypoint in path can't see the destination
			if((path.Count <= 0 
					&& !(PathFinder.lineOfSight(transform.position+new Vector3(0, 2.5f, -1f), 
				finalDestination.position))))
			{
				path = pathFinder.getPath (transform.position, finalDestination.position);
			}
			//check whether the final destination is visible.
			//determine whether we need a new current and next destination
			if(nextDestination != Vector3.zero && 
				PathFinder.lineOfSight(transform.position+new Vector3(0, 2.5f, -1f), nextDestination))
			{
				if(path.Count > 0)
				{
					//determine what the currentDestination and nextDestination are.
					//remove the current destination from the path
					path.RemoveAt(0);
					//and then set the current destination to the next destination.
					currentDestination = nextDestination;
					//check to see if there is a valid next destination.
					if(path.Count > 1)
					{
						nextDestination = path[1];
					}
					else if(path.Count == 1)
					{
						//if there are no more waypoints set the next destination to the final destination
						nextDestination = finalDestination.position;
					}
				}
			}
			if(!stopCode)
			{
				//If the final destination is in line of sight...
				if(PathFinder.lineOfSight(transform.position+new Vector3(0, 2.5f, -1f), 
					finalDestination.position))
				{
					//set current destination equal to the final destination's position
					if(currentDestination != finalDestination.position)
						currentDestination = finalDestination.position;
					//remove remaining waypoints from the path (no longer needed)
					if(path.Count > 0)
						path.RemoveAll(_unused => true);
					//then set next destination to Vector3.zero (we won't have another next with this list)
					if(nextDestination != Vector3.zero)
						nextDestination = Vector3.zero;
				}
				//point towards the current destination
				transform.LookAt(new Vector3(currentDestination.x, 1.5f, currentDestination.z));
				//use transform.forward.normalized to get the direction
				//then Time.deltaTime and speed in order to determine how fast the object will be moving.
				//Additionally, use a y-coordinate value of 1.5 for the height the tank hovers at.
				rigidbody.MovePosition(new Vector3(transform.position.x, 1.5f, transform.position.z) 
					+ transform.forward.normalized * Time.deltaTime * speed);
			}
		}
	}
}