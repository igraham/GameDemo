using UnityEngine;
using System.Collections;

public class GUITextureCorrect : MonoBehaviour {
	
	public float x =50;
	public float y=50;
	public float width=100;
	public float height=100;
	public float z=0;
	
	// Use this for initialization
	void Start () {
		transform.position = Vector3.zero;
        transform.localScale = Vector3.zero;
        guiTexture.pixelInset = new Rect(x, y, width, height);
		transform.position = new Vector3(Vector3.zero.x,Vector3.zero.y,z);
	}
	
}
