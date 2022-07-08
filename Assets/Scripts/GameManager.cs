using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string piecePrefab = "Piece";
    [SerializeField] private BoardPlayer playerOne;
    [SerializeField] private BoardPlayer playerTwo;
    [SerializeField] private GameObject grid;
    [SerializeField] private UIManager uiManager;
    private List<BoardPlayer> playerList = new List<BoardPlayer>();
    private List<GameObject> tilesList = new List<GameObject>();
    private int defaultFuel = 10;
    private int defaultLumber = 10;
    private int defaultVictoryPoints = 0;
    private int defaultNumberOfTurns = 3;
    private string[] defaultPieceLayout = new string[] { "1,B,1","1,S,9","2,B,32","2,S,24"};
    //Gamelooping variables
    private int turnsTaken;
    private bool isPlayerOnesTurn = true;
    private bool gameInProgress = true;
    private bool moveButtonPressed = false;
    private bool spawnButtonPressed = false;
    private Tile TileInQuestion;
    private int priorTurnNumber = 0;
    private float destroyPiecesCountDown = 0;
    private float destroyPiecesDefualtTime = 0.15f;
    private float transferSceneCountdown = 0;
    private float transferSceneDefualtTime = 5.4f;
    private bool canCheckForNeighbours = false;
    private bool gameHasBeenEnded = false;
    private bool transferScene = false;

    //public button functions
    public void MoveButtonPressed() //Method used by buttons to close all pannels and declare a prior tile whilist enabling moveButtonPressed
    {
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion = uiManager.GetLastTileSelected();
        GetComponent<PhotonView>().RPC("SetIsMoveButtonPressed", RpcTarget.AllBuffered, true);
    }
    public void UpgradeMenuButtonPressed() //Dependant on the piece type, toggles the upgrade button 
    {
        Tile temp = uiManager.GetLastTileSelected();
        uiManager.ToggleOffPieceMenuPannel();
        uiManager.ToggleOnUpgradeMenuPannel(temp.GetPieceInTile().GetPieceType());
    }
    public void DigButtonPressed() //Performs the dig action
    {
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion.GetPieceInTile().GetComponent<PhotonView>().RPC("PerformPieceAbility", RpcTarget.AllBuffered, p1ID, p2ID, gmID);
    }
    public void ChopButtonPressed() //Performs the chop action
    {
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion.GetPieceInTile().GetComponent<PhotonView>().RPC("PerformPieceAbility", RpcTarget.AllBuffered, p1ID, p2ID, gmID);
    }
    public void UpgradeToLESSButtonPressed() //Upgrades the piece inside of the tile to a LESS
    {
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion.GetPieceInTile().GetComponent<PhotonView>().RPC("UpgradePiece", RpcTarget.AllBuffered, "L", p1ID, p2ID, gmID);
    }
    public void UpgradeToPBDSButtonPressed()//Upgrades the piece inside of the tile to a PBDS
    {
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion.GetPieceInTile().GetComponent<PhotonView>().RPC("UpgradePiece", RpcTarget.AllBuffered, "P", p1ID, p2ID, gmID);
    }
    public void SpawnNewSurfButtonPressed() //Method used by buttons to close all pannels and declare a prior tile whilist enabling spawnButtonPressed
    {
        uiManager.ToggleOffAllMenuPannels();
        TileInQuestion = uiManager.GetLastTileSelected();
        GetComponent<PhotonView>().RPC("SetIsSpawnButtonPressed", RpcTarget.AllBuffered, true);
    }
    public void MovePiece(Tile target) //Used by tileCollider to take the input of a target tile and move a piece
    {
        int targetID = target.GetComponent<PhotonView>().ViewID;
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        GetComponent<PhotonView>().RPC("SetIsMoveButtonPressed", RpcTarget.AllBuffered, false);
        TileInQuestion.GetPieceInTile().GetComponent<PhotonView>().RPC("MovePiece", RpcTarget.AllBuffered, targetID, p1ID, p2ID, gmID);
    }
    public void SpawnPiece(Tile target) //Used by tileCollider to take the input of target tile and spawn a new piece dependant on if the tile is valid
    {
        GetComponent<PhotonView>().RPC("SetIsSpawnButtonPressed", RpcTarget.AllBuffered, false);
        int targetID = target.GetComponent<PhotonView>().ViewID;
        int p1ID = playerOne.GetComponent<PhotonView>().ViewID;
        int p2ID = playerTwo.GetComponent<PhotonView>().ViewID;
        int gmID = this.GetComponent<PhotonView>().ViewID;
        TileInQuestion.GetPieceInTile().photonView.RPC("SpawnNewPiece", RpcTarget.AllBuffered, targetID, p1ID, p2ID, piecePrefab, gmID);
        //Else error
    }
    public BoardPlayer GetPlayerOne() { return playerOne; } //returns playerOne
    public BoardPlayer GetPlayerTwo() { return playerTwo; } //returns playerTwo
    public void DecrementTurnsTaken() { turnsTaken -= 1; } //Increment the amount of turns taken
    public bool GetIsPlayerOnesTurn() { return isPlayerOnesTurn; } //Returns whether it is player ones turn
    public bool GetIsMoveButtonPressed() { return moveButtonPressed; } //Returns whether the move button has been pressed (used by the UIManager)
    public bool GetIsSpawnButtonPressed() { return spawnButtonPressed; } //Returns whether the spawn button has been pressed (used by the UIManager)
    public void SetTileInQuestion(Tile t) { TileInQuestion = t; } //Allows outside methods (TileColldier) to send this script the tile when pressed
    public int GetNumberOfTurns() { return turnsTaken; } //Returns the number of turns taken
    public int GetMaxNumberOfTurns() { return defaultNumberOfTurns; } //Returns the maximum number of turns
    [PunRPC]
    public void SetIsPlayerOnesTurn(bool t) { isPlayerOnesTurn = t; } //Sets the value of player Ones turn to the passed value
    [PunRPC]
    public void SetIsMoveButtonPressed(bool m) { moveButtonPressed = m; } //Sets the value of the move button pressed to the passed value
    [PunRPC]
    public void SetIsSpawnButtonPressed(bool s) { spawnButtonPressed = s; } //Sets the value of whether the spawn button was pressed by the value
    [PunRPC]
    public void DecrementTurns() { turnsTaken -= 1; } //Decrements the turns taken by one when called
    public int GetTurns() { return turnsTaken; }
    public int GetDefualtNumberOfTurns() { return defaultNumberOfTurns; }
    [PunRPC]
    public void SetGameInProgress(bool s) { gameInProgress = s; }
    [PunRPC]
    public void EndTheGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            gameHasBeenEnded = true;
            string winner = null;
            string winnerMsg = "";
            if (playerOne.GetVictoryPoints() > playerTwo.GetVictoryPoints()) { winner = playerOne.GetPlayerName(); }
            else if (playerTwo.GetVictoryPoints() > playerOne.GetVictoryPoints()) { winner = playerTwo.GetPlayerName(); }
            else if (playerOne.GetVictoryPoints() == playerTwo.GetVictoryPoints()) { winner = null; }
            if (winner == null) { winnerMsg = $"Draw!"; }
            else { winnerMsg = $"{winner} won the game!"; }
            uiManager.photonView.RPC("DisplayVictoryMessage", RpcTarget.AllBuffered, winnerMsg);
            transferSceneCountdown = transferSceneDefualtTime;
            transferScene = true;
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left game");
        PhotonNetwork.LoadLevel("scnMenu");
    }
    [PunRPC]
    public void LeaveTheRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    private void Start() //Initalises all of the players, pieces and varibles
    {
        if (PhotonNetwork.IsMasterClient) { photonView.RPC("InitalisePlayerNames", RpcTarget.AllBuffered, 1, PhotonNetwork.NickName); }
        else { photonView.RPC("InitalisePlayerNames", RpcTarget.AllBuffered, 2, PhotonNetwork.NickName); }
        if (PhotonNetwork.IsMasterClient)
        {
            InitaliseVariables();
            photonView.RPC("InitalisePlayers", RpcTarget.AllBuffered);
            InitalisePieces();
        }
    }
    private void InitaliseVariables() //initalises the variable playerList and tilesList
    {
        foreach (Transform t in grid.transform)
        {
            tilesList.Add(t.gameObject);
        }
        turnsTaken = defaultNumberOfTurns;
        priorTurnNumber = turnsTaken;
        playerList.Add(playerOne);
        playerList.Add(playerTwo);
    }
    [PunRPC]
    private void InitalisePlayers() //sets all the players defualt values such as fuel and lumber
    {
        foreach (var p in playerList)
        {
            p.SetFuel(defaultFuel);
            p.SetLumber(defaultLumber);
            p.SetVictoryPoints(defaultVictoryPoints);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            playerOne.SetPlayerOwnerID(photonView.ViewID);
            Hashtable hash = new Hashtable();
            hash.Add("isPlayerOne", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        else
        {
            playerTwo.SetPlayerOwnerID(photonView.ViewID);
            Hashtable hash = new Hashtable();
            hash.Add("isPlayerOne", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    [PunRPC]
    private void InitalisePlayerNames(int playerNumber, string name)
    {
        if(playerNumber == 1)
        {
            playerOne.SetPlayerName(name);
        }
        else
        {
            playerTwo.SetPlayerName(name);
        }

    }
    private void InitalisePieces() //initalises all of the defualt pieces so that they go to their correct tiles with the correct varibles
    {
        bool belongsToPlayerOne = false;
        foreach (var p in defaultPieceLayout)
        {
            foreach (var t in tilesList)
            {
                if (t.name.Substring(1) == p.Substring(4)) 
                {
                    Tile tl = t.GetComponent<Tile>();
                    GameObject z = PhotonNetwork.InstantiateSceneObject(piecePrefab, tl.GetLocationForPiece(), new Quaternion(0,0,0,0));
                    Piece h = z.GetComponent<Piece>();
                    if(p.Substring(0, 1) == "1") { belongsToPlayerOne = true; }
                    else if(p.Substring(0,1) == "2") { belongsToPlayerOne = false; }
                    h.GetComponent<PhotonView>().RPC("SetOwnedByPlayerOne", RpcTarget.AllBuffered, belongsToPlayerOne);
                    h.GetComponent<PhotonView>().RPC("SetPieceType", RpcTarget.AllBuffered, p.Substring(2, 1));
                    //h.SetPieceType(char.Parse(p.Substring(2, 1)));
                }
            }
        }
    }
    [PunRPC]
    private void DestroyNeighbouringTiles() //checks every tile for a piece and subsequently every piece to see if it has more than one connection thus gifting the correct players vp's and destroying the pieces
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
            List<Piece> piecesToDestroy = new List<Piece>();
            bool notAllOnTile = false;
            foreach (var p in pieces)
            {
                if (p.GetComponent<Piece>().GetTilePieceIsOn() == null)
                {
                    notAllOnTile = true;
                }
            }
            if (notAllOnTile)
            {
                destroyPiecesCountDown = destroyPiecesDefualtTime;
                canCheckForNeighbours = true;
            }
            else if (!notAllOnTile)
            {
                canCheckForNeighbours = false;
                foreach (var t in tilesList)
                {
                    Tile tile = t.GetComponent<Tile>();
                    if (tile.GetIsOccupied())
                    {
                        int vpCost = tile.GetPieceInTile().CheckNeighoursToDestroyPiece();
                        if (vpCost != -1)
                        {
                            bool notAlreadyAdded = true;
                            foreach (var p in piecesToDestroy)
                            {
                                if (tile.GetPieceInTile() == p)
                                {
                                    notAlreadyAdded = false;
                                }
                            }
                            if (notAlreadyAdded)
                            {
                                piecesToDestroy.Add(tile.GetPieceInTile());
                                if (tile.GetPieceInTile().GetBelongsToPlayerOne())
                                {
                                    playerTwo.AddVictoryPoints(vpCost);
                                }
                                else
                                {
                                    playerOne.AddVictoryPoints(vpCost);
                                }
                            }

                        }
                    }
                }
                foreach (var p in piecesToDestroy)
                {
                    Debug.Log(p.photonView.ViewID);
                }
                foreach (var p in piecesToDestroy)
                {
                    Debug.Log($"Removing ID {p.photonView.ViewID}");
                    p.photonView.RPC("SetDestroyed", RpcTarget.AllBuffered, true);
                }
                piecesToDestroy.Clear();
                foreach (var p in piecesToDestroy)
                {
                    Debug.Log(p.photonView.ViewID);
                }
                gameObject.GetComponent<PhotonView>().RPC("CheckForGameOver", RpcTarget.AllBuffered);
            }
        }
    }
    [PunRPC]
    private void CheckForGameOver() //Checks if all tiles have been cleared for either player which would result in the end of the game
    {
        if(PhotonNetwork.IsMasterClient)
        {
            int p1Pieces = 0;
            int p2Pieces = 0;
            int p1PiecesUnlocked = 0;
            int p2PiecesUnlocked = 0;
            int p1PBDSLocked = 0;
            int p2PBDSLocked = 0;
            int upgradeCost = 0;
            int spawnCost = 0;
            foreach (var t in tilesList)
            {
                Tile x = t.GetComponent<Tile>();
                if (x.GetPieceInTile() != null)
                {
                    upgradeCost = x.GetPieceInTile().GetUpgradeCost();
                    spawnCost = x.GetPieceInTile().GetSpawnCost();
                    if (x.GetPieceInTile().GetBelongsToPlayerOne())
                    {
                        p1Pieces += 1;
                        if ((x.GetPieceInTile().GetPieceType() == 'P') && x.GetTerrain() == '~')
                        {
                            p1PBDSLocked += 1;
                        }
                        if(!((x.GetPieceInTile().GetPieceType() == 'L' && x.GetTerrain() == '#') || (x.GetPieceInTile().GetPieceType() == 'P' && x.GetTerrain() == '~')))
                        {
                            p1PiecesUnlocked += 1;
                        }
                    }
                    else
                    {
                        p2Pieces += 1;
                        if((x.GetPieceInTile().GetPieceType() == 'P') && x.GetTerrain() == '~')
                        {
                            p2PBDSLocked += 1;
                        }
                        if (!((x.GetPieceInTile().GetPieceType() == 'L' && x.GetTerrain() == '#') || (x.GetPieceInTile().GetPieceType() == 'P' && x.GetTerrain() == '~')))
                        {
                            p2PiecesUnlocked += 1;
                        }
                    }

                }
            }
            if (p1Pieces == 0 || p2Pieces == 0) //If there are no pieces for a player
            {
                gameObject.GetComponent<PhotonView>().RPC("SetGameInProgress", RpcTarget.AllBuffered, false);
            }
            else if((p1PiecesUnlocked == 0) || (p2PiecesUnlocked == 0)) //If there are no chacaters unlocked
            {
                gameObject.GetComponent<PhotonView>().RPC("SetGameInProgress", RpcTarget.AllBuffered, false);
            }
            else if((playerOne.GetFuel() == 0 && p1PBDSLocked == 0) || (playerTwo.GetFuel() == 0 && p2PBDSLocked == 0)) //IF no fuel and no PBDS
            {
                gameObject.GetComponent<PhotonView>().RPC("SetGameInProgress", RpcTarget.AllBuffered, false);
            }
        }
    }
    private void UpdateDestroyNeighbourCountDown()
    {
        if(destroyPiecesCountDown > 0)
        {
            destroyPiecesCountDown -= Time.deltaTime;
        }
    }
    private void UpdateEndGameCountDown()
    {
        if(transferScene)
        {
            if (transferSceneCountdown > 0 && transferScene)
            {
                transferSceneCountdown -= Time.deltaTime;
            }
            else if (transferSceneCountdown <= 0)
            {
                photonView.RPC("LeaveTheRoom", RpcTarget.AllBuffered);
            }
        }
    }
    private void GameLoop()
    {
        if (gameInProgress)
        {
            if (turnsTaken <= 0)
            {
                turnsTaken = defaultNumberOfTurns;
                this.GetComponent<PhotonView>().RPC("SetIsPlayerOnesTurn", RpcTarget.AllBuffered, !isPlayerOnesTurn);

            }
            if (priorTurnNumber != turnsTaken)
            {
                priorTurnNumber = turnsTaken;
                gameObject.GetComponent<PhotonView>().RPC("DestroyNeighbouringTiles", RpcTarget.AllBuffered); //SetUpTimer for stackOverflow handling
            }
        }
        else
        {
            if (!gameHasBeenEnded)
            {
                gameObject.GetComponent<PhotonView>().RPC("EndTheGame", RpcTarget.AllBuffered);
            }

        }
        UpdateDestroyNeighbourCountDown();
        if (destroyPiecesCountDown <= 0 && canCheckForNeighbours)
        {
            gameObject.GetComponent<PhotonView>().RPC("DestroyNeighbouringTiles", RpcTarget.AllBuffered);
        }
        UpdateEndGameCountDown();
    }
    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameLoop();
        }
    }
}
