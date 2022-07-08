using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
public class TileCollider : MonoBehaviourPun
{
    private UIManager uiManager;
    private GameManager gameManager;
    [SerializeField] private GameObject tileObject;
    private Camera cam;
    private Tile tile;
    private Vector3 downPosition;
    private Vector3 upPosition;
    private float raiseFactor = 0.3f;
    [PunRPC]
    public void SetTileObject(int viewOfTile) //Sets up the tile varible alongside creating the vectors for the upwards and downwards tile positions
    {
        tileObject = PhotonView.Find(viewOfTile).gameObject;
        tile = tileObject.GetComponent<Tile>();
        downPosition = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y, tileObject.transform.position.z);
        upPosition = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + raiseFactor, tileObject.transform.position.z);
        transform.position = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + (float)1.2, tileObject.transform.position.z);
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnMouseEnter() //When the mouse hovers over the tile the tile is elevated 
    {
        if(GetComponent<PhotonView>().IsMine)
        {
            tile.transform.position = upPosition;
        }
        else
        {
            this.photonView.RequestOwnership();
            tile.photonView.RequestOwnership();
            gameManager.photonView.RequestOwnership();
        }
    }
    private void OnMouseOver()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            tile.transform.position = upPosition;
        }
    }
    private void OnMouseExit() //When the mouse moves away from the tile the tile decents back to its original position
    {
        tile.transform.position = downPosition;
    }
    private void OnMouseDown() //When a click is recieved on the tile send the data to the according functions
    {
        if (!IsPointerOverUIObject() && (gameManager.GetIsPlayerOnesTurn() == (bool)PhotonNetwork.LocalPlayer.CustomProperties["isPlayerOne"]))
        {
            if (gameManager.GetIsMoveButtonPressed()) /////This works both sides
            {
                gameManager.MovePiece(tile);/////This method is called but needs PUNRPC share photon ID in metghid
            }
            else if (gameManager.GetIsSpawnButtonPressed())/////This works both sides
            {
                gameManager.SpawnPiece(tile); /////This method is called but needs PUNRPC share photon ID in metghid
            }
            else
            {
                gameManager.SetTileInQuestion(tile);
                if (uiManager.GetIsMenuPannelOpen())
                {
                    uiManager.ToggleOffAllMenuPannels();
                }
                else
                {
                    if (tile.GetPieceInTile() != null && (gameManager.GetIsPlayerOnesTurn() == tile.GetPieceInTile().GetBelongsToPlayerOne()))
                    {
                        
                        uiManager.ToggleOnPieceMenuPannel(cam.WorldToScreenPoint(new Vector3(transform.position.x + 4, transform.position.y - (float)1.4, transform.position.z)), tile.GetPieceInTile().GetPieceType(), tile);
                    }
                }
            }
        }


    }
    private bool IsPointerOverUIObject() //So that the mouse dosnt pass through a button
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
