using UnityEngine;
using System.Collections;

public class VolumeControl : MonoBehaviour
{

	public float sliderVolumeLevel = 0f;
	public bool sliderHidden = false;
	
	void OnGUI()
	{
		sliderHidden = GUI.Toggle (new Rect(0f, 0f, 150f, 20f), sliderHidden, "Hide/Show Volume");
		if(!sliderHidden)
		{
			sliderVolumeLevel = GUI.HorizontalSlider(new Rect(25f, 25f, 100f, 30f), 
				sliderVolumeLevel, 0.0f, 1.0f);
			GUI.Label (new Rect(25f, 75f, 200f, 20f), "Volume: "+100f*sliderVolumeLevel);
			audio.volume = sliderVolumeLevel;
		}
	}
	
}
