using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestRotateGO : MonoBehaviour
{
    
    private Transform m_Transform;



    private void Awake()
    {

        m_Transform = gameObject.transform;

    }

    void Update()
    {

        //m_Transform.LookAt(Camera.main.WorldToScreenPoint(Input.mousePosition));

        LookAt2D(Camera.main.ScreenToWorldPoint(Input.mousePosition));

    }

    public void LookAt2D(Vector3 lookTarget)
    {

        // the direction we want the X axis to face (from this object, towards the target)
        Vector3 xDirection = (lookTarget - transform.position).normalized;

        // Y axis is 90 degrees away from the X axis
        Vector3 yDirection = Quaternion.Euler(0, 0, 90) * xDirection;

        // Z should stay facing forward for 2D objects
        Vector3 zDirection = Vector3.forward;

        // apply the rotation to this object
        m_Transform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }


}
