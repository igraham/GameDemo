using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {

	bool forward = false;
	bool reverse = false;
	bool rotRight = false;
	bool rotLeft = false;
	bool strRight = false;
	bool strLeft = false;
	public float speed = 5f;
	public float maxSpeed = 15f;
	public Vector3 rotationSpeed = new Vector3 (0, 50f, 0);
	public GameObject turret;
	public Camera playerCamera;
	
	[RPC]
	public void setClientMovementControls (bool f, bool r, bool rotR, bool rotL, bool strR, bool strL)
	{
		forward = f;
		reverse = r;
		rotRight = rotR;
		rotLeft = rotL;
		strRight = strR;
		strLeft = strL;
	}
	
	private void fixUnintentionalRotation()
	{
		if(transform.localEulerAngles.z > 10f)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
													 transform.localEulerAngles.y, 0);
		}
	}
	
	private void moveForward()
	{
		if(Quaternion.Angle (rigidbody.rotation, turret.transform.rotation) < 5f)
		{
			rigidbody.AddForce(transform.forward.normalized * speed);
			if(rigidbody.velocity.magnitude > maxSpeed)
			{
				rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
			}
		}
		rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, 
											  		  turret.transform.rotation, 
											  		  Time.deltaTime * 4f);
		turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, 
													 		 rigidbody.rotation, 
													 		 Time.deltaTime * 2f);
		playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, 
														   		   turret.transform.rotation, 
														   		   Time.deltaTime * 3f);
	}
	
	private void moveBack()
	{
		rigidbody.AddForce(-1f * transform.forward.normalized * speed);
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}
	
	private void rotateLeft()
	{
		Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime * -1f);
		rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
	}
	
	private void rotateRight()
	{
		Quaternion deltaRotation = Quaternion.Euler(rotationSpeed * Time.deltaTime);
		rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
	}
	
	private void strafeLeft()
	{
		rigidbody.AddForce(-1f * transform.right.normalized * speed);
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}
	
	private void strafeRight()
	{
		rigidbody.AddForce(transform.right.normalized * speed);
		if(rigidbody.velocity.magnitude > maxSpeed)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
	}
	
	public void Controls()
	{
		if(forward && reverse)
		{
			//do nothing
		}
		else if(forward)
		{
			moveForward();
		}
		else if(reverse)
		{
			moveBack();
		}
		if(rotRight && rotLeft)
		{
			//do nothing
		}
		else if(rotRight)
		{
			rotateRight();
		}
		else if(rotLeft)
		{
			rotateLeft();
		}
		if(strLeft && strRight)
		{
			//do nothing
		}
		else if(strRight)
		{
			strafeRight();
		}
		else if(strLeft)
		{
			strafeLeft();
		}
		fixUnintentionalRotation();
	}
	
}
