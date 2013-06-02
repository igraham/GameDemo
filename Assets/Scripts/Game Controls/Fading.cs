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

	public bool typeRenderer;
	
	private bool radarCooldown = false;
	
	private void RadarCooldown()
	{
		radarCooldown = false;
	}

	void Awake()
	{
		if(gameObject.guiText)
		{
			typeRenderer = false;
		}
		else if(gameObject.renderer)
		{
			typeRenderer = true;
		}
	}
	
	void Update()
	{
		if(typeRenderer)
		{
			if(!radarCooldown && Input.GetKey(KeyCode.Space))
			{
				FadeRadar();
				radarCooldown = true;
				Invoke("RadarCooldown",radarCooldownTime);
				networkView.RPC("showRadarDotToEnemies", RPCMode.Server);
			}
		}
		else
		{
			StartCoroutine(ControlledFadingGUIText(0f, 1f, 0.15f, 1f, 0f, 0.75f, 0f));
		}
	}
	
	public void FadeRadar()
	{
		StartCoroutine (ControlledFadingMat(1f, 0f, 0.5f, 0f, 1f, 2f, 0f));
	}

	private IEnumerator ControlledFadingGUIText(float startFadeIn, float endFadeIn, float fadeInTime,
										 float startFadeOut, float endFadeOut, float fadeOutTime,
										 float fadeOutDelay)
	{
		yield return StartCoroutine(FadeGUIText(startFadeIn, endFadeIn, fadeInTime));
		yield return new WaitForSeconds(fadeOutDelay);
		yield return StartCoroutine(FadeGUIText(startFadeOut, endFadeOut, fadeOutTime));
		Destroy(gameObject);
	}

	private IEnumerator ControlledFadingMat(float startFadeIn, float endFadeIn, float fadeInTime,
										 float startFadeOut, float endFadeOut, float fadeOutTime,
										 float fadeOutDelay)
	{
		yield return StartCoroutine(FadeMaterial(startFadeIn, endFadeIn, fadeInTime));
		yield return new WaitForSeconds(fadeOutDelay);
		yield return StartCoroutine(FadeMaterial(startFadeOut, endFadeOut, fadeOutTime));
	}

	private IEnumerator StartFadingMat()
	{
		yield return StartCoroutine(FadeMaterial(startFadeIn, endFadeIn, fadeInTime));
		yield return new WaitForSeconds(fadeOutDelay);
		yield return StartCoroutine(FadeMaterial(startFadeOut, endFadeOut, fadeOutTime));
	}

	private IEnumerator FadeGUIText(float startLevel, float endLevel, float time)
	{
		float speed = 1.0f / time;
		
		for (float t = 0.0f; t < 1.0; t += Time.deltaTime*speed)
		{ 
			float a = Mathf.Lerp (startLevel, endLevel, t);
			guiText.material.color = new Color (guiText.material.color.r,
										  guiText.material.color.g,
										  guiText.material.color.b, a);
			yield return 0;
		}
	}

	private IEnumerator FadeMaterial (float startLevel, float endLevel, float time)
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