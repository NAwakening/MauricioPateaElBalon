using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.Demo.Cockpit;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    #region References

    [SerializeField] TextMeshProUGUI m_timer;
    [SerializeField] TextMeshProUGUI m_endText;
    [SerializeField] TextMeshProUGUI m_redText;
    [SerializeField] TextMeshProUGUI m_blueText;
    [SerializeField] TextMeshProUGUI m_chat;
    [SerializeField] PhotonView m_pv;
    [SerializeField] Ball m_ball;
    [SerializeField] AudioSource m_audioSource;

    #endregion

    #region RuntimeVariables

    int m_firstTimer = 3;
    [SerializeField]int m_lastTimer = 5;
    int m_redScore = 0;
    int m_blueScore = 0;

    #endregion

    #region UnityMethods

    private void Awake()
    {
        if (instance != null && instance !=this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("time", out object time))
        {
            m_lastTimer = int.Parse(time.ToString());
        }
    }

    #endregion

    #region Local Methods

    protected void RaiseEndEvent()
    {
        EndGameEvent();
        Debug.Log("escogiendo ganador");
        m_audioSource.Play();
        if (m_redScore > m_blueScore)
        {
            m_pv.RPC("UpdateWinnerText", RpcTarget.All, "Red Team Wins");
            m_pv.RPC("ChangeTextToRed", RpcTarget.All);
        }
        else if (m_blueScore > m_redScore)
        {
            m_pv.RPC("UpdateWinnerText", RpcTarget.All, "Blue Team Wins");
            m_pv.RPC("ChangeTextToBlue", RpcTarget.All);
        }
        else
        {
            m_pv.RPC("UpdateWinnerText", RpcTarget.All, "Tie");
        }
        Debug.Log("a winner has been chosen");
    }

    protected void RaiseStartGameEvent()
    {
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            Hashtable m_playerTeam = new Hashtable();
            if (i % 2 == 0)
            {
                m_playerTeam["Team"] = "blue";
            }
            else
            {
                m_playerTeam["Team"] = "red";
            }
            m_playersArray[i].SetCustomProperties(m_playerTeam);
        }
        StartGameEvent();
    }

    void AddMessageToChat(string message)
    {
        m_pv.RPC("UpdateChat", RpcTarget.All, message);
    }

    #endregion

    #region PublicMethods

    public void RedTeamScored()
    {
        m_pv.RPC("UpdateRedTeamScore", RpcTarget.All);
    }

    public void BlueTeamScored()
    {
        m_pv.RPC("UpdateBlueTeamScore", RpcTarget.All);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player newplayer)
    {
        m_pv.RPC("UpdateChat", RpcTarget.All, newplayer.NickName + " entered the room");
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // probablemente el GameManager tenga que ser el masterclient
            PhotonNetwork.CurrentRoom.IsVisible = false;
            StartCoroutine(WaitForFirstTimer());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AddMessageToChat(otherPlayer.NickName + " lefte the room");
    }

    public void SendGoalMessage()
    {
        m_pv.RPC("UpdateChat", RpcTarget.All, m_ball.LastPlayer + " has scored a goal");
    }

    #endregion

    #region Corrutinas

    IEnumerator WaitForFirstTimer()
    {
        m_pv.RPC("UpdateTimer", RpcTarget.All, "Starting in: " + m_firstTimer.ToString());
        for (int i = m_firstTimer; i != 0; --i)
        {
            yield return new WaitForSeconds(1);
            m_firstTimer--;
            m_pv.RPC("UpdateTimer", RpcTarget.All, "Starting in: " + m_firstTimer.ToString());
        }
        RaiseStartGameEvent();
        StartCoroutine(WaitForLastTimer());
        
    }

    IEnumerator WaitForLastTimer()
    {
        m_pv.RPC("UpdateTimer", RpcTarget.All, m_lastTimer.ToString());
        for (int i = m_lastTimer; i != 0; --i)
        {
            yield return new WaitForSeconds(1);
            m_lastTimer--;
            m_pv.RPC("UpdateTimer", RpcTarget.All, m_lastTimer.ToString());
        }
        Debug.Log("finalizando partida");
        RaiseEndEvent();
    }

    IEnumerator WaitToDeleteChat()
    {
        yield return new WaitForSeconds(5);
        m_pv.RPC("DeleteChat", RpcTarget.All);
    }

    #endregion

    #region RPCMethods

    [PunRPC]
    void ChangeTextToBlue()
    {
        m_endText.color = Color.blue;
    }

    [PunRPC]
    void ChangeTextToRed()
    {
        m_endText.color = Color.red;
    }

    [PunRPC]
    void UpdateTimer(string text)
    {
        m_timer.text = text;
    }

    [PunRPC]
    void UpdateBlueTeamScore()
    {
        m_blueScore++;
        m_blueText.text = m_blueScore.ToString();
    }

    [PunRPC]
    void UpdateRedTeamScore()
    {
        m_redScore++;
        m_redText.text = m_redScore.ToString();
    }

    [PunRPC]
    void UpdateWinnerText(string text)
    {
        m_endText.text = text;
    }

    [PunRPC]
    void UpdateChat(string text)
    {
        m_chat.text += text + '\n';
        StartCoroutine(WaitToDeleteChat());
    }

    [PunRPC]
    void DeleteChat()
    {
        m_chat.text = " ";
    }

    #endregion

    #region Events

    void StartGameEvent()
    {
        byte m_ID = 1;//Codigo del Evento (1...199)
        object content = "Asignacion de nuevo rol...";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    void EndGameEvent()
    {
        byte m_ID = 2;
        object content = "Fin";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion
}
