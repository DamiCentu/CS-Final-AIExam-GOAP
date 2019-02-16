using UnityEngine;
using System.Collections;

public enum ItemType
{
	Invalid,
	Key,
	WaitZone,
	Entity,
	Mace,
	PastaFrola,
    Mine,
    Defense,
    Cannon,
    Core,
    WorkTable
}

public class Item : MonoBehaviour
{
	public ItemType type;
	
	MapNode _mapNode;
	bool insideInventory;

    public int goldValue = 15;

	public void OnInventoryAdd()
    {
		Destroy(GetComponent<Rigidbody>());
		insideInventory = true;
		if(_mapNode != null)
			_mapNode.nearbyItems.Remove(this);
	}

	public void OnInventoryRemove()
    {
		gameObject.AddComponent<Rigidbody>();
		insideInventory = false;
	}

	void Start ()
    {
		_mapNode = Navigation.instance.NearestTo(transform.position);
		_mapNode.nearbyItems.Add(this);	
	}

	public void Kill()
    {
		var ent = GetComponent<Entity>();
		if(ent != null)
        {
			foreach(var it in ent.RemoveAllitems())
				it.transform.parent = null;
		}
		DestroyObject(gameObject);
	}

	void OnDestroy()
    {
		_mapNode.nearbyItems.Remove(this);
	}

    //Aplcando que podría mejorarse esto?
	void Update ()
    {
		if(!insideInventory)
        {
			_mapNode.nearbyItems.Remove(this);
			_mapNode = Navigation.instance.NearestTo(transform.position);
			_mapNode.nearbyItems.Add(this);
		}
	}
}
