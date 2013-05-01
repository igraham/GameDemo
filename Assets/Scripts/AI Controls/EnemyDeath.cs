using UnityEngine;
using System.Collections;

public class EnemyDeath : MonoBehaviour
{
	//Goes onto destructable object, also object needs tag Destructable
	
	public int durability = 25;
	
	public Detonator normal;
	public Detonator insanity;
	
	[RPC]
	void noTargetsDetonation()
	{
		Network.Instantiate(insanity, transform.position, Quaternion.identity,0);
		Network.Destroy(gameObject);
	}
	
	[RPC]
	void damageEnemy(int damage)
	{
		durability = durability-damage;
		if(durability <= 0)
		{
			Network.Instantiate(normal, transform.position, Quaternion.identity,0);
			Network.Destroy(gameObject);
		}
	}
}
