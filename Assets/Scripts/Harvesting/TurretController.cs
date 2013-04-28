using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{
	private float timer = 0;
	public float shotTimeLimit = 3f;
	Rigidbody bullet;
	float angle = 15f;
	float dist = 0;
	public Rigidbody bulletPrefab;
	public bool turretActive = false;
	
	void turretShooter ()
	{
		//Debug.Log ("Test");
		
		Collider closest = null;
		Collider[] damagable = Physics.OverlapSphere (transform.position, 20f);
		foreach (Collider hit in damagable) {
			
			
			if (hit.gameObject.tag.Equals ("Enemy")) {
				if(closest == null)
				{
					closest = hit;
					
				}
					
					if(closest != null && Vector3.Distance(closest.gameObject.transform.position,gameObject.transform.position) > 
						Vector3.Distance(hit.gameObject.transform.position,gameObject.transform.position))
						closest = hit;
				
			}
		}
		
		if(closest != null)
		{
			dist = Vector3.Distance(closest.gameObject.transform.position,gameObject.transform.position);
			Vector3 pos = closest.gameObject.transform.position;
				 transform.LookAt (new Vector3 (pos.x, 
     			Mathf.Sin (angle) * dist / Mathf.Sin (90 * Mathf.Deg2Rad - angle), pos.z));
			
				shootAt (closest.gameObject);
		}
	}
 
	void shootAt (GameObject obj)
	{
		bullet = Instantiate (bulletPrefab, transform.position + Vector3.up, Quaternion.identity) as Rigidbody;
		BulletScript bulletScript = (BulletScript)bullet.GetComponent (typeof(BulletScript));
		bulletScript.damage = 10;
		bullet.AddForce (BallisticVel (obj.transform), ForceMode.Impulse);
	}
 
	Vector3 BallisticVel (Transform target)
	{
		Vector3 dir = target.position - transform.position;  // get target direction
		float h = dir.y;  // get height difference
		dir.y = 0;  // retain only the horizontal direction
		float dist = dir.magnitude;  // get horizontal distance
		dir.y = dist * Mathf.Tan (angle);  // set dir to the elevation angle
		dist += h / Mathf.Tan (angle);  // correct for small height differences
		// calculate the velocity magnitude
		float vel = Mathf.Sqrt (dist * Physics.gravity.magnitude / Mathf.Sin (2 * angle));
		return vel * dir.normalized;
	}

	
	// Update is called once per frame
	void Update ()
	{
		timer += Time.deltaTime;
		if(timer >= shotTimeLimit && turretActive)
		{
			turretShooter ();
			timer = 0;
		}
	}
}
