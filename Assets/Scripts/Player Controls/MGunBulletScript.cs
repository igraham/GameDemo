using UnityEngine;
using System.Collections;

public class MGunBulletScript : MonoBehaviour
{
	void OnCollisionEnter(Collision col)
	{
		Network.Destroy(gameObject);
	}
}
