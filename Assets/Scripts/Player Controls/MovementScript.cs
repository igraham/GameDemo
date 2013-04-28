using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {
	public float speed = 60f;
	public Vector3 rotationSpeed = new Vector3(0,100f,0);
  
	// Update is called once per frame	
	void Update () 
	{
		//move foreward
		if(Input.GetButton("Forward"))
			rigidbody.AddForce(transform.forward.normalized * speed);
		//move backwards
		if(Input.GetButton("Reverse"))
			rigidbody.AddForce(-1f*transform.forward.normalized * speed);
		//rotate right
		if(Input.GetButton("Right")) 
		{
			Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime);
        	rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		}
		//rotate left
		if(Input.GetButton("Left"))
		{
        	Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime * -1f);
        	rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		}
		//strafe right
		if(Input.GetButton("StrRight")) 
			rigidbody.AddForce(transform.right.normalized * speed);
		//strafe left
		if(Input.GetButton("StrLeft")) 
			rigidbody.AddForce(-1f*transform.right.normalized * speed);
	}
}