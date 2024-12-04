using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using System.Xml.Serialization;

public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region  Knobs

    [SerializeField] float m_speed;
    [SerializeField] float m_rotSpeed = 5;
    [SerializeField] float m_kickForce;

    #endregion

    #region References

    [SerializeField] Rigidbody m_rb;
    [SerializeField] Animator m_anim;
    [SerializeField] PhotonView m_pv;
    [SerializeField] Transform m_camera;
    [SerializeField] Transform m_playerRenderer;
    [SerializeField] Transform m_orientation;
    [SerializeField] TextMeshPro m_nickname;
    [SerializeField] Transform m_ballPosition;

    #endregion

    #region RunTimeVariables

    float m_hor, m_vert;
    Vector3 m_direction;
    [SerializeField]bool m_canPlay, m_hasBall;
    string m_team;
    GameObject m_ball;

    #endregion

    #region UnityMethods

    private void Start()
    {

    }

    private void Update()
    {
        if (m_pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E) && m_hasBall)
            {
                m_ball = m_ballPosition.GetChild(0).gameObject;
                m_ball.GetComponent<Rigidbody>().isKinematic = false;
                m_ball.transform.SetParent(null);
                m_ball.GetComponent <Rigidbody>().AddForce(m_playerRenderer.transform.forward * m_kickForce, ForceMode.Impulse);
                m_hasBall = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //codigo para salir del nivel
            }
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            m_ball = other.gameObject;
            m_ball.GetComponent<Rigidbody>().isKinematic = true;
            m_ball.transform.position = m_ballPosition.position;
            m_ball.transform.SetParent(m_ballPosition, true);
            m_hasBall = true;
        }
    }

    public void OnEnable()
    {
        if (m_pv.IsMine)
        {
            PhotonNetwork.AddCallbackTarget(this); 
        }
    }

    public void OnDisable()
    {
        if (m_pv.IsMine)
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }

    #endregion

    #region LocalMethods

    void Movement()
    {
        if (m_pv.IsMine && m_canPlay)
        {
            m_orientation.forward = (transform.position - new Vector3(m_camera.position.x, transform.position.y, m_camera.position.z)).normalized;
            m_hor = Input.GetAxisRaw("Horizontal");
            m_vert = Input.GetAxisRaw("Vertical");
            m_direction = (m_hor * m_orientation.right + m_vert * m_orientation.forward).normalized;
            m_anim.SetFloat("MoveSpeed", m_direction.magnitude);
            if (m_direction.magnitude > 0)
            {
                m_playerRenderer.forward = Vector3.Slerp(m_playerRenderer.forward, m_direction.normalized, Time.fixedDeltaTime * m_rotSpeed);
            }
            m_rb.velocity = m_direction.normalized * m_speed;
        }
    }

    void GetNewGameplayRole()
    {
        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object role))
        {
            m_team = role.ToString();

            // codigo para ver a que equipo pertences
        }
    }

    #endregion

    #region PublicMethods

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case 1:
                GetNewGameplayRole();
                break;
        }
    }

    #endregion

    #region RPCMethods

    

    #endregion

    #region IEnumarator

    

    #endregion

    #region Events

    

    #endregion
}
