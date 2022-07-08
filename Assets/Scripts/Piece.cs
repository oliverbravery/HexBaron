using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviourPun
{
    [SerializeField] private bool belongsToPlayerOne;
    [SerializeField] private char pieceType;
    [SerializeField] private bool destroyed = false;
    [SerializeField] private Tile tilePieceIsOn;
    [SerializeField] private List<string> styles;
    [SerializeField] private List<Material> materials;
    private int chopAmount = 1;
    private int digAmount = 1;
    private int upgradeCost = 5;
    private int spawnCost = 3;
    private GameObject cosmetic;

    public Tile GetTilePieceIsOn() { return tilePieceIsOn; }
    public bool GetDestroyed() { return destroyed; } //returns whether the tile is destroyed
    public int GetUpgradeCost() { return upgradeCost; }
    public int GetSpawnCost() { return spawnCost; }
    public int GetChopAmount() { return chopAmount; }
    public int GetDigAmount() { return digAmount; }
    [PunRPC]
    public void SetDestroyed(bool d) //Destorys the piece object whilist changing the state of the tile's occupation to false
    { 
        destroyed = d; 
        if(destroyed)
        {
            tilePieceIsOn.SetIsOccupied(false, gameObject);
            if(gameObject.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    public bool GetBelongsToPlayerOne() { return belongsToPlayerOne; } //Returns which player the piece belongs to
    [PunRPC]
    public void SetOwnedByPlayerOne(bool o) //Sets who owns the piece alongside changing the colour of the piece accordingly
    {
        belongsToPlayerOne = o; 
        if(belongsToPlayerOne == true)
        {
            GetComponent<MeshRenderer>().material = materials[0];
        }
        else
        {
            GetComponent<MeshRenderer>().material = materials[1];
        }
    }
    [PunRPC]
    public void SetPieceType(string p) //Sets the pieces type alongside changing the pieces cosmetics 
    {
        pieceType = char.Parse(p); 
        if(pieceType == 'B')
        {
            cosmetic = PhotonNetwork.InstantiateSceneObject(styles[0], new Vector3(transform.position.x, (float)2.456, transform.position.z), new Quaternion(0,0,0,0)).gameObject;
            cosmetic.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
            foreach (Transform c in transform)
            {
                if (c.tag == "Cosmetic" && c.name != "Crown(Clone)") { Destroy(c.gameObject); }
            }
        }
        else if (pieceType == 'L')
        {
            cosmetic = PhotonNetwork.InstantiateSceneObject(styles[1], new Vector3(transform.position.x - (float) 0.6, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0)).gameObject;
            cosmetic.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
            foreach (Transform c in transform)
            {
                if (c.tag == "Cosmetic" && c.name != "Fire_Axe(Clone)") { Destroy(c.gameObject); }
            }
        }
        else if(pieceType == 'P')
        {
            cosmetic = PhotonNetwork.InstantiateSceneObject(styles[2], new Vector3(transform.position.x - (float)0.6, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0)).gameObject;
            cosmetic.GetComponent<PhotonView>().RPC("SetParentObject", RpcTarget.AllBuffered, gameObject.GetComponent<PhotonView>().ViewID);
            foreach (Transform c in transform)
            {
                if (c.tag == "Cosmetic" && c.name != "PickAxe(Clone)") { Destroy(c.gameObject); }
            }
        }
    }
    public char GetPieceType() { return pieceType; } //Returns the piece's type
    [PunRPC]
    public void MovePiece(int targetTileID, int playerOneID, int playerTwoID, int gameManagerID) //moves the piece, checks validity, whilist checking if the player has enough fuel and deducting the cost whilst decrementing the turns taken
    {
        GameManager gameManager = PhotonView.Find(gameManagerID).GetComponent<GameManager>();
        BoardPlayer playerTwo = PhotonView.Find(playerTwoID).GetComponent<BoardPlayer>();
        BoardPlayer playerOne = PhotonView.Find(playerOneID).GetComponent<BoardPlayer>();
        GameObject targetTile = PhotonView.Find(targetTileID).gameObject; 
        bool validMove = CheckMoveIsValid(targetTile);
        BoardPlayer p;
        if (belongsToPlayerOne)
        {
            p = playerOne;
        }
        else
        {
            p = playerTwo;
        }
        if (validMove)
        {
            int g = CheckMovePrice();
            if ((p.GetFuel() - g) >= 0)
            {
                MoveToNewTile(targetTile);
                p.RemoveFuel(g);
                gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    public void UpgradePiece(string pS, int playerOneID, int playerTwoID, int gameManagerID) //Checks if the upgrade is valid then deducts the cost and upgrades the piece
    {
        char p = char.Parse(pS);
        GameManager gameManager = PhotonView.Find(gameManagerID).GetComponent<GameManager>();
        BoardPlayer playerTwo = PhotonView.Find(playerTwoID).GetComponent<BoardPlayer>();
        BoardPlayer playerOne = PhotonView.Find(playerOneID).GetComponent<BoardPlayer>();
        BoardPlayer pl;
        if (belongsToPlayerOne)
        {
            pl = playerOne;
        }
        else
        {
            pl = playerTwo;
        }
        if (CheckHasEnoughToUpgrade(pl))
        {
            if (p == 'P')
            {
                this.GetComponent<PhotonView>().RPC("SetPieceType", RpcTarget.AllBuffered, p.ToString());
                pl.RemoveLumber(upgradeCost);
                gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered);
            }
            else if (p == 'L')
            {
                this.GetComponent<PhotonView>().RPC("SetPieceType", RpcTarget.AllBuffered, p.ToString());
                pl.RemoveLumber(upgradeCost);
                gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    public void PerformPieceAbility(int playerOneID, int playerTwoID, int gameManagerID) //Checks the validity of the ability then performs the ability deducting a turn
    {
        GameManager gameManager = PhotonView.Find(gameManagerID).GetComponent<GameManager>();
        BoardPlayer playerTwo = PhotonView.Find(playerTwoID).GetComponent<BoardPlayer>();
        BoardPlayer playerOne = PhotonView.Find(playerOneID).GetComponent<BoardPlayer>();
        BoardPlayer pl;
        if (belongsToPlayerOne)
        {
            pl = playerOne;
        }
        else
        {
            pl = playerTwo;
        }
        int moveValid = CheckAbilityValidity();
        if (moveValid == 1) //Chop
        {
            int lumberToRemove = pl.GetLumber() + chopAmount;
            pl.SetLumber(lumberToRemove);
            if (PhotonNetwork.IsMasterClient)
            {
                gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered); //happening twice for some reason
            }
        }
        else if(moveValid == 2) //Dig
        {
            int fuelToRemove = pl.GetFuel() + digAmount;
            pl.SetFuel(fuelToRemove);
            if(PhotonNetwork.IsMasterClient)
            {
                gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered); //happening twice for some reason
            }
        }
    }
    public int CheckNeighoursToDestroyPiece() //Checks neighbouring tiles to see if this piece needs to be destroyed; returns the VP value of the piece 
    {
        List<GameObject> n = new List<GameObject>();
        if (tilePieceIsOn != null)
        {
            n = tilePieceIsOn.GetNeighbours();
        }
        List<Tile> t = new List<Tile>();
        foreach (var q in n) //Gets the tile script for all neighbours into a list
        {
            t.Add(q.GetComponent<Tile>());
        }
        int neighboursOccupied = 0;
        foreach (var q in t)
        {
            if (q.GetIsOccupied())
            {
                neighboursOccupied += 1;
            }
        }
        if (neighboursOccupied >= 2)
        {
            if (pieceType == 'B') { return 10; }
            else if (pieceType == 'S') { return 1; }
            else if (pieceType == 'L') { return 3; }
            else if (pieceType == 'P') { return 2; }
            else { return -1; }
        }
        else
        {
            return -1;
        }
    }
    [PunRPC]
    public void SpawnNewPiece(int targetTileID, int playerOneID, int playerTwoID, string piecePrefab, int gameManagerID) //If everything is valid then a new piece is instantiated
    {
        GameManager gameManager = PhotonView.Find(gameManagerID).GetComponent<GameManager>();
        BoardPlayer playerTwo = PhotonView.Find(playerTwoID).GetComponent<BoardPlayer>();
        BoardPlayer playerOne = PhotonView.Find(playerOneID).GetComponent<BoardPlayer>();
        GameObject targetTile = PhotonView.Find(targetTileID).gameObject;
        BoardPlayer pl;
        if (belongsToPlayerOne)
        {
            pl = playerOne;
        }
        else
        {
            pl = playerTwo;
        }
        if (pieceType == 'B')
        {
            bool validTargetTile = false;
            foreach (var n in tilePieceIsOn.GetNeighbours())
            {
                if (n.name == targetTile.name && !targetTile.GetComponent<Tile>().GetIsOccupied())
                {
                    validTargetTile = true;
                }
            }
            if (validTargetTile)
            {
                if ((pl.GetLumber() - spawnCost) >= 0)
                {
                    pl.RemoveLumber(spawnCost);
                    GameObject x = PhotonNetwork.InstantiateSceneObject(piecePrefab, targetTile.GetComponent<Tile>().GetLocationForPiece(), Quaternion.identity);
                    x.GetComponent<Piece>().GetComponent<PhotonView>().RPC("SetOwnedByPlayerOne", RpcTarget.AllBuffered, belongsToPlayerOne);
                    x.GetComponent<Piece>().GetComponent<PhotonView>().RPC("SetPieceType", RpcTarget.AllBuffered, "S");
                    gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered);
                }
            }
        }
    }

    private bool CheckHasEnoughToUpgrade(BoardPlayer pl) //check if the player has enough to upgrade a piece returning true if they do
    {
        if((pl.GetLumber() - upgradeCost) < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void OnTriggerEnter(Collider other) //Detects when the piece moves tile and sets the tiles occupication to true
    {
        if(other.tag == "Tile")
        {
            Tile t = other.GetComponent<Tile>();
            tilePieceIsOn = t;
            t.SetIsOccupied(true, this.gameObject);
        }
    }
    private void OnTriggerExit(Collider other) //Detects when the piece moves tile and sets the prior tiles occupation to false whilist setting this pieces tilePieceIsOn to null
    {
        if (other.tag == "Tile")
        {
            Tile t = other.GetComponent<Tile>();
            t.SetIsOccupied(false , gameObject);
            tilePieceIsOn = null;
        }
    }
    private void MoveToNewTile(GameObject targetObject) //Move the piece to the desired location whilist setting the occupation of the prior tile to false and the new tile to true
    {
        tilePieceIsOn.SetIsOccupied(false, this.gameObject);
        tilePieceIsOn = targetObject.GetComponent<Tile>();
        transform.position = tilePieceIsOn.GetLocationForPiece();
        tilePieceIsOn.SetIsOccupied(true, this.gameObject);
    }
    private bool CheckMoveIsValid(GameObject targetObject) //Checks if this piece can move to the targeted tile returning true if the move is valid
    {
        bool validMove = false;
        foreach (var n in tilePieceIsOn.GetNeighbours())
        {
            if (n == targetObject)
            {
                validMove = true;
            }
        }

        if(!targetObject.GetComponent<Tile>().GetIsOccupied() && validMove)
        {
            if (pieceType == 'B')
            {
                return true;
            }
            else if (pieceType == 'S')
            {
                return true;
            }
            else if (pieceType == 'L')
            {
                if (tilePieceIsOn.GetTerrain() == '#')
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (pieceType == 'P')
            {
                if (tilePieceIsOn.GetTerrain() == '~')
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        else
        {
            return false;
        }
        return false;
    }
    private int CheckMovePrice() //Checks the price of the move returning the fuel price
    {
        if (pieceType == 'B') { return 1; }
        else if (pieceType == 'L' || pieceType == 'S')
        {
            if (tilePieceIsOn.GetTerrain() == '~')
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else if (pieceType == 'P') { return 2; }
        return 0;
    }
    private int CheckAbilityValidity() //returns 1 if chop is valid, 2 if dig is valid or -1 if not valid
    {
        if(tilePieceIsOn.GetTerrain() == '#' && pieceType == 'L')
        {
            return 1;
        }
        else if(tilePieceIsOn.GetTerrain() == '~' && pieceType == 'P')
        {
            return 2;
        }
        return -1;
    }

}
