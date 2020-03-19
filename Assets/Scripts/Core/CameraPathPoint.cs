using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Точки положений камеры для каждой из комнат.
/// При старте заполняем массив точек в GameManager.
/// </summary>
public class CameraPathPoint : MonoBehaviour
{

    private void Awake()
    {

        //Если нет точки в массиве, то добавляем её и сортируем массив по имени.
        if (!GameManager.Instance.camPathPoint.Contains(gameObject.transform))
        {

            GameManager.Instance.camPathPoint.Add(gameObject.transform);
            GameManager.Instance.camPathPoint.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });

        }

    }

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
