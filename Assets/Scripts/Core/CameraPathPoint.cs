using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathPoint : MonoBehaviour
{

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
                
    }

}
