using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviourPun
{
    [SerializeField] private bool isOccupied;
    [SerializeField] private Vector3 locationForPiece;
    [SerializeField] private Vector3 locationForTerrain;
    [SerializeField] private Vector3 locationForTileCollider;
    [SerializeField] private char terrain;
    [SerializeField] private List<GameObject> neighbours;
    [SerializeField] private List<string> terrainAssets;
    [SerializeField] private List<Material> tileMaterials;
    [SerializeField] private Piece pieceInTile;
    private void Awake() //Sets up the locationFor varibles
    {
        this.transform.parent = GameObject.FindGameObjectWithTag("HexGrid").transform;
        locationForPiece = gameObject.transform.position;
        locationForPiece = new Vector3(locationForPiece.x, (float)2.881602, locationForPiece.z);
        locationForTileCollider = locationForPiece;
        locationForTerrain = new Vector3(transform.position.x, transform.position.y + (float)1.4, transform.position.z - (float)0.4);
    }
    [PunRPC]
    public void SetName(string name)
    {
        transform.name = name;
    }
    public char GetTerrain() { return terrain; } //Returns the terrain for this tile
    public bool GetIsOccupied() { return isOccupied; } //Returns whether the tile is occupied
    public Vector3 GetLocationForPiece() { return locationForPiece; } //returns the location ontop of a tile for a piece
    public Vector3 GetLocationForTileCollider() { return locationForTileCollider; }  //returns the location that can be used by a tile collider 
    public List<GameObject> GetNeighbours() { return neighbours; } //returns a list of all neighbouring tiles
    public Piece GetPieceInTile() { return pieceInTile; } //returns the piece located within this tile
    public void SetIsOccupied(bool o, GameObject p) //sets the occupation of the tile alongside assigning the player object 
    { 
        isOccupied = o; 
        if(o == true)
        {
            pieceInTile = p.GetComponent<Piece>();
        }
        else if (o ==false)
        {
            pieceInTile = null;
        }
    } 
    [PunRPC]
    public void SetTerrain(string t, int random) //dependant on what terrain is passed in, terrain for the tile is set which instantiates the correct asset 
    {
        terrain = char.Parse(t);
        MeshRenderer m = GetComponent<MeshRenderer>();
        if (terrain == '#')
        {
            GameObject x = PhotonNetwork.InstantiateSceneObject(terrainAssets[0], locationForTerrain, Quaternion.identity).gameObject;
            x.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
            m.material = tileMaterials[0];
        }
        else if(terrain == '^')
        {
            GameObject x = PhotonNetwork.InstantiateSceneObject(terrainAssets[random], locationForTerrain, Quaternion.identity).gameObject;
            x.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
            m.material = tileMaterials[0];
        }
        else if(terrain == '~')
        {
            m.material = tileMaterials[1];
            GameObject x = PhotonNetwork.InstantiateSceneObject(terrainAssets[5], locationForTerrain, Quaternion.identity).gameObject;
            x.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
        }
    }
    private void OnTriggerEnter(Collider other) //adds ajacent tiles to the tile list
    {
        if(other.tag == "Tile")
        {
            bool alreadyAdded = false;
            foreach (var n in neighbours)
            {
                if(n == other.gameObject)
                {
                    alreadyAdded = true;
                }
            }
            if(!alreadyAdded) { neighbours.Add(other.gameObject); }
        }
    }
    private void Start()
    {
        GameObject x = PhotonNetwork.InstantiateSceneObject("TileCollider", this.transform.position, Quaternion.identity);
        x.GetComponent<PhotonView>().RPC("SetTileObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
    }
}
