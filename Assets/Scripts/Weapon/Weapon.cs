using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Контроль оружия и эффектов попадания.
/// </summary>
public class Weapon : MonoBehaviour
{

    [Header("Максимальное количество коллизий перед взрывом снаряда")]
    [SerializeField]
    private int maxCollisionCount = 4;
    //Настоящее количество попаданий во что-либо кроме павна
    private int collisionCount = 0;

    [Header("Наносимый урон")]
    [SerializeField]
    public float damage = 4;

    //GameObject FX попадания, трансформ, аниматор.
    private GameObject hitGOPrefab;
    private GameObject hitGO;
    private Transform hitGOTransform;
    private Animator hitAnimator;

    //FX попадания не по павну
    private GameObject hitSimpleLightingPrefab;
    private GameObject hitSimpleLighting;
    private Transform hitSimpleLightingTransform;
    private ParticleSystem hitSimpleLightingPS;

    private int findIndex;

    private void Awake()
    {

        LoadResources();

        hitGO = Instantiate(hitGOPrefab);
        hitGOTransform = hitGO.GetComponent<Transform>();
        hitAnimator = hitGO.GetComponent<Animator>();

        hitSimpleLighting = Instantiate(hitSimpleLightingPrefab);
        hitSimpleLightingTransform = hitSimpleLighting.GetComponent<Transform>();
        hitSimpleLightingPS = hitSimpleLighting.GetComponent<ParticleSystem>();

    }

    public virtual void LoadResources()
    {

        hitGOPrefab = Resources.Load("Hit") as GameObject;

        hitSimpleLightingPrefab = Resources.Load("SimpleLightingIskri") as GameObject;

    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall") | collision.gameObject.CompareTag("DestructionObject"))
        {

            if (collision.gameObject.CompareTag("DestructionObject"))
            {

                hitGOTransform.position = collision.GetContact(0).point;
                hitAnimator.SetTrigger("isStart");
                hitGOTransform.parent = null;

                Destroy(collision.gameObject);

            }

            collisionCount++;

            if (collisionCount > maxCollisionCount)
            {

                hitSimpleLightingTransform.position = collision.GetContact(0).point;
                hitSimpleLightingPS.Play();

                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

                gameObject.SetActive(false);

            }

        }
        else if (collision.gameObject.CompareTag("Enemy") | collision.gameObject.CompareTag("Player"))
        {

            if (collision.gameObject.CompareTag("Player")) GameManager.Instance.m_HeroPawn.TakeDamage(Random.Range((damage - (damage / 5)), (damage + (damage / 5))));
            else
            {

                findIndex = GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyPWGO.IndexOf(collision.gameObject);

                GameManager.Instance.enemyInRoom[GameManager.Instance.camPointNumber].enemyPW[findIndex].TakeDamage(Random.Range((damage - (damage / 5)), (damage + (damage / 5))));

            }

            hitGOTransform.position = collision.GetContact(0).point;
            hitAnimator.SetTrigger("isStart");

            GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

            gameObject.SetActive(false);

        }

    }

    private void OnEnable()
    {

        collisionCount = 0;

    }

    private void OnDestroy()
    {

        Destroy(hitGO);
        Destroy(hitSimpleLighting);

    }

}
