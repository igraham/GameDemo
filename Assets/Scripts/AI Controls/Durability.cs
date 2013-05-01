using UnityEngine;
using System.Collections;

public class Durability : MonoBehaviour {
	
	//Goes onto destructable object, also object needs tag Destructable
	//Currently not in use, but can be used to model further destructable objects.
	//Networked methods would need to be implemented (this is as simple as adding
	//Network to Destroy and [RPC] to damageDurability.
	
	public int durability = 100;
	
	// Update is called once per frame
	void Update ()
	{
		if(durability <=0)
			Destroy(gameObject);
	}
	
	public void damageDurability(int damage)
	{
		durability = durability-damage;
	}
}
