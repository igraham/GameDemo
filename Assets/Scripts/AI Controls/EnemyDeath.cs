using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour
{
	//Goes onto destructable object, also object needs tag Destructable
	
	public int durability = 25;
	
	public void damageDurability(int damage)
	{
		durability = durability-damage;
		if(durability <= 0)
		{
			Network.Destroy(gameObject);
		}
	}
}
