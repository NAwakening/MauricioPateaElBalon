using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    [SerializeField] TextMeshProUGUI m_timer;
    [SerializeField] TextMeshProUGUI m_endText;
    [SerializeField] TextMeshProUGUI m_redText;
    [SerializeField] TextMeshProUGUI m_blueText;

    int m_firstTimer = 3;
    int m_lastTimer = 200;
    int m_redScore = 0;
    int m_blueScore = 0;

    // Start is called before the first frame update
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void RaiseEndEvent()
    {
        if (m_redScore > m_blueScore)
        {
            m_endText.text = "Red Team Wins";
            m_endText.color = Color.red;
        }
        else if (m_blueScore > m_redScore)
        {
            m_endText.text = "Blue Team Wins";
            m_endText.color = Color.blue;
        }
        else
        {
            m_endText.text = "Tie";
            m_endText.color = Color.white;
        }
        byte m_ID = 2;
        object content = "Fin";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void RedTeamScored()
    {
        m_redScore++;
        m_redText.text = m_redScore.ToString();
    }

    public void BlueTeamScored()
    {
        m_blueScore++;
        m_blueText.text = m_blueScore.ToString();
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
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // probablemente el GameManager tenga que ser el masterclient
            PhotonNetwork.CurrentRoom.IsVisible = false;
            StartCoroutine(WaitForFirstTimer());
        }
    }

    IEnumerator WaitForFirstTimer()
    {
        m_timer.text = "Starting in" + m_firstTimer.ToString();
        if (m_firstTimer == 0)
        {
            byte m_ID = 1;//Codigo del Evento (1...199)
            object content = "Asignacion de nuevo rol...";
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
            StartCoroutine(WaitForLastTimer());
        }
        else
        {
            yield return new WaitForSeconds(1);
            m_firstTimer--;
        }
    }

    IEnumerator WaitForLastTimer()
    {
        m_timer.text = m_lastTimer.ToString();
        if(m_lastTimer == 0)
        {
            RaiseEndEvent();
        }
        else
        {
            yield return new WaitForSeconds(1);
            m_lastTimer--;
        }
    }
}
