using System.Collections;
using UnityEngine;

public class Fading : MonoBehaviour
{
	public float startFadeIn = 0f;
	public float endFadeIn = 1.0f;
	public float fadeInTime = 0.5f;
	public float fadeOutDelay = 3f;
	public float startFadeOut = 1.0f;
	public float endFadeOut = 0f;
	public float fadeOutTime = 2f;
	public float radarCooldownTime = 60f;
	
	private bool radarCooldown = false;
	
	private void RadarCooldown()
	{
		radarCooldown = false;
	}
	
	void Update()
	{
		if(!radarCooldown && Input.GetKey(KeyCode.Space))
		{
			FadeRadar();
			radarCooldown = true;
			Invoke("RadarCooldown",radarCooldownTime);
			networkView.RPC("showRadarDotToEnemies", RPCMode.Server);
		}
	}
	
	public void FadeRadar()
	{
		StartCoroutine (ControlledFading(1f, 0.0f, 0.5f, 0.0f, 1f, 2f, 0f));
	}
	
	private IEnumerator ControlledFading(float startFadeIn, float endFadeIn, float fadeInTime,
										 float startFadeOut, float endFadeOut, float fadeOutTime,
										 float fadeOutDelay)
	{
		yield return StartCoroutine(Fade(startFadeIn, endFadeIn, fadeInTime));
		yield return new WaitForSeconds(fadeOutDelay);
		yield return StartCoroutine(Fade(startFadeOut, endFadeOut, fadeOutTime));
	}

	private IEnumerator StartFading()
	{
		yield return StartCoroutine(Fade(startFadeIn, endFadeIn, fadeInTime));
		yield return new WaitForSeconds(fadeOutDelay);
		yield return StartCoroutine(Fade(startFadeOut, endFadeOut, fadeOutTime));
	}

	private IEnumerator Fade (float startLevel, float endLevel, float time)
	{ 
		float speed = 1.0f / time;
		
		for (float t = 0.0f; t < 1.0; t += Time.deltaTime*speed)
		{ 
			float a = Mathf.Lerp (startLevel, endLevel, t);
			renderer.material.color = new Color (renderer.material.color.r, 
									   	  		 renderer.material.color.g, 
									   	  		 renderer.material.color.b, a);
			yield return 0;
		}
	}
}