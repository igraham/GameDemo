using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour
{
	
	public float currentHeight = 0f;
	public float hoverHeight = 2.5f;
	public float hoverForceMultiplier = 0f;
	public float hoverForce = 30f;
	public float speed = 7f;
	PathFinder pathFinder;
	public Vector3 hoverForceApplied = Vector3.zero;
	public Vector3 currentDestination = Vector3.zero;
	public Vector3 nextDestination = Vector3.zero;
	public Transform finalDestination; //sounds like a movie name
	GameObject[] targetList;
	public List<Vector3> path = null;
	
	void Start ()
	{
		pathFinder = GetComponent<PathFinder> ();
		targetList = GameObject.FindGameObjectsWithTag ("HasDrones");
		finalDestination = targetList [Random.Range (0, targetList.Length)].transform;
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
	
	//use this to change the transform being used as the final destination
	//when it is changed the model with this script attached will begin to follow the new target.
	public void setTarget (Transform target)
	{
		finalDestination = target;
	}
	
	void FixedUpdate ()
	{
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
		//If the final destination is in line of sight...
		if(PathFinder.lineOfSight(transform.position, finalDestination.position))
		{
			//set current destination equal to the final destination's position
			if(currentDestination != finalDestination.position)
				currentDestination = finalDestination.position;
			//remove remaining waypoints from the path (no longer needed)
			if(path.Count > 0)
				path.RemoveAll(_unused => true);
			//then set next destination to null (we won't have another next with this list)
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
