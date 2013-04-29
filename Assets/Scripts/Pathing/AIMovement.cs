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
	public Transform finalDestination;
	GameObject[] targetList;
	public List<Vector3> path = null;
	
	void Start ()
	{
		pathFinder = GetComponent<PathFinder> ();
		targetList = GameObject.FindGameObjectsWithTag ("HasDrones");
		finalDestination = targetList [Random.Range (0, targetList.Length)].transform;
		path = pathFinder.getPath(transform.position, finalDestination.position);
	}
	
	public void setTarget (Transform target)
	{
		finalDestination = target;
	}
	
	void FixedUpdate ()
	{
		//check if path is null, if path is empty, 
		//or if the last waypoint in path can't see the destination
		if(path == null || path.Count <= 0 
			|| !(PathFinder.lineOfSight(path.FindLast(_unused => true), finalDestination.position)))
		{
			path = pathFinder.getPath (transform.position, finalDestination.position);
		}
		
		//other code to go here
		
		if(Vector3.Distance (currentDestination, transform.position) > 4)
		{
			//rigidbody.isKinematic = false;
			transform.LookAt(currentDestination);
			rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
		}
	}
}
