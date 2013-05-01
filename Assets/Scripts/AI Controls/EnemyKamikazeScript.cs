using UnityEngine;
using System.Collections;

public class EnemyKamikazeScript : MonoBehaviour {

	public Detonator explosion;
	
	public int damage = 25;
	//Goes onto legion enemy drones
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag.Equals("Player"))
		{
			collision.gameObject.networkView.RPC ("damagePlayer", RPCMode.AllBuffered, damage);
			Network.Instantiate(explosion, transform.position, Quaternion.identity,0);
			Network.Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag.Equals("HasDrones"))
		{
			other.gameObject.networkView.RPC ("damageNode",RPCMode.AllBuffered, damage);
			Network.Instantiate(explosion, transform.position, Quaternion.identity,0);
			Network.Destroy(gameObject);
		}
	}
}
