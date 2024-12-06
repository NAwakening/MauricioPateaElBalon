using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    string m_characterChosen;
    bool m_redTeam;
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out object character))
        {
            Debug.Log("entro");
            m_characterChosen = character.ToString();
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("turn", out object team))
            {
                if (team.ToString() == "true")
                {
                    m_redTeam = true;
                }
                else
                {
                    m_redTeam = false;
                }
            }

            if (!m_redTeam)
            {
                Hashtable m_teamSelected = new Hashtable();
                m_teamSelected["Team"] = "blue";
                PhotonNetwork.LocalPlayer.SetCustomProperties(m_teamSelected);
                switch (m_characterChosen)
                {
                    case "Red":
                        PhotonNetwork.Instantiate("PlayerRed", new Vector3(-14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Green":
                        PhotonNetwork.Instantiate("PlayerGreen", new Vector3(-14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Blue":
                        PhotonNetwork.Instantiate("PlayerBlue", new Vector3(-14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Yellow":
                        PhotonNetwork.Instantiate("PlayerYellow", new Vector3(-14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                }
                m_teamSelected["turn"] = "true";
            }
            else
            {
                Hashtable m_teamSelected = new Hashtable();
                m_teamSelected["Team"] = "red";
                PhotonNetwork.LocalPlayer.SetCustomProperties(m_teamSelected);
                switch (m_characterChosen)
                {
                    case "Red":
                        PhotonNetwork.Instantiate("PlayerRed", new Vector3(14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Green":
                        PhotonNetwork.Instantiate("PlayerGreen", new Vector3(14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Blue":
                        PhotonNetwork.Instantiate("PlayerBlue", new Vector3(14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                    case "Yellow":
                        PhotonNetwork.Instantiate("PlayerYellow", new Vector3(14, transform.position.y, Random.Range(-15, 15)), Quaternion.identity);
                        break;
                }
                m_teamSelected["turn"] = "false";
            }

        }
    }
}
