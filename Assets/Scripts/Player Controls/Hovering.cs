using UnityEngine;
using System.Collections;

public class Hovering : MonoBehaviour
{
	
	public float currentHeight = 0;
	public float hoverHeight = 2.5f;
	public float hoverForceMultiplier = 0;
	public float hoverForce = 10f;
	public Vector3 hoverForceApplied = Vector3.zero;
	
	void FixedUpdate ()
	{
		RaycastHit rayHit;
		if (Physics.Raycast (transform.position+transform.forward.normalized, Vector3.down, out rayHit))
		{
			currentHeight = rayHit.distance;
			hoverForceApplied = (Vector3.up * Physics.gravity.magnitude);
			if(currentHeight < hoverHeight-1f)
			{
				rigidbody.AddForce(new Vector3(0f, -rigidbody.velocity.y, 0f));
			}
			if(currentHeight - Time.deltaTime < hoverHeight)
			{
				hoverForceMultiplier = (hoverHeight - currentHeight) / hoverHeight;
             	hoverForceApplied += (Vector3.up * hoverForce * hoverForceMultiplier);
			}
			else if(currentHeight > hoverHeight + (hoverHeight*0.5f))
			{
				hoverForceApplied = Vector3.zero;
			}
			else if((currentHeight - hoverHeight - Time.deltaTime) < (hoverHeight / 2))
			{
				hoverForceApplied *= ((hoverHeight - (currentHeight - hoverHeight)) / hoverHeight);
			}
			rigidbody.AddForce(hoverForceApplied);
		}
	}
}