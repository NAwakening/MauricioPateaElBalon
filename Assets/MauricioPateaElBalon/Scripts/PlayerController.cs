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
    [SerializeField] BoxCollider m_sweept;
    [SerializeField] AudioSource m_audioSource;

    #endregion

    #region RunTimeVariables

    float m_hor, m_vert;
    Vector3 m_direction;
    [SerializeField]bool m_canPlay, m_hasBall, m_canSweept = true;
    string m_team;
    GameObject m_ball;

    #endregion

    #region UnityMethods

    private void Start()
    {
        m_ball = FindObjectOfType<Ball>().gameObject;
        m_pv.RPC("GetNickname", RpcTarget.All);
    }

    private void Update()
    {
        if (m_pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E) && m_canPlay)
            {
                if (m_hasBall)
                {
                    m_pv.RPC("KickBall", RpcTarget.All);
                }
                else if (m_canSweept)
                {
                    m_pv.RPC("Swept", RpcTarget.All);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LevelManager.instance.LeaveRoom();
            }
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (other.transform.parent == null)
            {
                GetBall(other.gameObject);
            }
        }
        if (other.gameObject.CompareTag("Sweept"))
        {
            Debug.Log("AUUUUUUU");
            if (m_hasBall)
            {
                m_pv.RPC("LooseBall", RpcTarget.All, other);
            }
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

    void GetTeamRole()
    {
        m_pv.RPC("ShowNickname", RpcTarget.All);
    }

    void StartGame()
    {
        m_canPlay = true;
        m_audioSource.Play();
        GetTeamRole();
    }

    void EndGame()
    {
        m_canPlay = false;
        m_audioSource.Stop();
    }

    #endregion

    #region PublicMethods

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch (eventCode)
        {
            case 1:
                StartGame();
                break;
            case 2:
                EndGame();
                break;
        }
    }

    public void GetBall(GameObject ball)
    {
        m_ball = ball;
        m_pv.RPC("SetBallAsMine", RpcTarget.All);
    }

    #endregion

    #region RPCMethods

    [PunRPC]
    void GetNickname()
    {
        if (m_pv.IsMine)
        {
            m_nickname.text = PhotonNetwork.NickName;
        }
    }

    [PunRPC]
    void ShowNickname()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object role) && m_pv.IsMine)
        {
            m_team = role.ToString();
            if (m_team == "red")
            {
                m_nickname.color = Color.red;
                transform.position = new Vector3(14, 0, transform.position.z);
            }
            else
            {
                m_nickname.color = Color.blue;
                transform.position = new Vector3(-14, 0, transform.position.z);
            }

        }
    }

    [PunRPC]
    void SetBallAsMine()
    {
        m_ball.GetComponent<Ball>().LastPlayer = m_nickname.text;
        m_ball.GetComponent<Rigidbody>().isKinematic = true;
        m_ball.transform.position = m_ballPosition.position;
        m_ball.transform.SetParent(m_ballPosition, true);
        m_hasBall = true;
    }

    [PunRPC]
    void LooseBall(Collision other)
    {
        m_ball.transform.parent = null;
        GetComponent<Rigidbody>().isKinematic =  false;
        other.transform.parent.parent.parent.GetComponent<PlayerController>().GetBall(m_ball);
        m_hasBall = false;
    }

    [PunRPC]
    void KickBall()
    {
        m_ball.GetComponent<Rigidbody>().isKinematic = false;
        m_ball.transform.parent = null;
        m_ball.GetComponent<Rigidbody>().AddForceAtPosition(m_playerRenderer.transform.forward * m_kickForce, transform.position, ForceMode.Impulse);
        m_hasBall = false;
        m_anim.Play("Kick");
    }

    [PunRPC]
    void Swept()
    {
        m_sweept.enabled = true;
        m_anim.Play("Swept");
        m_canSweept = false;
        m_rb.AddForce(m_playerRenderer.forward * 2f, ForceMode.Impulse);
        m_canPlay = false;
        StartCoroutine(SweeptCooldown());
    }

    [PunRPC]
    void StopSwept()
    {
        m_sweept.enabled = false;
    }

    #endregion

    #region IEnumarator

    IEnumerator SweeptCooldown()
    {
        yield return new WaitForSeconds(1f);
        m_pv.RPC("StopSwept", RpcTarget.All);
        m_canPlay = true;
        yield return new WaitForSeconds(1.5f);
        m_canSweept = true;
    }

    #endregion

    #region Events



    #endregion

    #region GettersAndSetters

    public bool HasBall
    {
        set { m_hasBall = value; }
    }

    #endregion
}
