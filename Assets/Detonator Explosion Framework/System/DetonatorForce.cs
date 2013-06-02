using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Force")]
public class DetonatorForce : DetonatorComponent {

	private float _baseRadius = 50.0f;
	private float _basePower = 4000.0f;
	private float _scaledRange;
	private float _scaledIntensity;
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	public float radius;
	public float power;
	public GameObject fireObject;
	public float fireObjectLife;
	
	public Collider[] _colliders;
	private GameObject _tempFireObject;
	
	public int bombDamage = 25;
	public int bulletDamage = 5;
	
	override public void Init()
	{
		//unused
	}

	void Update()
	{
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private Vector3 _explosionPosition;
	
	override public void Explode()
	{
		if (!on) return;
		if (detailThreshold > detail) return;	
		
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
		}
		if (_explodeDelay <= 0) //if the delayTime is zero
		{
			//tweak the position such that the explosion center is related to the explosion's direction
			_explosionPosition = transform.position; //- Vector3.Normalize(MyDetonator().direction);
			_colliders = Physics.OverlapSphere (_explosionPosition, radius);
			
			foreach (Collider hit in _colliders) 
			{
				if (!hit)
				{
					continue;
				}
				if(hit.rigidbody)
				{
					RaycastHit hitInfo;
					if (Physics.Linecast(transform.position, hit.transform.position, out hitInfo))
					{
						if(hitInfo.transform.tag == "Terrain")
						{
							continue;
						}
					}
					
					float dist = Vector3.Distance (hit.transform.position, transform.position);
					
					if(hit.rigidbody.tag == "Wall")
					{
						if(hit.gameObject && dist <= 10f)
						{
							//print ("wall hit!");
							int percent = (int)(25*(transform.position-hit.transform.position).magnitude/10f);
							hit.gameObject.networkView.RPC ("damageWall", RPCMode.AllBuffered, percent);
						}
						continue;
					}
					if(hit.rigidbody.tag == "Enemy")
					{
						if(dist <= 10f)
						{
							int percent = (int)(bombDamage*(transform.position-hit.transform.position).magnitude/10f);
							hit.gameObject.networkView.RPC ("damageEnemy", RPCMode.AllBuffered, percent);
						}
					}
					if(hit.rigidbody.tag == "Player")
					{
						if(dist <= 10f)
						{
							int percent = (int)(bombDamage*(transform.position-hit.transform.position).magnitude/10f);
							hit.gameObject.networkView.RPC ("damagePlayer", RPCMode.AllBuffered, percent);
						}
					}
					if(hit.rigidbody.tag == "HasDrones")
					{
						if(dist <= 10f)
						{
							int percent = (int)(bombDamage*(transform.position-hit.transform.position).magnitude/10f);
							hit.gameObject.networkView.RPC ("damageNode", RPCMode.AllBuffered, percent);
						}
					}
					//align the force along the object's rotation
					//this is wrong - need to attenuate the velocity according to distance from the explosion center			
					//offsetting the explosion force position by the negative of the explosion's direction may help
					hit.rigidbody.AddExplosionForce((power * size), _explosionPosition, (radius * size), (4f * MyDetonator().upwardsBias * size));
					SendMessage("OnDetonatorForceHit", null, SendMessageOptions.DontRequireReceiver);
					
					//and light them on fire for Rune
					if (fireObject)
					{
						//check to see if the object already is on fire. being on fire twice is silly
						if (hit.transform.Find(fireObject.name+"(Clone)"))
						{
							return;
						}
						_tempFireObject = (Instantiate(fireObject, this.transform.position, this.transform.rotation)) as GameObject;
						_tempFireObject.transform.parent = hit.transform;
						_tempFireObject.transform.localPosition = new Vector3(0f,0f,0f);
						if (_tempFireObject.particleEmitter)
						{
							_tempFireObject.particleEmitter.emit = true;
							Destroy(_tempFireObject,fireObjectLife);
						}
					}
				}
			}
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;
		}
		else
		{
			//tell update to start reducing the start delay and call explode again when it's zero
			_delayedExplosionStarted = true;
		}
	}
	
	public void Reset()
	{
		radius = _baseRadius;
		power = _basePower;
	}
}