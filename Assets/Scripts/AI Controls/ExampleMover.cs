using UnityEngine;
using System.Collections;

public class ExampleMover : MonoBehaviour {
	
	PathFinder pathFinder;
	public Vector3 destination; //sample target destination.; 
	public float speed = 7f;
	
	public Vector3 pathPoint = Vector3.zero;
	public Vector3 origin = Vector3.zero;
	
	public GameObject target;
	
	// Use this for initialization
	void Start () {
		pathFinder = GetComponent<PathFinder>();
		destination = target.transform.position+Vector3.up*2;
		Debug.Log (destination);
	}
	
	// Update is called once per frame
	void Update () {
		pathPoint = pathFinder.getPath(transform.position, destination);
		origin = transform.position;
		if(Vector3.Distance(pathPoint, transform.position)>4)
		{
			transform.LookAt(pathPoint);
			rigidbody.MovePosition(transform.position + transform.forward*Time.deltaTime*speed);
		}
		else
		{
			transform.LookAt(transform.position + Vector3.forward);
		}
	}
}
