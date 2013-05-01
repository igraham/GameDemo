using UnityEngine;
using System.Collections;

//Goes onto main camera

public class HarvestButtonGUI : MonoBehaviour {
	
	
	public bool showCommandButtons = false;
	public bool showNodeButtons = false;
	public bool showConfirmButton = false;
	public GUIText gameState;
	NodeGameState state;
	NetworkPlayer n;
	// Use this for initialization
	void Start () {
		//print ("Harvest Button tests" + n.ToString() + " " + Network.player.ToString());
	}
	
	void OnGUI () {
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (125, 0, 540, 50));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
		
		if(showCommandButtons && Network.player == n)
		{
		// We'll make a box so you can see where the group is on-screen.
			GUI.Box (new Rect (0,0,540,100), "Commands");
			
			if(GUI.Button (new Rect (0,20,100,30), "Mine(F1)"))
				state.mineButtonPressed = true;
			if(GUI.Button (new Rect (110,20,100,30), "Expand(F2)"))
				state.expandButtonPressed = true;
			if(GUI.Button (new Rect (220,20,100,30), "Fortify(F3)"))
				state.fortifyButtonPressed = true;
			if(GUI.Button (new Rect (330,20,100,30), "Defend(F4)"))
				state.defendButtonPressed = true;
			if(GUI.Button (new Rect (440,20,100,30), "Reproduce(F5)"))
				state.reproduceButtonPressed = true;
		}
		
		if(showNodeButtons && Network.player == n)
		{
		// We'll make a box so you can see where the group is on-screen.
			GUI.Box (new Rect (0,0,550,100), "Nodes");
			
			if(GUI.Button (new Rect (0,20,100,30), "Node 1")){
				state.node1ButtonPressed = true;	
			}
			if(GUI.Button (new Rect (100,20,100,30), "Node 2"))
				state.node2ButtonPressed = true;;
			if(GUI.Button (new Rect (200,20,100,30), "Node 3"))
				state.node3ButtonPressed = true;;
			if(GUI.Button (new Rect (300,20,100,30), "Node 4"))
				state.node4ButtonPressed = true;;
			if(GUI.Button (new Rect (400,20,100,30), "Node 5"))
				state.node5ButtonPressed = true;;
		}
		
		
		if(showConfirmButton && Network.player == n)
		{
		// We'll make a box so you can see where the group is on-screen.
			GUI.Box (new Rect (0,0,550,100), "Confirmation");
			
			if(GUI.Button (new Rect (200,20,100,30), "Confirm(`)"))
				state.confirmButtonPressed = true;
			
		}
		
		
		
		// End the group we started above. This is very important to remember!
		GUI.EndGroup ();
	}
	
	public void setOwnership()
		{
			state = (NodeGameState) gameState.GetComponent (typeof(NodeGameState));
			ClientPlayerController cpc = (ClientPlayerController) transform.parent.GetComponent(typeof(ClientPlayerController));
			n =  cpc.getOwner();
			
		}
}
