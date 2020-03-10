using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class LevelHeroPathPoint : MonoBehaviour
{

    public static LevelHeroPathPoint lastAwailablePoint;
    
    public PointType pointType = PointType.PathPoint;
    public enum PointType
    {

        StartPoint,
        PathPoint,
        FinishPoint

    }

    //Отметка пройденых точек
    public bool pointPassed; 

    public List<LevelHeroPathPoint> linkedPoints = new List<LevelHeroPathPoint>();
    public static List<LevelHeroPathPoint> allPoints = new List<LevelHeroPathPoint>();


    private float size;

    private void Awake()
    {

        switch (pointType)
        {

            case PointType.PathPoint:

                break;

            case PointType.StartPoint:
                pointPassed = true;
                break;

            case PointType.FinishPoint:

                break;

            default:
                break;

        }

        allPoints.Add(this);

    }

    internal static Transform GetNextPoint()
    {

        Transform tempTransform = null;

        foreach(LevelHeroPathPoint point in allPoints)
        {

            if (point.pointPassed)
            {

                tempTransform = point.gameObject.transform;
                point.pointPassed = false;

            }

        }

        return tempTransform;

    }

    internal static LevelHeroPathPoint GetNextAwailablePoint()
    {
        if (allPoints.Count == 0) return null;

        //Находим стартовую точку
        LevelHeroPathPoint startPoint = allPoints.Find(kpp => kpp.pointType == PointType.StartPoint);

        if (lastAwailablePoint != null) startPoint = lastAwailablePoint;

        if (startPoint == null)
        {

            Debug.LogError("На сцене нет стартовой точки.");
            return null;

        }

        foreach (LevelHeroPathPoint point in startPoint.linkedPoints)
        {

            if (point == null) continue;
            if (point.pointPassed) continue;

            if (Linecast(startPoint.transform.position, point.transform.position))
            {

                Debug.Log("Попал физикой в другую точку: " + point.name, point);

                LevelHeroPathPoint targetPoint = point;

                //Точка доступна
                while (targetPoint.pointPassed == true)
                {

                    foreach (LevelHeroPathPoint linkPoint in targetPoint.linkedPoints)
                    {

                        if (linkPoint.pointPassed) continue;

                        if (Linecast(targetPoint.transform.position, linkPoint.transform.position))
                        {

                            targetPoint = linkPoint;
                            continue;

                        }

                    }

                    break;

                }

                lastAwailablePoint = targetPoint;

                return targetPoint;

            }

        }

        return null;

    }

    public static bool Linecast(Vector3 start, Vector3 end)
    {

        List<RaycastHit2D> results = new List<RaycastHit2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();

        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;

        LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");

        contactFilter.SetLayerMask(layerMask);

        int lineCasts = Physics2D.Linecast((Vector2)start, (Vector2)end, contactFilter, results);

        if (lineCasts > 0)
        {

            foreach (RaycastHit2D hit in results)
            {

                if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "Enemy")
                {

                    lineCasts--;

                }

            }

        }

        return lineCasts == 0;

    }


    internal void Passed()
    {

        pointPassed = true;

    }
         
    public void OnDrawGizmos()
    {
        
        switch (pointType)
        {

            case PointType.PathPoint:
                Gizmos.color = Color.blue;
                break;

            case PointType.StartPoint:
                Gizmos.color = Color.green;
                break;

            case PointType.FinishPoint:
                Gizmos.color = Color.red;
                break;

            default:
                break;

        }

        size = 0.15f;

        if (lastAwailablePoint == this) size = 0.3f;

        Gizmos.DrawSphere(transform.position, size);
        Gizmos.DrawWireSphere(transform.position, size);

        if (Application.isPlaying) return;

        foreach (LevelHeroPathPoint pathPoint in linkedPoints)
        {

            if (pathPoint == null)
            {

                Debug.Log("PathPoint is null on {gameObject}" + gameObject);

                continue;

            }

            if (pointPassed || pathPoint.pointPassed) Debug.DrawLine(transform.position, pathPoint.transform.position, Color.red);
            else Debug.DrawLine(transform.position, pathPoint.transform.position, Color.yellow);

        }

    }

    private void OnDestroy()
    {

        allPoints.Remove(this);

    }




#if UNITY_EDITOR
    [CustomEditor(typeof(LevelHeroPathPoint))]
    class KnightPathpointEditor : Editor
    {

        private void OnSceneGUI()
        {

            LevelHeroPathPoint m_PathPoint = target as LevelHeroPathPoint;

            foreach (LevelHeroPathPoint pathPoint in FindObjectsOfType<LevelHeroPathPoint>())
            {
                if (pathPoint == m_PathPoint) continue;

                Vector3 midpoint = m_PathPoint.transform.position + ((pathPoint.transform.position - m_PathPoint.transform.position) * 0.8f);

                if (!m_PathPoint.linkedPoints.Contains(pathPoint))
                {

                    //Точки нет, рисуем добавлялку
                    if (Handles.Button(midpoint, Quaternion.identity, 0.2f, 0.5f, Handles.RectangleHandleCap))
                    {

                        m_PathPoint.linkedPoints.Add(pathPoint);
                        pathPoint.linkedPoints.Add(m_PathPoint);

                    }

                }
                else
                {

                    Debug.DrawLine(m_PathPoint.transform.position, m_PathPoint.transform.position, Color.yellow);

                    //Точки нет, рисуем добавлялку
                    if (Handles.Button(midpoint, Quaternion.identity, 0.2f, 0.5f, Handles.CircleHandleCap))
                    {
                        m_PathPoint.linkedPoints.Remove(pathPoint);
                        pathPoint.linkedPoints.Remove(m_PathPoint);

                    }

                }

            }

        }

    }

#endif

}
