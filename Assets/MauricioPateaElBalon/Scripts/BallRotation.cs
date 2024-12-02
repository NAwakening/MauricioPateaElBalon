using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRotation : MonoBehaviour
{
    #region References

    [SerializeField] float rotationSpeed;

    #endregion

    #region UnityMethods

    void Start()
    {

    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.back, rotationSpeed * Time.fixedDeltaTime);
    }
    #endregion
}
