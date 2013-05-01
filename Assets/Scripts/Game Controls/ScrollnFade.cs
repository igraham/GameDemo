using UnityEngine;
using System.Collections;

public class ScrollnFade : MonoBehaviour {
	public float speed = .2f;
	public bool crawling = true;
	// Use this for initialization
	void Start () {
	GUIText tc = gameObject.GetComponent<GUIText>();
		tc.fontSize = Screen.height/18;
	string creds = "Year: 2500 Rising tensions between nations result in the \n";
	creds += "development of the L.E.G.I.O.N weapon system.\n";
	creds += "Designed to be a devasting force to anything set\n";
	creds +="in its path, this massive swarm of attack drones soon\n";
	creds += "became aware of its power and turned rogue.\n";
	creds +="Nearing extinction, the remains of the human race sought\n";
	creds +="refuge in space stations. Their only hope lies with you\n";
    creds +="and your AV-590 Salvager, the most advance defense vehicle\n";
	creds +="known to mankind. Your job is to take back the earth from\n";
	creds +="the L.E.G.I.O.N. swarm and save the human \n";
	creds  +="race from their imminent demise.";
	tc.text = creds;
	}
	
	// Update is called once per frame
	void Update () {
	if(!crawling)
			return;
		transform.Translate(Vector3.up * Time.deltaTime*speed);
		/*if(gameObject.transform.position.y>.8)
		{
		crawling = false;	
		}*/
		if(Input.GetKeyDown(KeyCode.Return))
		{
		Application.LoadLevel("Splashscreen");	
		
		}
		if(Time.time > 50f)
		{
		  Application.LoadLevel("Splashscreen");
		}
	}
}
