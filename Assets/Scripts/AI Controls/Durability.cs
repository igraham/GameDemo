using UnityEngine;
using System.Collections;

public class Durability : MonoBehaviour {
	/*
	 * Goes onto destructable object, also object needs tag Destructable
	 */
	public int durability = 100;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(durability <=0)
			Destroy(gameObject);
	
	}
	
	public void damageDurability(int damage)
	{
		durability = durability-damage;
	}
}
