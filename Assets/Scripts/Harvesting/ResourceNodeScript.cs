using UnityEngine;
using System.Collections;

public class ResourceNodeScript : MonoBehaviour
{
 
	public int resourceAmount = 3000;
	public int reourceType = 0;
	public int nodeDurability = 100;
	public int nodeHealth = 100;
	public GameObject rawResourceModel;
	public GameObject nodeModel;
	public int droneCount = 0;
	public int resourceCapacity = 100;
	public int minedAmount = 0;
	//public GUIText gameState;
	public bool isNode = false;
	public float timer = 0f;
	public int nodeMode = 0;
	public int durabilityLevel = 1;
	public int turretLevel = 1;
	public int capacityLevel = 1;
	public int progress = 0;
	public int previousNodeMode = 0;
	public bool acceptCommands = true;
	public int resourceNodeNumber = 0;
	public int extractable =0;
	public bool isBusy =false;
	int reprodCost =0;
	public GameObject droppedResources;
//	public GameObject turretModel;
	
	void Start ()
	{
		Component[] nodeRenderers = nodeModel.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in nodeRenderers)
		{
			r.enabled = false;
		}
		/*Component[] turretRenderers = turretModel.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in turretRenderers) {
			r.enabled = false;
		}*/
	}
	
	void Update ()
	{
		if(nodeHealth <= 0)
			droneCount =0;
		//NodeGameState gState = (NodeGameState)gameState.GetComponent (typeof(NodeGameState));
		//if (gState.nodes.Count < 6) {
			if (droneCount > 0 && isNode == false)
			{
				timer = 0f;
				Component[] resourcRenderers = rawResourceModel.GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in resourcRenderers)
				{
					r.enabled = false;
				}
				Component[] nodeRenderers = nodeModel.GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in nodeRenderers)
				{
					r.enabled = true;
				}
				//gState.addNode (this.gameObject);
				isNode = true;
				
				gameObject.tag = "HasDrones";
				if(turretLevel >1)
				{
					/*TurretController tcontrol = (TurretController) turretModel.GetComponent(typeof(TurretController));
					tcontrol.turretActive = true;
					Component[] turretRenderers = turretModel.GetComponentsInChildren<Renderer>();
					foreach (Renderer r in turretRenderers)
					{
						r.enabled = true;
					}*/
				}
			}
			if (droneCount <= 0 && isNode)
			{
				Component[] resourcRenderers = rawResourceModel.GetComponentsInChildren<Renderer>();
				foreach (Renderer r in resourcRenderers)
				{
					r.enabled = true;
				}
				Component[] nodeRenderers = nodeModel.GetComponentsInChildren<Renderer>();
				foreach (Renderer r in nodeRenderers)
				{
					r.enabled = false;
				}	
				//reset node stats to base
				minedAmount = 0;
				nodeHealth = 100;
				nodeDurability = 100;
				resourceCapacity = 100;
				durabilityLevel = 1;
				turretLevel = 1;
				capacityLevel = 1;
				progress = 0;
				previousNodeMode = 0;
				acceptCommands = true;
				isBusy =false;
				/*Component[] turretRenderers = turretModel.GetComponentsInChildren<Renderer> ();
				foreach (Renderer r in turretRenderers)
				{
					r.enabled = false;
				}
				TurretController tcontrol = (TurretController) turretModel.GetComponent(typeof(TurretController));
				tcontrol.turretActive = false;*/
				//gState.removeNode (this.gameObject);
				isNode = false;
				gameObject.tag = "ResourceNode";
				GameObject dr = Network.Instantiate(droppedResources,transform.position,Quaternion.identity,0) as GameObject;
				CollectDroppedResource cdr = (CollectDroppedResource) dr.GetComponent(typeof(CollectDroppedResource));
				cdr.setResourceAmount(minedAmount/2);
			}
		//}
		if (isNode)
		{
			if(nodeMode ==0)
			{
				isBusy =false;
				if (minedAmount < resourceCapacity && isNode)
				{
					timer += Time.deltaTime * droneCount;
					minedAmount = Mathf.RoundToInt (timer);
				}
				if (minedAmount > resourceCapacity)
					minedAmount = resourceCapacity;
			}
			if(nodeMode ==1)
			{
				timer += Time.deltaTime * droneCount;
				progress = Mathf.RoundToInt (timer);
				isBusy = true;
				if(progress >= calculatedCapacityUpgradeCost())
				{
					minedAmount = minedAmount - calculatedCapacityUpgradeCost();
					resourceAmount = resourceAmount - calculatedCapacityUpgradeCost();
					resourceCapacity = resourceCapacity + 25;
					increaseCapacityLevel();
					setModeZero();
					acceptCommands = true;
				}
			}
			if(nodeMode ==2)
			{
				timer += Time.deltaTime * droneCount;
				progress = Mathf.RoundToInt (timer);
				isBusy = true;
				if(progress >= calculatedDurabilityUpgradeCost())
				{
					minedAmount = minedAmount - calculatedDurabilityUpgradeCost();
					resourceAmount = resourceAmount - calculatedDurabilityUpgradeCost();
					nodeDurability = nodeDurability + 50;
					nodeHealth=nodeDurability;
					increaseDurabilityLevel();
					setModeZero();
					acceptCommands = true;
				}
			}
			if(nodeMode ==3)
			{
				timer += Time.deltaTime * droneCount;
				progress = Mathf.RoundToInt (timer);
				isBusy = true;
				if(progress >= calculatedDefenseUpgradeCost())
				{
					//nodeDurability = nodeDurability + 50;
					minedAmount = minedAmount - calculatedDefenseUpgradeCost();
					resourceAmount = resourceAmount - calculatedDefenseUpgradeCost();
					increaseTurretLevel();
					setModeZero();
					acceptCommands = true;
				}
			}
			if(nodeMode ==4)
			{
				timer += Time.deltaTime * droneCount;
				progress = Mathf.RoundToInt (timer);
				isBusy = true;
				if(progress >= reprodCost)
				{
					addDrone();
					minedAmount = minedAmount - reprodCost;
					resourceAmount = resourceAmount - reprodCost;
					setModeZero();
					acceptCommands = true;
				}
			}
		}	
	}
	
	[RPC]
	public void damageNode(int damage)
	{
		if(isNode)
			nodeHealth -= damage;
	}
	
	public int getRemainingResource()
	{
		return resourceAmount - minedAmount;
	}
	
	[RPC]
	public void addDrone()
	{
		if (droneCount < 10)
			droneCount++;
	}
	
	[RPC]
	public void removeDrone()
	{
		if(droneCount > 0)
			droneCount--;
	}
	
	[RPC]
	public void extractResources()
	{
		
		resourceAmount = getRemainingResource();
		int extracted = minedAmount;
		//Debug.Log(""+ minedAmount);
		timer = 0;
		minedAmount = 0;
		extractable = extracted;
		
			
		//networkView.RPC("setExtractionAmount",RPCMode.AllBuffered,extracted,extracted);
	}
	
	public void setMode(int mode)
	{
		previousNodeMode = nodeMode;
		nodeMode = mode;
		
		if(previousNodeMode != nodeMode && acceptCommands)
		{
			minedAmount = Mathf.RoundToInt(timer);
			timer = 0;
			acceptCommands = false;
		}
	}
	
	public void setModeZero()
	{
		previousNodeMode = nodeMode;
		nodeMode = 0;
		if(previousNodeMode != nodeMode)
		{
			timer = 0+minedAmount;
		}
	}
	
	public void increaseDurabilityLevel()
	{
		durabilityLevel++;
	}
	
	public void increaseTurretLevel()
	{
		turretLevel++;
	}
	
	public void increaseCapacityLevel()
	{
		capacityLevel++;
	}
	
	public int calculatedCapacityUpgradeCost()
	{
		return 25+ 25 * capacityLevel;
	}
	
	public int calculatedDurabilityUpgradeCost()
	{
		return 25*Mathf.RoundToInt(Mathf.Pow(2,durabilityLevel))+(nodeDurability - nodeHealth);
	}
	
	public int calculatedDefenseUpgradeCost()
	{
		return 50+Mathf.RoundToInt(Mathf.Pow(2,turretLevel));
	}
	
	public int calculatedReproductionUpgradeCost(int totalDrones)
	{
		int cost =0;
		cost = 150+(totalDrones-10)*25;
		if(cost < 150)
			cost=150;
			reprodCost = cost;
		return cost;
	}
}
