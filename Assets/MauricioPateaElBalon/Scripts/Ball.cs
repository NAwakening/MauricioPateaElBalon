using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region References

    [SerializeField] Rigidbody m_rb;

    #endregion

    #region RuntimeVariables

    string m_lastPlayer;

    #endregion

    #region UnityMethods

    private void Update()
    {
        if (transform.position.x <= -31f || transform.position.x >= 31f || transform.position.y <= -1 || transform.position.z <= -20f || transform.position.z >= 21f)
        {
            ResetPosition();
        }
    }

    #endregion

    #region PublicMethods

    public void ResetPosition()
    {
        transform.position = new Vector3(0.32f, 5f, 1f);
        m_rb.velocity = Vector3.zero;
    } 

    #endregion

    #region GettersAndSetters

    public string LastPlayer
    {
        get { return m_lastPlayer; }
        set { m_lastPlayer = value; }
    }

    #endregion
}
