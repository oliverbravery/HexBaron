using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public class UIManager : MonoBehaviourPun
{
    [Header("Player One Elements")]
    [SerializeField] private Text txtPlayerOneLumber;
    [SerializeField] private Text txtPlayerOneFuel;
    [SerializeField] private Text txtPlayerOneVictoryPoints;
    [SerializeField] private Text txtUsrNameP1;
    [Header("Player Two Elements")]
    [SerializeField] private Text txtPlayerTwoLumber;
    [SerializeField] private Text txtPlayerTwoFuel;
    [SerializeField] private Text txtPlayerTwoVictoryPoints;
    [SerializeField] private Text txtUsrNameP2;
    [Header("UI Pannels")]
    [SerializeField] private GameObject pnlPieceMenu;
    [SerializeField] private GameObject pnlPieceAbilityMenu;
    [SerializeField] private GameObject pnlUpgradeMenu;
    [SerializeField] private GameObject pnlErrorMessage;
    [SerializeField] private GameObject pnlVictoryMessage;
    [SerializeField] private GameObject pnlSettings;
    [Header("Sub-Pannel Elements")]
    [SerializeField] private Text txtVictory;
    [SerializeField] private GameObject btnAbilityPBDS;
    [SerializeField] private GameObject btnAbilityLESS;
    [SerializeField] private GameObject btnAbilitySpawn;
    [Header("Turn Bars")]
    [SerializeField] private Image imgPlayerOneTurns;
    [SerializeField] private Image imgPlayerTwoTurns;
    [Header("Other GameObjects")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject btnSkipTurn;
    private BoardPlayer playerOne;
    private BoardPlayer playerTwo;
    private Tile lastTileSelected;
    private float errorMessageTimer = 0;
    private float errorMessageDisplayTime = 3.2f;
    private string p1Name;
    private string p2Name;

    public void OnCogPress()
    {
        pnlSettings.SetActive(true);
    }
    public void OnLeaveGamePress()
    {
        gameManager.photonView.RPC("LeaveTheRoom", RpcTarget.AllBuffered);
        pnlSettings.SetActive(false);
    }
    public void OnExitMenuPress()
    {
        pnlSettings.SetActive(false);
    }

    public void ToggleOnPieceMenuPannel(Vector3 locationOfPannel, char pieceType) //Toggles the piece menu pannel to either on whilist turning on the according/ appropriate ability buttons
    {
        pnlPieceMenu.transform.position = locationOfPannel;
        if(lastTileSelected.GetPieceInTile() != null)
        {
            pnlPieceMenu.SetActive(true);
            if (pnlPieceMenu.activeSelf) //if the piece menu is being toggled on //if menu is being toggled open
            {
                if (pieceType == 'L' || pieceType == 'P' || pieceType == 'B') //open the ability menu and set the according ability button to active
                {
                    pnlPieceAbilityMenu.SetActive(true);
                    if (pieceType == 'L')
                    {
                        btnAbilityLESS.SetActive(true);
                    }
                    else if (pieceType == 'P')
                    {
                        btnAbilityPBDS.SetActive(true);
                    }
                    else if(pieceType == 'B')
                    {
                        btnAbilitySpawn.SetActive(true);
                    }
                }
                else
                {
                    pnlPieceAbilityMenu.SetActive(false);
                    btnAbilityLESS.SetActive(false);
                    btnAbilityPBDS.SetActive(false);
                    btnAbilitySpawn.SetActive(false);
                }
            }
        }

    }
    public void ToggleOnPieceMenuPannel(Vector3 locationOfPannel, char pieceType, Tile tile) //Toggles the piece menu pannel to either on whilist taking the param of last tile
    {
        lastTileSelected = tile;
        pnlPieceMenu.transform.position = locationOfPannel;
        if(lastTileSelected.GetPieceInTile() != null)
        {
            pnlPieceMenu.SetActive(true);
            if (pnlPieceMenu.activeSelf) //if the piece menu is being toggled on //if menu is being toggled open
            {
                if (pieceType == 'L' || pieceType == 'P' || pieceType == 'B') //open the ability menu and set the according ability button to active
                {
                    pnlPieceAbilityMenu.SetActive(true);
                    if (pieceType == 'L')
                    {
                        btnAbilityLESS.SetActive(true);
                    }
                    else if (pieceType == 'P')
                    {
                        btnAbilityPBDS.SetActive(true);
                    }
                    else if(pieceType == 'B')
                    {
                        btnAbilitySpawn.SetActive(true);
                    }
                }
            }
            else //if the piece menu is being toggled off
            {
                btnAbilityPBDS.SetActive(false);
                btnAbilityLESS.SetActive(false);
                btnAbilitySpawn.SetActive(false);
                pnlPieceAbilityMenu.SetActive(false);
            }
        }

    }
    public void ToggleOffPieceMenuPannel() //Toggles the piece menu pannel off
    {
        btnAbilityPBDS.SetActive(false);
        btnAbilityLESS.SetActive(false);
        btnAbilitySpawn.SetActive(false);
        pnlPieceAbilityMenu.SetActive(false);
    }
    public void ToggleOnUpgradeMenuPannel(char pieceType) //Turns on the upgrade menu ONLY if the piece is a surf
    {
        if(pieceType == 'S')
        {
            pnlUpgradeMenu.transform.position = pnlPieceMenu.transform.position;
            ToggleOffPieceMenuPannel();
            pnlUpgradeMenu.SetActive(true);
        }
        //else error
        DisplayErrorMessage();
    }
    public void ToggleOffUpgradeMenuPannel() //Sets the upgrade menu to off
    {
        pnlUpgradeMenu.SetActive(false);
    }
    public void ToggleOffAllMenuPannels() //Toggles every pannel menu off
    {
        pnlPieceMenu.SetActive(false);
        pnlPieceAbilityMenu.SetActive(false);
        pnlUpgradeMenu.SetActive(false);
        btnAbilityLESS.SetActive(false);
        btnAbilityPBDS.SetActive(false);
        btnAbilitySpawn.SetActive(false);
    }
    public Tile GetLastTileSelected() { return lastTileSelected; } //returns the last tile that was passed in as an argument
    public bool GetIsPieceMenuPannelOpen() { return pnlPieceMenu.activeSelf; } //returns the active state of the piece menu
    public bool GetIsMenuPannelOpen()//returns true if a pannel menu is open
    {
        if (pnlUpgradeMenu.activeSelf || pnlPieceMenu.activeSelf || pnlPieceAbilityMenu.activeSelf)
        { 
            return true; 
        } 
        return false; 
    } 
    public void DisplayErrorMessage() //Restarts the timer of which keeps the error message on the screen
    {
        errorMessageTimer = errorMessageDisplayTime;
    } 
    public void BtnSkipTurnPressed()
    {
        int turnsToTake = gameManager.GetTurns();
        for (int i = 0; i < Math.Abs(turnsToTake); i++)
        {
            gameManager.photonView.RPC("DecrementTurns", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    public void DisplayVictoryMessage(string victoryMessage)
    {
        string localVictoryMessage = victoryMessage;
        txtVictory.text = localVictoryMessage;
        pnlVictoryMessage.SetActive(true);
    }

    [PunRPC]
    public void SetTextForPlayers(string p1F, string p2F, string p1L, string p2L, string p1V, string p2V, int x, bool isP1T, string p1N, string p2N)
    {
        txtPlayerOneFuel.text = p1F;
        txtPlayerOneLumber.text = p1L;
        txtPlayerOneVictoryPoints.text = p1V;
        txtPlayerTwoFuel.text = p2F;
        txtPlayerTwoLumber.text = p2L;
        txtPlayerTwoVictoryPoints.text = p2V;
        txtUsrNameP1.text = p1N;
        txtUsrNameP2.text = p2N;
        double y = 0;
        if (x == 3) { y = 1; }
        else if (x == 2) { y = 0.6; }
        else if (x == 1) { y = 0.3; }
        else if (x == 0) { y = 0; }
        if (isP1T)
        {
            imgPlayerOneTurns.fillAmount = (float)y;
        }
        else
        {
            imgPlayerTwoTurns.fillAmount = (float)y;
        }
    }


    private void Start() //Calls the initalise variables method
    {
        InitaliseVariables();
    }
    private void InitaliseVariables() //Sets the player variables from the gameManager player GET methods
    {
        playerOne = gameManager.GetPlayerOne();
        playerTwo = gameManager.GetPlayerTwo();
    }
    private void UpdatePlayerUIPannels() //Assign the value of the players fuel, vp and lumber to the UI statistics and both player turn sliders
    {
        int x = gameManager.GetNumberOfTurns();
        GetComponent<PhotonView>().RPC("SetTextForPlayers", RpcTarget.AllBuffered, playerOne.GetFuel().ToString(), playerTwo.GetFuel().ToString(), 
            playerOne.GetLumber().ToString(), playerTwo.GetLumber().ToString(), playerOne.GetVictoryPoints().ToString(),
            playerTwo.GetVictoryPoints().ToString(), x, gameManager.GetIsPlayerOnesTurn(), playerOne.GetPlayerName(), playerTwo.GetPlayerName());

    }
    private void UpdateErrorMessageTimer() //Decrements the timer for error message alongside displaying and hiding the error message panel
    {
        if(errorMessageTimer > 0)
        {
            pnlErrorMessage.SetActive(true);
            errorMessageTimer -= Time.deltaTime;
        }
        else
        {
            if(pnlErrorMessage.activeSelf)
            {
                pnlErrorMessage.SetActive(false);
            }
        }
    }
    private void Update() //Every frame call the UpdatePlayerUIPannels method to update the players statistics
    {
        if(PhotonNetwork.IsMasterClient)
        {
            UpdatePlayerUIPannels();
        }
        UpdateErrorMessageTimer();
        ShowElementsOnGo();
    }
    private void ShowElementsOnGo()
    {
        if (gameManager.GetIsPlayerOnesTurn() == (bool)PhotonNetwork.LocalPlayer.CustomProperties["isPlayerOne"])
        {
            btnSkipTurn.SetActive(true);
        }
        else
        {
            btnSkipTurn.SetActive(false);
        }
    }
}
