using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OBSOLETE

public class TileColliderManager : MonoBehaviourPun
{
    [SerializeField] private GameObject hexGrid;
    [SerializeField] private string tileCollider;
    private List<Tile> tiles = new List<Tile>();
    private void Awake()
    {
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid");
    }
    private void Start() //Instantiates all of the tile colliders at their proper locations whilist assigning them their correct tile
    {
        if(PhotonNetwork.IsMasterClient)
        {
            foreach (Transform c in hexGrid.transform)
            {
                tiles.Add(c.GetComponent<Tile>());
            }
            foreach (var t in tiles)
            {
                Vector3 f = t.GetLocationForTileCollider();
                f = new Vector3(f.x, f.y + (float)1.5, f.z);
                TileCollider x = PhotonNetwork.InstantiateSceneObject(tileCollider, f, new Quaternion(0, 0, 0, 0)).GetComponent<TileCollider>();
                //x.SetTileObject("Yeah this script dosnt work anymore");
                //x.GetComponent<PhotonView>().RPC("SetTileObject", RpcTarget.All, );
            }
        }
    }
}
