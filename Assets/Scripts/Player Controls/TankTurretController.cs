using UnityEngine;
using System.Collections;

public class TankTurretController : MonoBehaviour
{
	float mouseH = 0;
	float mouseV = 0;
	public float rotSpeed = 60f;
	public GameObject turret;
	public GameObject gunBarrel;
	public Camera playerCamera;
	public float upDown = 0f;
	public float leftRight = 0f;
	bool vertChanged = false;
	bool horizChanged = false;
	
	[RPC]
	public void setClientTurretControls (float mouseX, float mouseY)
	{
		if(mouseH != mouseX){horizChanged = true;}
		mouseH = mouseX;
		if(mouseV != mouseY){vertChanged = true;}
		mouseV = mouseY;
		upDown = -mouseV * Time.deltaTime * rotSpeed;
		leftRight = mouseH * Time.deltaTime * rotSpeed;
	}
	
	private void rotateVertically()
	{
		if(gunBarrel.transform.localEulerAngles.x < 2)
		{
			gunBarrel.transform.Rotate(upDown, 0, 0);
		}
		else if(gunBarrel.transform.localEulerAngles.x > 335)
		{
			gunBarrel.transform.Rotate(upDown, 0, 0);         
		}
	}
	
	private void rotateHorizontally()
	{
		if(turret.transform.localEulerAngles.y < 33)
		{
			turret.transform.Rotate(0, leftRight, 0);
		}
		else if(turret.transform.localEulerAngles.y > 327)
		{
			turret.transform.Rotate(0, leftRight, 0);        
		}
	}
	
	private void fixHorizontalRotation()
	{
		if(turret.transform.localEulerAngles.y >= 33 && turret.transform.localEulerAngles.y < 327)
		{
			if(turret.transform.localEulerAngles.y < 180)
			{
				turret.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 32.9F, 0);
				playerCamera.transform.Rotate(0, leftRight, 0);
			}
			else
			{
				turret.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 327.1F, 0);
				playerCamera.transform.Rotate(0, leftRight, 0);
			}
		}
	}
	
	private void fixVerticalRotation()
	{
		if(gunBarrel.transform.localEulerAngles.x >= 2 && gunBarrel.transform.localEulerAngles.x < 335)
		{
			if(gunBarrel.transform.localEulerAngles.x < 180)
			{
				gunBarrel.transform.localEulerAngles = new Vector3(1.9F, gunBarrel.transform.localEulerAngles.y, 0);
			}
			else
			{
				gunBarrel.transform.localEulerAngles = new Vector3(335.1F, gunBarrel.transform.localEulerAngles.y, 0);
			}
		}
	}
	
	private void fixUnintentionalRotation()
	{
		if(turret.transform.localEulerAngles.z != 0)
		{
			turret.transform.localEulerAngles = new Vector3(turret.transform.localEulerAngles.x, 
				turret.transform.localEulerAngles.y, 0);
		}
	}
	
	public void Controls()
	{
		if(vertChanged)
		{
			rotateVertically();
			fixVerticalRotation();
			vertChanged = false;
		}
		if(horizChanged)
		{
	 		rotateHorizontally();
			fixHorizontalRotation();
			horizChanged = false;
		}
		if(vertChanged || horizChanged)
		{
			fixUnintentionalRotation();
			vertChanged = false;
			horizChanged = false;
		}
	}
}
