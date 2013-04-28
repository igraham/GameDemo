using UnityEngine;
using System.Collections;

public class EnemyKamikazeScript : MonoBehaviour {

	public Detonator explosion;
	
	/*
	 * Goes onto legion enemy drones
	 */
	public int damage =25;
	// Use this for initialization
	
	void explosionDamage()
	{
		Collider[] damagable = Physics.OverlapSphere (transform.position, 10f);
		foreach (Collider hit in damagable)
		{
			if(hit.gameObject.tag.Equals ("HasDrones"))
			{
				
				int percent = (int)(damage*(transform.position-hit.transform.position).magnitude/10f);
				
				if(hit.gameObject.name.Equals("Tank"))
				{
					PlayerGameState player = (PlayerGameState) hit.gameObject.GetComponent(typeof(PlayerGameState));
					player.damagePlayer(percent);
				}
				if(hit.gameObject.name.Equals("Resource"))
				{
					ResourceNodeScript node = (ResourceNodeScript) hit.gameObject.GetComponent(typeof(ResourceNodeScript));
					node.damageNode(percent);
				}
				if(hit.gameObject.name.Equals("Untitled (4)"))
				{
					ResourceNodeScript node = (ResourceNodeScript) hit.gameObject.GetComponent(typeof(ResourceNodeScript));
					node.damageNode(percent);
				}
				
			}
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		//Debug.Log("TEST " + collision.gameObject.name);
		if(collision.gameObject.tag.Equals ("HasDrones"))
		{
			
			if(collision.gameObject.name.Equals("Tank"))
			{
				PlayerGameState player = (PlayerGameState) collision.gameObject.GetComponent(typeof(PlayerGameState));
				player.damagePlayer(damage);
			}
			
			if(collision.gameObject.name.Equals("Untitled (4)"))
			{
				ResourceNodeScript node = (ResourceNodeScript) collision.gameObject.GetComponent(typeof(ResourceNodeScript));
				node.damageNode(damage);
			}
			Instantiate(explosion, transform.position, Quaternion.identity) ;
			explosionDamage();
		    Destroy (gameObject);
			
		}
		

	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.name.Equals("Resource"))
			{
				ResourceNodeScript node = (ResourceNodeScript) other.gameObject.GetComponent(typeof(ResourceNodeScript));
				node.damageNode(damage);
				Instantiate(explosion, transform.position, Quaternion.identity) ;
				explosionDamage();
		    	Destroy (gameObject);
			}
			
		}
}
