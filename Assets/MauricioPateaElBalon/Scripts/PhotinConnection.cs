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
    [SerializeField] TMP_InputField m_Number;
    [SerializeField] RoomItem m_RoomItemButton;
    List<RoomItem> m_RoomItemsList;
    [SerializeField] Transform m_contentObject;
    [SerializeField] GameObject m_loadingScreen, m_normalScreen;
    private bool m_hasChosenACharacter;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        m_RoomItemsList = new List<RoomItem>();
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

    RoomOptions NewRoomInfo(int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;
    }

    public void JoinRoom(string p_roomName)
    {
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
        PhotonNetwork.JoinRoom(p_roomName);
    }

    public void CreateRoom()
    {
        if (m_RoomName.text == "")
        {
            Debug.LogWarning("Tiene que dar un nombre al cuarto primero");
            m_errorText.text = "Tiene que dar un nombre al cuarto primero";
            return;
        }
        if (m_Number.text == "")
        {
            m_errorText.text = "Tiene que especificar el número de jugadores";
            return;
        }
        else
        {
            if (int.Parse(m_Number.text) % 2 == 1)
            {
                m_errorText.text = "El número de jugadores tiene que ser par";
                return;
            }
            else
            {
                if (int.Parse(m_Number.text) < 2 || int.Parse(m_Number.text) > 10)
                {
                    m_errorText.text = "El número de jugadores tiene que ser como minimo 2 y como maximo 10";
                    return;
                }
            }
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
        PhotonNetwork.CreateRoom(m_RoomName.text, NewRoomInfo(int.Parse(m_Number.text)));
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomItem roomItem in m_RoomItemsList)
        {
            Destroy(roomItem.gameObject);
        }
        m_RoomItemsList.Clear();

        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.IsOpen)
            {
                RoomItem m_newButtonRoom = Instantiate(m_RoomItemButton, m_contentObject);
                m_newButtonRoom.SetNewRoomName(roomInfo.Name, " | " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers);
                m_RoomItemsList.Add(m_newButtonRoom);
            }
        }
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
