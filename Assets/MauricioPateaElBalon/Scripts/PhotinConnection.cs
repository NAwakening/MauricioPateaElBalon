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
    [SerializeField] Slider m_time;
    [SerializeField] TextMeshProUGUI m_timeText;
    private bool m_hasChosenACharacter;
    private float m_matchTime;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        m_RoomItemsList = new List<RoomItem>();
    }

    void Update()
    {
        m_matchTime = m_time.value * 10;
        m_timeText.text = "Match Time: " + m_matchTime.ToString();
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
        m_errorText.text = "There was an error when creating the room " + m_RoomName.text;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogWarning("Hubo un erro al entrar: " + message);
        m_errorText.text = "There was an error when entering the room " + m_RoomName.text;
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
            m_errorText.text = "You must choose a Nickname first";
            return;
        }
        if (!m_hasChosenACharacter)
        {
            m_errorText.text = "You must choose a Character first";
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
            m_errorText.text = "You must give the room a name";
            return;
        }
        if (m_Number.text == "")
        {
            m_errorText.text = "You must specify the number of player";
            return;
        }
        else
        {
            if (int.Parse(m_Number.text) % 2 == 1)
            {
                m_errorText.text = "The number of players must be odd";
                return;
            }
            else
            {
                if (int.Parse(m_Number.text) < 2 || int.Parse(m_Number.text) > 10)
                {
                    m_errorText.text = "The numbre of player must be between 2 and 10";
                    return;
                }
            }
        }
        if (m_Nickname.text == "")
        {
            m_errorText.text = "You must choose a Nickname first";
            return;
        }
        if (!m_hasChosenACharacter)
        {
            m_errorText.text = "You must choose a Character first";
            return;
        }
        PhotonNetwork.NickName = m_Nickname.text;
        Hashtable playtime = new Hashtable();
        playtime["time"] = m_matchTime;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playtime);
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
                m_newButtonRoom.SetNewRoomName(roomInfo.Name, " : " + roomInfo.PlayerCount + " of " + roomInfo.MaxPlayers);
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
