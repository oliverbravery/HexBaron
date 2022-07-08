using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    [SerializeField] private GameObject pnlMenuOptions;
    [SerializeField] private GameObject pnlMasterConnectionOptions;
    [SerializeField] private GameObject pnlEnterCode;
    [Header("Buttons")]
    [SerializeField] private Button btnHostGame;
    [SerializeField] private Button btnJoinGame;
    [SerializeField] private Button btnConnectToMaster;
    [SerializeField] private Button btnInputField;
    [Header("Input Fields")]
    [SerializeField] private InputField iptCodeField;
    [SerializeField] private InputField iptUsername;
    [Header("Text")]
    [SerializeField] private Text txtErrorUsrName;
    private string versionName = "0.0.1";

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        pnlMasterConnectionOptions.SetActive(false);
        pnlMenuOptions.SetActive(true);
    }

    public void HostGame()
    {
        if(CheckUsernameValidity())
        {
            pnlMenuOptions.SetActive(false);
            pnlEnterCode.SetActive(true);
            txtErrorUsrName.gameObject.SetActive(false);
        }
        else
        {
            txtErrorUsrName.gameObject.SetActive(true);
        }
    }
    public void JoinGame()
    {
        if(CheckUsernameValidity())
        {
            pnlMenuOptions.SetActive(false);
            pnlEnterCode.SetActive(true);
            txtErrorUsrName.gameObject.SetActive(false);
        }
        else
        {
            txtErrorUsrName.gameObject.SetActive(true);
        }

    }
    
    public void JoinOrHostGame()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(iptCodeField.text, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LocalPlayer.NickName = iptUsername.text;
        PhotonNetwork.LoadLevel("scnGame");
    }

    private void Awake()
    {
        pnlMasterConnectionOptions.SetActive(true);
    }
    private bool CheckUsernameValidity()
    {
        string txtInputUsername = iptUsername.text;
        if (txtInputUsername != "")
        {
            return true;
        }
        return false;
    }

    

    //SetUp the scene instantiates and if the cosmetics and biome attributes like forests dont sync then add photon view to them
    //Also the prefabs need to go into the photon resources file and referances need to be strings
}
