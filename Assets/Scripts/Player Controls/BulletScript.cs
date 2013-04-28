using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	
	public Detonator explosion;
	Vector3 gunLocation;
	public int damage =25;

	void Start ()
	{
		GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
		GameObject temp;
		float dist = float.MaxValue;
		foreach(GameObject obj in tanks)
		{
			if(Vector3.Distance(transform.position, obj.transform.position) < dist)
			{
				temp = obj;
				dist = Vector3.Distance(transform.position, obj.transform.position);
				gunLocation = temp.transform.FindChild("Main Camera").FindChild("Turret").FindChild ("GunBarrel").position;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(Network.isServer)
		{
			Vector3 backtrack = Vector3.zero;
			if(collision.gameObject.tag.Equals ("Terrain"))
			{
				backtrack = Vector3.MoveTowards(transform.position,gunLocation,3f);
				Network.Instantiate(explosion, backtrack, Quaternion.identity, 0);
				Network.Destroy(gameObject);
				return;
			}
			else if(collision.gameObject.tag.Equals ("Enemy"))
			{
				EnemyDeath dura2 = (EnemyDeath) collision.gameObject.GetComponent(typeof(EnemyDeath));
				dura2.damageDurability(damage);
			}
			else if(collision.gameObject.tag.Equals ("Player"))
			{
				collision.gameObject.networkView.RPC ("damagePlayer", RPCMode.AllBuffered, damage);
			}
			Network.Instantiate(explosion, transform.position, Quaternion.identity, 0);
			Network.Destroy(gameObject);
		}
	}
}