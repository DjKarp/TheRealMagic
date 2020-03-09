using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFXAfterAnim : MonoBehaviour
{

    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private string nameAnimation;
    private bool isDestroy = false;


    private void Update()
    {

        if (!isDestroy && m_Animator.GetCurrentAnimatorStateInfo(0).IsName(nameAnimation))
        {

            isDestroy = true;
            Destroy(gameObject);

        } 

    }


}
