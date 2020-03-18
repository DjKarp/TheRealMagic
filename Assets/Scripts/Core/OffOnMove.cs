using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Отключение объектов с коллайдерами, которые могут пошать проходу игрока в следующую комнату. Если игрок их не разрушил к этому моменту.
/// </summary>
public class OffOnMove : MonoBehaviour
{

    [Header("Список выключаемых объектов")]
    public GameObject[] gameObjectsOff;

    [Header("Точка позиции камеры, после которой, следует отключить объекты")]
    public int camPointNumberTarget = 0;


    private bool isOff = false;



    private void Update()
    {
        
        if (!isOff && GameManager.Instance.camPointNumber >= camPointNumberTarget)
        {

            isOff = true;
            GameObjectOffNow();

        }

    }

    private void GameObjectOffNow()
    {

        foreach (GameObject go in gameObjectsOff) if (go != null) go.SetActive(false);

    }

}
