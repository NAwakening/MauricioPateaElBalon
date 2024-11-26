using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using TMPro;

public class PhotinConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField m_RoomName;
    [SerializeField] TextMeshProUGUI m_errorText;
    [SerializeField] TMP_InputField m_Nickname;
    [SerializeField] GameObject m_loadingScreen, m_normalScreen;
    private bool m_hasChosenACharacter;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("se ha conectado al master");
        m_loadingScreen.SetActive(false);
        m_normalScreen.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Se ha entrado al lobby Abstracto");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("se entro al room");
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogWarning("Hubo un erro al crear un room: " + message);
        m_errorText.text = "Hubo un error al crear el room " + m_RoomName.text;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogWarning("Hubo un erro al entrar: " + message);
        m_errorText.text = "Hubo un erro al entrar al room " + m_RoomName.text;
    }

    RoomOptions NewRoomInfo()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public void JoinRoom()
    {
        if (m_RoomName.text == "")
        {
            Debug.LogWarning("Tiene que dar un nombre al cuarto primero");
            m_errorText.text = "Tiene que dar un nombre al cuarto primero";
            return;
        }
        if (m_Nickname.text == "")
        {
            m_errorText.text = "Tienes que poner un nickname primero";
            return;
        }
        if (!m_hasChosenACharacter)
        {
            m_errorText.text = "Tienes que escoger un personaje primero";
            return;
        }
        PhotonNetwork.NickName = m_Nickname.text;
        PhotonNetwork.JoinRoom(m_RoomName.text);
    }

    public void CreateRoom()
    {
        if (m_RoomName.text == "")
        {
            Debug.LogWarning("Tiene que dar un nombre al cuarto primero");
            m_errorText.text = "Tiene que dar un nombre al cuarto primero";
            return;
        }
        if (m_Nickname.text == "")
        {
            m_errorText.text = "Tienes que poner un nickname primero";
            return;
        }
        if (!m_hasChosenACharacter)
        {
            m_errorText.text = "Tienes que escoger un personaje primero";
            return;
        }
        PhotonNetwork.NickName = m_Nickname.text;
        PhotonNetwork.CreateRoom(m_RoomName.text, NewRoomInfo());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SelectCharacter(string id)
    {
        Hashtable m_characterSelected = new Hashtable();
        m_characterSelected["character"] = id;
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_characterSelected);
        m_hasChosenACharacter = true;
    }
}
