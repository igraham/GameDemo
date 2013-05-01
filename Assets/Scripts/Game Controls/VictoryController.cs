using UnityEngine;
using System.Collections;

public class VictoryController : MonoBehaviour {
   public float power = 2f;
   public GameObject rocket;
   public GameObject mainCamera;
   public GUIText victory;
   
   float x;
   float y;
   float z;
	// Use this for initialization
	void Start () {
	x = transform.position.x;
	y = transform.position.y;
	z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
	
	transform.position= new Vector3(x,y+Time.time*power,z);
	power+=0.005f;
	mainCamera.transform.LookAt(transform.position);
	if(Time.time > 8)
		{
		 victory.text = "Victory!";
		}
	if(Input.GetKeyDown(KeyCode.Return))
{
  Application.LoadLevel("Splashscreen");
}
	if(Time.time>15)
		{
		Application.LoadLevel("Splashscreen");
		}
	}
}
