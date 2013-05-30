using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeGameState : MonoBehaviour
{
	public ArrayList nodes = new ArrayList ();
	public ArrayList nodeTexts = new ArrayList ();
	public ArrayList nodeModes = new ArrayList ();
	public ArrayList nodeTypes = new ArrayList ();
	public ArrayList progressBars = new ArrayList ();
	public ArrayList progressBarTexts = new ArrayList ();
	public ArrayList healthBars = new ArrayList ();
	public ArrayList healthBarTexts = new ArrayList ();
	public GUIText nodeText2;
	public GUIText nodeText3;
	public GUIText nodeText4;
	public GUIText nodeText5;
	public GUIText commandText;
	private bool fButtonPressed = false;
	private float commandTimer = 0f;
	private int nodeMode = 0;
	private bool nodeSelected = false;
	private int selectedNode = 0;
	public Camera main;
	private float maxProgressBarWidth = 100f;
	private float minProgressBarWidth = 0f;
	public GUITexture progressBar1;
	public GUITexture progressBar2;
	public GUITexture progressBar3;
	public GUITexture progressBar4;
	public GUITexture progressBar5;
	public GUIText progressBarText1;
	public GUIText progressBarText2;
	public GUIText progressBarText3;
	public GUIText progressBarText4;
	public GUIText progressBarText5;
	public GUITexture nodeHealthBar1;
	public GUITexture nodeHealthBar2;
	public GUITexture nodeHealthBar3;
	public GUITexture nodeHealthBar4;
	public GUITexture nodeHealthBar5;
	public GUIText nodeHealthBarText1;
	public GUIText nodeHealthBarText2;
	public GUIText nodeHealthBarText3;
	public GUIText nodeHealthBarText4;
	public GUIText nodeHealthBarText5;
	public bool mineButtonPressed = false;
	public bool expandButtonPressed = false;
	public bool fortifyButtonPressed = false;
	public bool defendButtonPressed = false;
	public bool reproduceButtonPressed = false;
	public bool node1ButtonPressed = false;
	public bool node2ButtonPressed = false;
	public bool node3ButtonPressed = false;
	public bool node4ButtonPressed = false;
	public bool node5ButtonPressed = false;
	public bool confirmButtonPressed = false;
	public Dictionary<int,GameObject> sortedNodeList = new Dictionary<int,GameObject> ();
	public GameObject[] resourceNodes;
	ClientPlayerController cpc;
	public NetworkPlayer netPlayer;
	
	
	void OnNetworkInstantiate (NetworkMessageInfo info)
	{
		resourceNodes = GameObject.FindGameObjectsWithTag ("ResourceNode");
		foreach (GameObject node in resourceNodes) {
			ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
			sortedNodeList.Add (nodeScript.resourceNodeNumber, node);
			//print ("NodeGameState Nodes dictionary: "+nodeScript.resourceNodeNumber);
		}
		

	}
	
	// Use this for initialization
	void Start ()
	{
		//main = GameObject.Find ("Main Camera").camera;
		
		
		
		nodeTexts.Add (this.guiText);
		nodeTexts.Add (nodeText2);
		nodeTexts.Add (nodeText3);
		nodeTexts.Add (nodeText4);
		nodeTexts.Add (nodeText5);
		
		nodeModes.Add ("Mine");
		nodeModes.Add ("Expand");
		nodeModes.Add ("Fortify");
		nodeModes.Add ("Defend");
		nodeModes.Add ("Reproduce");
		
		nodeTypes.Add ("Carbon");
		nodeTypes.Add ("Energon");
		nodeTypes.Add ("Metal");
		
		progressBars.Add (progressBar1);
		progressBars.Add (progressBar2);
		progressBars.Add (progressBar3);
		progressBars.Add (progressBar4);
		progressBars.Add (progressBar5);
		
		progressBarTexts.Add (progressBarText1);
		progressBarTexts.Add (progressBarText2);
		progressBarTexts.Add (progressBarText3);
		progressBarTexts.Add (progressBarText4);
		progressBarTexts.Add (progressBarText5);
		
		healthBars.Add (nodeHealthBar1);
		healthBars.Add (nodeHealthBar2);
		healthBars.Add (nodeHealthBar3);
		healthBars.Add (nodeHealthBar4);
		healthBars.Add (nodeHealthBar5);

	
		healthBarTexts.Add (nodeHealthBarText1);
		healthBarTexts.Add (nodeHealthBarText2);
		healthBarTexts.Add (nodeHealthBarText3);
		healthBarTexts.Add (nodeHealthBarText4);
		healthBarTexts.Add (nodeHealthBarText5);
	
		
		for (int i =0; i<5; i++) {
			GUIText progBarText = (GUIText)progressBarTexts [i];
			GUITexture progBar = (GUITexture)progressBars [i];
			GUIText healthBarText = (GUIText)healthBarTexts [i];
			
			
			progBarText.material.color = Color.grey;
			progBar.pixelInset = new Rect (progBar.pixelInset.x,
			progBar.pixelInset.y, minProgressBarWidth, progBar.pixelInset.height);
			
			healthBarText.material.color = Color.black;
			
			

		}
		
		
		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (nodes.Count > 0 && Network.player == netPlayer) {
			HarvestButtonGUI buttons = (HarvestButtonGUI)main.GetComponent (typeof(HarvestButtonGUI));
			buttons.showCommandButtons = true;
			buttons.showNodeButtons = false;
			buttons.showConfirmButton = false;
			
			for (int i =0; i<nodes.Count; i++) {
				GUIText text = (GUIText)nodeTexts [i];
				GameObject node = (GameObject)nodes [i];
				GUITexture progress = (GUITexture)progressBars [i];
				GUIText progText = (GUIText)progressBarTexts [i];
				GUITexture health = (GUITexture)healthBars [i];
				GUIText healthText = (GUIText)healthBarTexts [i];
				ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
				
				healthText.text = "Durability: " + nodeScript.nodeHealth + "/" + nodeScript.nodeDurability;
				
				Component[] healChildren = health.GetComponentsInChildren<GUITexture> ();
				foreach (GUITexture g in healChildren) 
					g.enabled = true;
			
				Component[] progChildren = progress.GetComponentsInChildren<GUITexture> ();
				foreach (GUITexture g in progChildren) 
					g.enabled = true;
				
				
				
				health.pixelInset = new Rect (health.pixelInset.x,
						health.pixelInset.y, (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.nodeHealth / (float)nodeScript.nodeDurability), progress.pixelInset.height);
					
				
				if (nodeScript.nodeMode == 0) {
					text.text = "Node " + (i + 1) + "\n"
							+ "Drone#: " + nodeScript.droneCount + "\n";
					progress.pixelInset = new Rect (progress.pixelInset.x,
						progress.pixelInset.y, (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.minedAmount / (float)nodeScript.resourceCapacity), progress.pixelInset.height);
					
					
					progText.text = (string)nodeModes [nodeScript.nodeMode] + ": " + nodeScript.minedAmount + "/" + nodeScript.resourceCapacity;
				}
				if (nodeScript.nodeMode == 1) {
					text.text = "Node " + (i + 1) + "\n"
							+ "Drone#: " + nodeScript.droneCount;
					
					progress.pixelInset = new Rect (progress.pixelInset.x,
						progress.pixelInset.y, minProgressBarWidth + (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.progress / (float)nodeScript.calculatedCapacityUpgradeCost ()), progress.pixelInset.height);
					
					progText.text = (string)nodeModes [nodeScript.nodeMode] + " " + nodeScript.capacityLevel;
					
					
				}
				if (nodeScript.nodeMode == 2) {
					text.text = "Node " + (i + 1) + "\n"
							+ "Drone#: " + nodeScript.droneCount + "\n";
							
					
					progress.pixelInset = new Rect (progress.pixelInset.x,
						progress.pixelInset.y, minProgressBarWidth + (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.progress / (float)nodeScript.calculatedDurabilityUpgradeCost ()), progress.pixelInset.height);
					
					progText.text = (string)nodeModes [nodeScript.nodeMode] + " " + nodeScript.durabilityLevel;
					
				}
				if (nodeScript.nodeMode == 3) {
					text.text = "Node " + (i + 1) + "\n"
							+ "Drone#: " + nodeScript.droneCount + "\n";
					
					progress.pixelInset = new Rect (progress.pixelInset.x,
						progress.pixelInset.y, minProgressBarWidth + (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.progress / (float)nodeScript.calculatedDefenseUpgradeCost ()), progress.pixelInset.height);
					
					progText.text = (string)nodeModes [nodeScript.nodeMode] + " " + nodeScript.turretLevel;
					
				}
				if (nodeScript.nodeMode == 4) {
					text.text = "Node " + (i + 1);
					
					progress.pixelInset = new Rect (progress.pixelInset.x,
						progress.pixelInset.y, minProgressBarWidth + (maxProgressBarWidth - minProgressBarWidth) * 
						((float)nodeScript.progress / (float)nodeScript.calculatedReproductionUpgradeCost (getTotalDrones())), progress.pixelInset.height);
					
					progText.text = (string)nodeModes [nodeScript.nodeMode] + " " + (nodeScript.droneCount + 1);
				}
			}
			
			
			
			if (nodeSelected == false) {
				
				//HarvestButtonGUI buttons = (HarvestButtonGUI) main.GetComponent(typeof(HarvestButtonGUI));
				//buttons.showCommandButtons = true;
				//buttons.showNodeButtons = false;
				//buttons.showConfirmButton = false;
				
				if (Input.GetButtonDown ("Mine") || mineButtonPressed) {
					fButtonPressed = true;
					nodeMode = 0;
					commandText.text = "Mine Resource\n" +
									"Select Node";
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
	
					confirmButtonPressed = false;
				}
				if (Input.GetButtonDown ("Expand") || expandButtonPressed) {
					fButtonPressed = true;
					nodeMode = 1;
					commandText.text = "Expand Capacity\n" +
									"Select Node";
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if (Input.GetButtonDown ("Fortify") || fortifyButtonPressed) {
					fButtonPressed = true;
					nodeMode = 2;
					commandText.text = "Fortify Durabilty\n" +
									"Select Node";
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if (Input.GetButtonDown ("Defend") || defendButtonPressed) {
					fButtonPressed = true;
					nodeMode = 3;
					commandText.text = "Make Defender\n" +
									"Select Node";
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if (Input.GetButtonDown ("Reproduce") || reproduceButtonPressed) {
					fButtonPressed = true;
					nodeMode = 4;
					commandText.text = "Produce Drone\n" +
									"Select Node";
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
			}
			if (fButtonPressed) {
				commandTimer += Time.deltaTime;
				
				if ((Input.GetButtonDown ("Node1") || node1ButtonPressed) && nodes.Count >= 1) {
					GameObject node = (GameObject)nodes [0];
					ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
					nodeCommandResponse (nodeScript);
					selectedNode = 0;
					
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
					
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if ((Input.GetButtonDown ("Node2") || node2ButtonPressed) && nodes.Count >= 2) {
					GameObject node = (GameObject)nodes [1];
					ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
					nodeCommandResponse (nodeScript);
					selectedNode = 1;
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if ((Input.GetButtonDown ("Node3") || node3ButtonPressed) && nodes.Count >= 3) {
					GameObject node = (GameObject)nodes [2];
					ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
					nodeCommandResponse (nodeScript);
					selectedNode = 2;
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if ((Input.GetButtonDown ("Node4") || node4ButtonPressed) && nodes.Count >= 4) {
					GameObject node = (GameObject)nodes [3];
					ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
					nodeCommandResponse (nodeScript);
					selectedNode = 3;
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				if ((Input.GetButtonDown ("Node5") || node5ButtonPressed) && nodes.Count >= 5) {
					GameObject node = (GameObject)nodes [4];
					ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
					nodeCommandResponse (nodeScript);
					selectedNode = 4;
					
					mineButtonPressed = false;
					expandButtonPressed = false;
					fortifyButtonPressed = false;
					defendButtonPressed = false;
					reproduceButtonPressed = false;
	
					node1ButtonPressed = false;
					node2ButtonPressed = false;
					node3ButtonPressed = false;
					node4ButtonPressed = false;
					node5ButtonPressed = false;
					
					confirmButtonPressed = false;
				}
				
				//HarvestButtonGUI buttons = (HarvestButtonGUI) main.GetComponent(typeof(HarvestButtonGUI));
				buttons.showCommandButtons = false;
				buttons.showNodeButtons = true;
				buttons.showConfirmButton = false;
				
				if (nodeSelected) {
					buttons.showCommandButtons = false;
					buttons.showNodeButtons = false;
					buttons.showConfirmButton = true;
				}
				
				confirmCommand ();
				//5 second timer to choose a node to apply the command to
				if (commandTimer > 5f) {
					commandTimer = 0f;
					fButtonPressed = false;
					nodeSelected = false;
					commandText.text = "";
				}
			}
		}
		
		/*	for (int j=nodes.Count; j<5; j++) {
			GUIText text = (GUIText)nodeTexts [j];
			text.text = "";
			
			GUIText proText = (GUIText)progressBarTexts [j];
			proText.text = "";
			
			GUIText healthText = (GUIText) healthBarTexts[j];
			healthText.text = "";
			
			GUITexture progress = (GUITexture)progressBars[j];
			GUITexture health = (GUITexture)healthBars[j];
				
			Component[] healChildren = health.GetComponentsInChildren<GUITexture> ();
			foreach (GUITexture g in healChildren) 
				g.enabled = false;
			
			Component[] progChildren = progress.GetComponentsInChildren<GUITexture> ();
			foreach (GUITexture g in progChildren) 
				g.enabled = false;
		}*/
	
	}
	
	[RPC]
	void addNode (int nodeKey)
	{
		if (nodes.Contains (sortedNodeList [nodeKey]) == false) {
			setOwnership ();
			//ortedNodeList [nodeKey].owner = netPlayer;
			HarvestButtonGUI buttons = (HarvestButtonGUI)main.GetComponent (typeof(HarvestButtonGUI));
			buttons.setOwnership ();
			if (Network.player == netPlayer)
				nodes.Add (sortedNodeList [nodeKey]);
		}
		//nodes.Add(serverController.sortedNodeList[nodeKey]);
	}
	
	[RPC]
	void removeNode (int nodeKey)
	{
		if (nodes.Contains (sortedNodeList [nodeKey]) == true) {
			setOwnership ();
			HarvestButtonGUI buttons = (HarvestButtonGUI)main.GetComponent (typeof(HarvestButtonGUI));
			buttons.setOwnership ();
			if (Network.player == netPlayer)
				nodes.Remove (sortedNodeList [nodeKey]);
		}
		//nodes.Remove(serverController.sortedNodeList[nodeKey]);
		//print ("Node Removed to NodeGameState "+ nodes.Count);
		//print ("Ownership NodeGameState "+ n.ToString() + " "+ Network.player.ToString());
	}
	
	void setOwnership ()
	{
		GameObject tank = transform.parent.parent.parent.FindChild ("NewTank").gameObject;
		cpc = (ClientPlayerController)tank.GetComponent (typeof(ClientPlayerController));
		netPlayer = cpc.getOwner ();
	}
	
	private void nodeCommandResponse (ResourceNodeScript nodeScript)
	{
		
		//blocks commands if chosen node is busy.
		
		
		if (nodeMode == 0) {
			nodeScript.setMode (nodeMode);
			commandTimer = 0f;
			fButtonPressed = false;
			nodeSelected = false;
			commandText.text = "Test";
		}
		
		if (nodeScript.isBusy == false) {
			if (nodeMode == 1) {
				if (nodeScript.minedAmount >= nodeScript.calculatedCapacityUpgradeCost ()) {
					commandTimer = 0f;
					commandText.text = "Cost: " + (nodeScript.calculatedCapacityUpgradeCost ()) + "\n" + 
								"Hit ` Key again To Commit Command\n" +
								"Benefit: 25 capacity";
					nodeSelected = true;
				} else {
					commandText.text = "Insuffient Materials";
					commandTimer = 3f;
				
				}
				//confirmCommand(nodeScript);
			
			}
			if (nodeMode == 2) {
				if (nodeScript.minedAmount >= nodeScript.calculatedDurabilityUpgradeCost ()) {
					commandTimer = 0f;
					commandText.text = "Cost: " + (nodeScript.calculatedDurabilityUpgradeCost ()) + "\n" + 
								"Hit ` Key again To Commit Command\n" +
								"Benefit: 50 Durability";
					nodeSelected = true;
				} else {
					commandText.text = "Insuffient Materials";
					commandTimer = 3f;
				
				}
				//confirmCommand(nodeScript);
			}
			if (nodeMode == 3) {
				if (nodeScript.minedAmount >= nodeScript.calculatedDefenseUpgradeCost () && nodeScript.droneCount > 1) {
					commandTimer = 0f;
					commandText.text = "Cost: 1 drone and" + (nodeScript.calculatedDefenseUpgradeCost ()) + "\n" + 
								"Hit ` Key again To Commit Command\n" +
								"Benefit: 1 Turret Level";	
					nodeSelected = true;
					//confirmCommand(nodeScript);
				} else {
					commandText.text = "Insuffient Materials";
					commandTimer = 3f;
				
				}
			}
			if (nodeMode == 4) {
				if (nodeScript.minedAmount >= nodeScript.calculatedReproductionUpgradeCost (getTotalDrones())) {
					commandTimer = 0f;
					commandText.text = "Cost: " + (nodeScript.calculatedReproductionUpgradeCost (getTotalDrones())) + "\n" + 
								"Hit ` Key again To Commit Command\n" +
								"Benefit: 1 Drone";	
					nodeSelected = true;
					//confirmCommand(nodeScript);
				} else {
					commandText.text = "Insuffient Materials";
					commandTimer = 3f;
				
				}
			}
		} 
		else {
			commandText.text = "Node Busy";
			commandTimer = 3f;
		}
		
	}
	
	public void confirmCommand ()
	{
		
		
		GameObject node = (GameObject)nodes [selectedNode];
		ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
		
		if (Input.GetButtonDown ("Confirm") || confirmButtonPressed && nodeSelected) {
			nodeScript.setMode (nodeMode);
			fButtonPressed = false;
			commandTimer = 0f;
			commandText.text = "";
			nodeSelected = false;
			
			mineButtonPressed = false;
			expandButtonPressed = false;
			fortifyButtonPressed = false;
			defendButtonPressed = false;
			reproduceButtonPressed = false;
	
			node1ButtonPressed = false;
			node2ButtonPressed = false;
			node3ButtonPressed = false;
			node4ButtonPressed = false;
			node5ButtonPressed = false;
			
		}
			
	}
	
	public int getTotalDrones()
	{
		int nodeDroneCount = 0;
		for (int i =0; i<nodes.Count; i++)
		{
			GameObject node = (GameObject)nodes [i];
			ResourceNodeScript nodeScript = (ResourceNodeScript)node.GetComponent (typeof(ResourceNodeScript));
			nodeDroneCount =nodeDroneCount +nodeScript.droneCount;
		}
		GameObject tank = transform.parent.parent.parent.FindChild ("NewTank").gameObject;
		PlayerGameState pgs = (PlayerGameState)tank.GetComponent (typeof(PlayerGameState));
		return nodeDroneCount+pgs.playerDroneCount;
	}
	
}
