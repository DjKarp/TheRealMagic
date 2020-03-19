using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт вешаем на точки пути автоматического перемещения рыцаря между комнатами.
/// Рисуем редакторе сцены сами точки и их соединения между собой. И заполняем массив точек в GameManager.
/// </summary>
public class LevelHeroPathPoint : MonoBehaviour
{

    public static LevelHeroPathPoint lastAwailablePoint;
    
    /// <summary>
    /// Выбор точки указателя. Типо она первая или последняя, или какая-то между ними
    /// </summary>
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
        //Если нет точки в массиве, то добавляем её и сортируем массив по имени.
        if (!GameManager.Instance.pathPointHero.Contains(gameObject.transform))
        {

            GameManager.Instance.pathPointHero.Add(gameObject.transform);

            GameManager.Instance.pathPointHero.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });

        }

    }

    /// <summary>
    /// Возвращает следующую доступную точку
    /// </summary>
    /// <returns>Трансформ следующей точки</returns>
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
#if UNITY_EDITOR
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
#endif
    private void OnDestroy()
    {

        allPoints.Remove(this);

    }

}
