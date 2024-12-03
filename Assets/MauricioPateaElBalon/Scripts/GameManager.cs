using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
       {
            PhotonNetwork.CurrentRoom.IsOpen = false; // probablemente el GameManager tenga que ser el masterclient
            PhotonNetwork.CurrentRoom.IsVisible = false;
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
