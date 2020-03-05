using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTriggerForChangeGameState : MonoBehaviour
{

    [SerializeField]
    private GameManager.GameMode m_GameMode;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        GameManager.Instance.ChangeGameMode(m_GameMode);

    }

}
