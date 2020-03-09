using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFXAfterAnim : MonoBehaviour
{

    [SerializeField]
    private Animator m_Animator;
    private bool isDestroy = false;


    private void Update()
    {

        if (!isDestroy && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("FX_Hit_01_EndFrame"))
        {

            isDestroy = true;
            Destroy(gameObject);

        } 

    }


}
