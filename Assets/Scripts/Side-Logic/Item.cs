using UnityEngine;
using System.Collections;
using System.Linq;

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

    public int miningGoldValueGiven = 15;
    public int itemCostToCreate = 15;
    public int itemCostToUpgrade = 15;
    public string owner;

    bool upgraded = false;

    public bool Upgraded { get { return upgraded; } set { upgraded = value; } }

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
        SetColor(owner);
        _mapNode = Navigation.instance.NearestTo(transform.position, owner);
		_mapNode.nearbyItems.Add(this);
	}

    public Item SetColor (string owner)
    {
        var color = owner == Navigation.IA ? Color.red : Color.blue;

        var render = GetComponent<Renderer>();
        if(render)
            render.material.color = color;

        foreach (var mat in GetComponentsInChildren<Renderer>().Select(r => r.material))
        {
           mat.color = color;
        }
        return this;
    }

    public Item SetOwner(string owner)
    {
       this.owner = owner;
       return this;
    }

    public Item SetNearbyMapNode(string owner)
    {
        _mapNode = Navigation.instance.NearestTo(transform.position, owner);
        return this;
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
            if (_mapNode == null)
                return;
			_mapNode.nearbyItems.Remove(this);
			_mapNode = Navigation.instance.NearestTo(transform.position, owner);
			_mapNode.nearbyItems.Add(this);
		}
	}
}
