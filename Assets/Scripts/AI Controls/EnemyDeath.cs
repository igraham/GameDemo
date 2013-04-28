using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour {
	/*
	 * Goes onto destructable object, also object needs tag Destructable
	 */
	public int durability = 25;
	public GameObject tank;
	ObjectiveController obj;
	// Use this for initialization
	void Start () {
		tank = GameObject.Find("Tank");
		obj = tank.GetComponent<ObjectiveController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		
	
	}
	
	public void damageDurability(int damage)
	{
		durability = durability-damage;
		if(durability <= 0)
		{
			Destroy(gameObject);
		    obj.KillEnemy();
		}
	}
}
