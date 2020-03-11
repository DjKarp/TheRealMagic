using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffOnMove : MonoBehaviour
{

    public GameObject[] gameObjectsOff;

    public bool isOff = false;


    private void Update()
    {
        
        if (GameManager.Instance.camPointNumber >= 4 && !isOff)
        {

            isOff = true;
            foreach (GameObject go in gameObjectsOff) if (go != null) go.SetActive(false);

        }

    }

}
