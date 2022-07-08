using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HexGrid : MonoBehaviourPun
{
    //Responsible for the generation and initalisation of terrain for ALL tiles
    [SerializeField] private string tilePrefab;
    [SerializeField] private GameObject tilePrefabNONEInstantiatable;
    private char[] levelTerrain;
    private List<GameObject> tiles = new List<GameObject>();
    private int gridWidth = 8, gridHeight = 4;
    private float tileSpacingX = 2.25f, tileSpacingZ = 1.2f, tileSpacingBetweenRows = 2.6f;

    private void Awake() //Runs all the functions on at the start of the game
    {
        InitaliseVaribles();
        CreateTiles();
        GenerateTerrain();
        InitaliseTerrain();
    }
    private void CreateTiles() //Instantiate all of the tiles into a hex grid formation
    {
        int incrementerForNames = 1;
        for (int h = 1; h < gridHeight + 1; h++)
        {
            incrementerForNames = CreateRowOfTiles(tileSpacingBetweenRows, h, incrementerForNames);
        }
    } 
    private int CreateRowOfTiles(float spacingZ, int h, int incrementer) //Instantiates a row of tiles returning the last tile number used in the nameing of the tiles
    {
        bool isUp = true;
        Vector3 positionOfTile = new Vector3(0, 0, spacingZ * h);
        for (int w = 1; w < gridWidth + 1; w++)
        {
            if (!isUp)
            {
                //create up tile
                isUp = !isUp;
                positionOfTile = new Vector3(positionOfTile.x + tileSpacingX, positionOfTile.y, positionOfTile.z + tileSpacingZ);
                GameObject x = PhotonNetwork.InstantiateSceneObject(tilePrefab, positionOfTile, tilePrefabNONEInstantiatable.transform.rotation);
                string name = $"T{incrementer}";
                x.GetComponent<PhotonView>().RPC("SetName", RpcTarget.AllBuffered, name);
                incrementer += 1;
                tiles.Add(x);
            }
            else
            {
                //create down tile
                isUp = !isUp;
                positionOfTile = new Vector3(positionOfTile.x + tileSpacingX, positionOfTile.y, positionOfTile.z - tileSpacingZ);
                GameObject x = PhotonNetwork.InstantiateSceneObject(tilePrefab, positionOfTile, tilePrefabNONEInstantiatable.transform.rotation);
                string name = $"T{incrementer}";
                x.GetComponent<PhotonView>().RPC("SetName", RpcTarget.AllBuffered, name);
                incrementer += 1;
                tiles.Add(x);
            }
        }
        return incrementer;
    }
    private void InitaliseVaribles() //Initalises any varibles
    {
        levelTerrain = new char[gridWidth * gridHeight];
    }
    private void GenerateTerrain() //randomly generate new terrain and save it to the levelTerrain variable
    {
        int maxAmountOfSpecialTiles = 10;
        int amountOfForests = 0;
        int amountOfBogs = 0;
        System.Random rnd = new System.Random();
        for (int i = 0; i < levelTerrain.Length; i++)
        {
            int r = rnd.Next(0, 9);
            if (r == 0 && amountOfBogs != maxAmountOfSpecialTiles) { levelTerrain[i] = '~'; amountOfBogs += 1; }
            else if (r == 1 && amountOfForests != maxAmountOfSpecialTiles) { levelTerrain[i] = '#'; amountOfForests += 1; }
            else { levelTerrain[i] = '^'; }
        }
        levelTerrain[0] = '^';
        levelTerrain[levelTerrain.Length - 1] = '^';

        string strLevelTerrain = "";
        foreach (var s in levelTerrain)
        {
            strLevelTerrain += s + ",";
        }
        Debug.Log(strLevelTerrain);
    }
    private void InitaliseTerrain() //go through every tile and initalise the terrain type dependant on the generated terrain
    {
        System.Random rd = new System.Random();
        int b;
        int counter = 0;
        foreach (var c in tiles)
        {
            b = rd.Next(1, 4);
            c.GetComponent<PhotonView>().RPC("SetTerrain", RpcTarget.AllBuffered, levelTerrain[counter].ToString(), b);
            counter += 1;
        }
    }
}
