using UnityEngine;
using System.Collections;

public class AIPath : MonoBehaviour {
	
	
	PathFinder pather;
	Animator anim;
	
	//points of interest.
	Vector3[] points = {new Vector3(4,1,1), 
		new Vector3(1.3f,1,15.5f), 
		new Vector3(-14f,1,12f), 
		new Vector3(-9f,1,3f)};
	Vector3 currentDestination;
	
	float timer;
	bool justArrived = false;

	// Use this for initialization
	void Start () {
		currentDestination = points[Random.Range(0, points.Length)];
		anim = GetComponent<Animator>();
		pather = GetComponent<PathFinder>();
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 tempPathPoint = pather.getPath(transform.position+Vector3.up, currentDestination);
		Vector3 direction = tempPathPoint - transform.position;
		float dis = direction.magnitude;
		if(dis > 1){
			transform.forward = new Vector3(direction.x, transform.forward.y, direction.z);
			anim.SetBool("Go", true);
		}
		else{
			anim.SetBool("Go", false);
			if(!justArrived){
				justArrived=true;
				timer = Time.time;
			}
			//we are at destination.  Wait until timer expires.
			if((Time.time - timer)>3f){
				currentDestination = points[Random.Range(0, points.Length)];
				timer = Time.time;
				justArrived=false;
			}
		}
	}
}
