using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour {

    public GameObject defensePrefab;
    public GameObject cannonPrefab;

    public float maxRaycastDistance = 300;
    public LayerMask hoverLayer;

    public float radiusOfNodesGizmos = 0.2f;

    Item _itemToInstantiate;
    IEnumerable _allNodes;
    Navigation _nav;
    Camera _cam;

    ItemType _lastTipe;

    void Start () {
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_CREATE, OnCreate);
        EventsManager.SubscribeToEvent(EventsConstants.PLAYER_UPGRADE, OnUpgrade);

        _nav = Navigation.instance;
        if (!_nav)
            throw new Exception("navigation null");

        _cam = Camera.main;

        if (!_cam)
            throw new Exception("Cam null");
    }

    private void OnUpgrade(object[] parameterContainer)
    {
        throw new NotImplementedException();
    }

    private void OnCreate(object[] parameterContainer)
    {
        EventsManager.TriggerEvent(EventsConstants.SUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });

        if ((ItemType)parameterContainer[0] == ItemType.Defense)
        {
            _itemToInstantiate = Instantiate(defensePrefab).GetComponent<Item>();
            _lastTipe = ItemType.Defense;
        }
        else if ((ItemType)parameterContainer[0] == ItemType.Cannon)
        {
            _itemToInstantiate = Instantiate(cannonPrefab).GetComponent<Item>();
            _lastTipe = ItemType.Cannon;
        }
        else
        {
            Debug.Log("Amigo plisss");
            EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
        }
    }

//     Vector3 debug = new Vector3();
//     Vector3 debug2 = new Vector3();

    void OnUpdate()
    {
        RaycastHit hit;
        var ray = _cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.yellow, Time.deltaTime);

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, hoverLayer))
        {
            if (_lastTipe == ItemType.Defense)
            {
                _itemToInstantiate.GetComponent<Defense>().Activate();
            }
            else if (_lastTipe == ItemType.Cannon)
            {
                _itemToInstantiate.GetComponent<Cannon>().Create();
            }
//             debug = hit.collider.transform.position;
//             debug2 = _nav.NearestTo(hit.point).position;
            _itemToInstantiate.transform.position = _nav.NearestTo(hit.point).position;
        }

        if(Input.GetMouseButtonDown(0))
        {
            EventsManager.TriggerEvent(EventsConstants.DESUBSCRIBE_UPDATE, new object[] { (Action)OnUpdate });
            EventsManager.TriggerEvent(EventsConstants.BLOCK_PLAYER_IF_FALSE, new object[] { true });
            EventsManager.TriggerEvent(EventsConstants.ITEM_CREATED, new object[] { _lastTipe });
        }
    }

    private void OnDrawGizmos()
    {
//         Gizmos.color = Color.blue;
//         Gizmos.DrawWireSphere(debug, 0.5f);
// 
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(debug2, 0.5f);
    }

    //     IEnumerable<GameObject> _gizmos;
    // 
    //     void ShowNodesGizmos (bool showNodes)
    //     {
    //         if(!showNodes)
    //         {
    //             for (int i = _gizmos.Count() - 1; i >= 0 ; i--)
    //             {
    //                 Destroy( _gizmos.Skip(i).First());
    //             }
    //         }
    // 
    //         _gizmos = _nav.AllMapNodes().Where(node => node.accessible).Aggregate(new FList<GameObject>(), (fList, node) =>
    //         {
    //             var sphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
    //             sphere.transform.position = node.position;
    //             sphere.transform.localScale = new Vector3(radiusOfNodesGizmos, radiusOfNodesGizmos, radiusOfNodesGizmos);
    //             sphere.GetComponent<MeshRenderer>().material.color = Color.blue;
    //             var col = sphere.GetComponent<Collider>();
    //             if (col)
    //             {
    //                 col.enabled = false;
    //             }
    //             return fList + sphere;
    //         });
    //     }
}
