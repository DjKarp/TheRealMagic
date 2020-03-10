using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathPoint : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {

        Gizmos.color = Color.magenta;

        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        int i;
        int.TryParse(gameObject.name.Substring(8, gameObject.name.Length - 8), out i);
        GameObject nextPathPoint = GameObject.Find(gameObject.name.Substring(0, 8) + (i + 1).ToString());

        if (nextPathPoint != null) Debug.DrawLine(transform.position, nextPathPoint.transform.position, Color.magenta);

    }
#endif

}
