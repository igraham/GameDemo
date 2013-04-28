using UnityEngine;
using System.Collections;

public class DefeatTimer : MonoBehaviour {
	
	private float timer;
	
	//Change Scene loaded to the splash scene
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer > 5f)
			Application.LoadLevel("Splashscene");
	}
}
