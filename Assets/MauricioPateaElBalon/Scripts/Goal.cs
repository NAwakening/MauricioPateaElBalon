using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    #region Knobs

    [SerializeField] bool m_red;

    #endregion

    #region References

    [SerializeField] ParticleSystem m_ParticleSystem;
    [SerializeField] AudioSource m_audio;

    #endregion

    #region UnityMethods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (other.transform.parent != null)
            {
                other.transform.parent.parent.parent.parent.GetComponent<PlayerController>().HasBall = false;
                other.transform.parent = null;
                other.GetComponent<Rigidbody>().isKinematic = false;
            }
            LevelManager.instance.SendGoalMessage();
            other.GetComponent<Ball>().ResetPosition();
            m_ParticleSystem.Play();
            m_audio.Play();
            if (m_red)
            {
                LevelManager.instance.RedTeamScored();
            }
            else
            {
                LevelManager.instance.BlueTeamScored();
            }
        }
    }

    #endregion
}
