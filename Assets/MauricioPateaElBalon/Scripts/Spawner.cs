using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    string m_characterChosen;
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out object character))
        {
            Debug.Log("entro");
            m_characterChosen = character.ToString();
            switch (m_characterChosen)
            {
                case "Red":
                    PhotonNetwork.Instantiate("PlayerRed", new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                    break;
                case "Green":
                    PhotonNetwork.Instantiate("PlayerGreen", new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                    break;
                case "Blue":
                    PhotonNetwork.Instantiate("PlayerBlue", new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                    break;
                case "Yellow":
                    PhotonNetwork.Instantiate("PlayerYellow", new Vector3(Random.Range(-15, 15), transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                    break;
            }
        }
    }
}
