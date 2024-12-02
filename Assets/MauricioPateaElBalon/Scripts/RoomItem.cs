using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_roomName;
    PhotinConnection m_photonConnectionManager;
    string m_nameRoom;
    // Start is called before the first frame update
    void Start()
    {
        m_photonConnectionManager = FindObjectOfType<PhotinConnection>();
    }

    public void SetNewRoomName(string roomName, string p_newInfo)
    {
        m_nameRoom = roomName;
        m_roomName.text = roomName + p_newInfo;
    }
    public void ClickToJoinRoom()
    {
        m_photonConnectionManager.JoinRoom(m_nameRoom);
    }
}
