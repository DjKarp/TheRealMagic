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
    protected int maxCollisionCount = 4;
    //Настоящее количество попаданий во что-либо кроме павна
    protected int collisionCount = 0;

    [Header("Наносимый урон")]
    [SerializeField]
    public float damage = 4;

    protected Transform m_Transform;

    //GameObject FX попадания, трансформ, аниматор.
    protected GameObject hitGOPrefab;
    protected GameObject hitGO;
    protected Transform hitGOTransform;
    protected Animator hitAnimator;

    //FX попадания не по павну
    [SerializeField]
    protected GameObject hitSimplePrefab;
    protected GameObject hitSimple;
    protected Transform hitSimpleTransform;
    protected ParticleSystem hitSimplePS;
    protected Animator hitSimpleAnimator;

    public GameObject hitRockDestroyPrefab;

    protected int findIndex;

    public virtual void Awake()
    {

        m_Transform = gameObject.transform;

        LoadResources();

        hitGO = Instantiate(hitGOPrefab);
        hitGOTransform = hitGO.GetComponent<Transform>();
        hitAnimator = hitGO.GetComponent<Animator>();

        hitSimple = Instantiate(hitSimplePrefab);
        hitSimpleTransform = hitSimple.GetComponent<Transform>();
        hitSimplePS = hitSimple.GetComponent<ParticleSystem>();
        hitSimpleAnimator = hitSimple.GetComponent<Animator>();

        hitRockDestroyPrefab = Resources.Load("RockDieParticles") as GameObject;

    }

    public virtual void LoadResources()
    {

        hitGOPrefab = Resources.Load("Hit") as GameObject;

        if (hitSimplePrefab == null) hitSimplePrefab = Resources.Load("SimpleLightingIskri") as GameObject;

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

                Instantiate(hitRockDestroyPrefab, collision.GetContact(0).point, Quaternion.identity);

                Destroy(collision.gameObject);

            }

            collisionCount++;

            if (collisionCount > maxCollisionCount)
            {

                hitSimpleTransform.position = collision.GetContact(0).point;
                if (hitSimplePS != null) hitSimplePS.Play();
                if (hitSimpleAnimator != null) hitSimpleAnimator.SetTrigger("isStart");

                GameManager.Instance.ChangeGameMode(GameManager.GameMode.EnemyTurn);

                SoundAndMusic.Instance.PlayWeaponDamage();

                gameObject.SetActive(false);

            }
            else
            {

                SoundAndMusic.Instance.PlayLightingRicoshetSound();

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

            SoundAndMusic.Instance.PlayWeaponDamage();

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
        Destroy(hitSimple);

    }

}
