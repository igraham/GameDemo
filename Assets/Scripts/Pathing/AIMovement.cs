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
	public List<Vector3> path = null;
	int targetIndex = -1;
	
	void Awake()
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
				gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
			}
			else
			{
				setRandomTargetFromList();
				path = pathFinder.getPath(transform.position, finalDestination.position);
				//set initial current and next destination
				if(path.Count > 0)
				{ //path has to have at least one element to get here.
					currentDestination = path[0];
					if(path.Count > 1)
					{ //path has to have at least two elements to get here.
						nextDestination = path[1];
					}
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
		targetIndex = Random.Range(0,targetList.Count);
		finalDestination = targetList[targetIndex].transform;
	}
	
	//use this to change the transform being used as the final destination
	//when it is changed the model with this script attached will begin to follow the new target.
	public void setTarget(Transform target)
	{
		finalDestination = target;
	}
	
	void FixedUpdate ()
	{
		//before anything else, determine whether there are any targets to follow.
		if(targetList.Count == 0)
		{
			gameObject.networkView.RPC("noTargetsDetonation", RPCMode.AllBuffered);
		}
		//check to see whether the final destination is no longer a valid target
		if(!finalDestination && targetList.Count > 0)
		{
			//if it is not, then generate a new target.
			//for enemies, remove the old target from the targetList and generate a new random target
			//removing a destroyed object works because the list only has a reference
			targetList.RemoveAt(targetIndex);
			setRandomTargetFromList();
		}
		//check if path is null, if path is empty (and can't see the destination currently), 
		//or if the last waypoint in path can't see the destination
		if(path == null
			|| (path.Count <= 0 
				&& !(PathFinder.lineOfSight(transform.position, finalDestination.position)))
			|| (path.Count > 0 
				&& !(PathFinder.lineOfSight(path.FindLast(_unused => true), finalDestination.position))))
		{
			path = pathFinder.getPath (transform.position, finalDestination.position);
		}
		//check whether the final destination is visible.
		//determine whether we need a new current and next destination
		if(nextDestination != Vector3.zero && PathFinder.lineOfSight(transform.position, nextDestination))
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
		//If the final destination is in line of sight...
		if(PathFinder.lineOfSight(transform.position, finalDestination.position))
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
		transform.LookAt(currentDestination);
		//use transform.forward.normalized to get the direction
		//then Time.deltaTime and speed in order to determine how fast the object will be moving.
		rigidbody.MovePosition(transform.position + transform.forward.normalized * Time.deltaTime * speed);
	}
}