using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour
{

	public GUIText display;
	public float secondsToCountDown = 3f;
	public string lastTime = "";
	public bool countDownStarted = false;

	[RPC]
	void startCountdown()
	{
		GUIText initial = Instantiate(display) as GUIText;
		initial.text = ""+(int)secondsToCountDown;
		countDownStarted = true;
	}

	void Update()
	{
		if(countDownStarted)
		{
			secondsToCountDown-=Time.deltaTime;
			string timeText = ""+(int)Mathf.Ceil(secondsToCountDown);
			if(timeText != lastTime)
			{
				lastTime = timeText;
				GUIText num = Instantiate(display) as GUIText;
				num.text = timeText;
			}
			if(secondsToCountDown <= 0)
			{
				networkView.RPC ("loadGame",RPCMode.AllBuffered, "RockyCrag");
				Destroy (gameObject);
			}
		}
	}
}
