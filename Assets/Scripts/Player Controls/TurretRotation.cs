using UnityEngine;
using System.Collections;

public class TurretRotation : MonoBehaviour {
	
	public GameObject gunBarrel;
	public Camera mainCamera;
	
	public float rotationSpeed = 60F;
	
	void Start()
	{
		//Screen.lockCursor = true;
	}
	
	void Update ()
	{	
		float upDown = -Input.GetAxis("Mouse Y") *Time.deltaTime * rotationSpeed;
		float leftRight = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
		
		//making it turn up or down
		if(gunBarrel.transform.localEulerAngles.x < 2 )
		{
    		gunBarrel.transform.Rotate(upDown, 0, 0);
		}
		else if (gunBarrel.transform.localEulerAngles.x > 335)
		{
			gunBarrel.transform.Rotate(upDown, 0, 0);         
		}
		
		//making it turn left or right
		if(transform.localEulerAngles.y < 33 )
		{
 		   transform.Rotate(0, leftRight, 0);
		} 
		else if (transform.localEulerAngles.y > 327)
		{
		    transform.Rotate(0, leftRight, 0);        
		}
 
		//making it not exeed rotation limit (left and right) and preventing it lock up
		if(transform.localEulerAngles.y >= 33 && transform.localEulerAngles.y < 327)
		{
   			if(transform.localEulerAngles.y < 180)
			{
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 32.9F, 0);
				mainCamera.transform.Rotate(0, leftRight, 0);
    		}
			else
			{
       			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 327.1F, 0);
				mainCamera.transform.Rotate(0, leftRight, 0);
    		}
		}
 
		//making it not exeed rotation limit (up and down) and preventing it lock up
		if(gunBarrel.transform.localEulerAngles.x >= 2 && gunBarrel.transform.localEulerAngles.x < 335){
   			if(gunBarrel.transform.localEulerAngles.x < 180)
			{
    		   gunBarrel.transform.localEulerAngles = new Vector3(1.9F, gunBarrel.transform.localEulerAngles.y, 0);
   			}
			else
			{
      			gunBarrel.transform.localEulerAngles = new Vector3(335.1F, gunBarrel.transform.localEulerAngles.y, 0);
			}
		}
 
		//making sure it doesn't turn on it's z axis
		if(transform.localEulerAngles.z != 0)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
				transform.localEulerAngles.y, 0);
		}
	}
}
