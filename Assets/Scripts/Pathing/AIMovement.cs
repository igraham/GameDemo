using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour
{
	
	public float currentHeight = 0f;
	public float hoverHeight = 1.5f;
	public float hoverForceMultiplier = 0f;
	public float hoverForce = 15f;
	public float speed = 7f;
	PathFinder pathFinder;
	public Vector3 hoverForceApplied = Vector3.zero;
	public Vector3 pathPoint = Vector3.zero;
	public Vector3 destination = Vector3.zero;
	public GameObject target;
	GameObject[] targetList;
	
	void Start ()
	{
		pathFinder = GetComponent<PathFinder> ();
		targetList = GameObject.FindGameObjectsWithTag ("HasDrones");
		target = targetList [Random.Range (0, targetList.Length)];
		destination = target.transform.position + Vector3.up * 1.5f;
	}
	
	public void setTarget (Transform target)
	{
		this.target = target.gameObject;
	}
	
	void FixedUpdate ()
	{
		destination = target.transform.position + Vector3.up * 1.5f;
		pathPoint = pathFinder.getPath (transform.position, destination);
		if (Vector3.Distance (destination, transform.position) > 4) {
			rigidbody.isKinematic = false;
			transform.LookAt (pathPoint);
			rigidbody.MovePosition (transform.position + transform.forward * Time.deltaTime * speed);
  
			RaycastHit rayHit;
			if (Physics.Raycast (transform.position + transform.forward.normalized, Vector3.down, out rayHit)) {
				currentHeight = rayHit.distance;
				hoverForceApplied = (Vector3.up * Physics.gravity.magnitude);
				if (rigidbody.velocity.magnitude < 1f) {
					transform.position += Vector3.up + transform.forward;
				}
				if (currentHeight < hoverHeight - 1f) {
					rigidbody.AddForce (new Vector3 (0f, -rigidbody.velocity.y, 0f));
				}
				if (currentHeight - Time.deltaTime < hoverHeight) {
					hoverForceMultiplier = (hoverHeight - currentHeight) / hoverHeight;
					hoverForceApplied += (Vector3.up * hoverForce * hoverForceMultiplier);
				} else if (currentHeight > hoverHeight + (hoverHeight * 0.5f)) {
					hoverForceApplied = Vector3.zero;
				} else if ((currentHeight - hoverHeight - Time.deltaTime) < (hoverHeight / 2)) {
					hoverForceApplied *= ((hoverHeight - (currentHeight - hoverHeight)) / hoverHeight);
				}
				rigidbody.AddForce (hoverForceApplied);
			}
		} else {
			if (!(rigidbody.isKinematic)) {
				transform.LookAt (new Vector3 (pathPoint.x, transform.position.y, pathPoint.z));
				rigidbody.velocity = Vector3.zero;
				rigidbody.isKinematic = true;
			}
		}
	}
	
}
