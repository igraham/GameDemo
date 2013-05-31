using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour {
	
	public int wallHealth = 100;
	public GameObject prefab;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(wallHealth <= 0)
		{
			Network.Destroy(gameObject);
			Network.Instantiate(prefab, transform.position, Quaternion.identity, 0);
		}
	}
	
	[RPC]
	public void damageWall(int damage)
	{
		wallHealth = wallHealth - damage;
	}
}
