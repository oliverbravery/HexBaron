using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildToParent : MonoBehaviourPun
{
    [SerializeField] private GameObject parentTileLocation;
    [PunRPC]
    public void SetParentObject(int photonID)
    {
        GameObject parent = PhotonView.Find(photonID).gameObject;
        transform.parent = parent.transform;
    }
    [PunRPC]
    public void DestroyCosmetic()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
