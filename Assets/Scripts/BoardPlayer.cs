using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlayer : MonoBehaviourPun
{
    [SerializeField] private string playerName;
    [SerializeField] private bool isPlayerOne;
    [SerializeField] private int lumber, fuel, victoryPoints;
    [SerializeField] private int playerOwnerID;
    public void SetPlayerOwnerID(int i) { playerOwnerID = i; }
    public int GetPlayerOwnerID() { return playerOwnerID; }
    public string GetPlayerName() { return playerName; } //returns the name of this player
    public void SetPlayerName(string n) { playerName = n; } //sets the player's name
    public bool GetIsPlayerOne() { return isPlayerOne; } //returns whether this is player one
    public int GetFuel() { return fuel; } //returns the fuel
    public void SetFuel(int f) { fuel = f; } //sets the fuel
    public void AddFuel(int f) { fuel += f; } //adds fuel to the balance
    public bool RemoveFuel(int f) //Checks if removing fuel won't make the player bankrupt and removes the fuel returning a boolean
    { 
        if((fuel - f) < 0)
        {
            return false;
        }
        else
        {
            fuel -= f;
            return true;
        }
    }
    public int GetLumber() { return lumber; } //returns the lumber
    public void SetLumber(int l) { lumber = l; } //sets the lumber
    public void AddLumber(int l) { lumber += l; } //adds lumber to the balance
    public bool RemoveLumber(int l) //Checks if removing lumber won't make the player bankrupt and removes the fuel returning a boolean
    {
        if ((lumber - l) < 0)
        {
            return false;
        }
        else
        {
            lumber -= l;
            return true;
        }
    }
    public int GetVictoryPoints() { return victoryPoints; } //returns the victorypoints
    public void SetVictoryPoints(int v) { victoryPoints = v; } //sets the victorypoints
    public void AddVictoryPoints(int v) { victoryPoints += v; } //adds victorypoints
}
