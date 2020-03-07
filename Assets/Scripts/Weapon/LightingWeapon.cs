using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightingWeapon : MonoBehaviour
{
    
    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;

    public int collisionCount = 0;
    public int maxCollisionCount = 4;

    public bool isTheEnd = false;

    public float damage = 5;

    [SerializeField]
    private GameObject hitPrefab;
    private GameObject hitGO;
    private Animator hitAnimator;

    //для преобразования при повороте
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;

    private void Awake()
    {

        m_Transform = gameObject.transform;
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        hitGO = Instantiate(hitPrefab, m_Transform);
        hitAnimator = hitGO.GetComponent<Animator>();
        hitGO.SetActive(false);

    }

    private void Update()
    {
        
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerTurn) LookAt2D(GetOurMouseMosition());
        else if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.PlayerWeaponWait) LookAt2D(m_Rigidbody2D.velocity);

    }

    private void LateUpdate()
    {

        if (collisionCount > maxCollisionCount) isTheEnd = true;

    }

    private Vector3 GetOurMouseMosition()
    {

        return Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall")) collisionCount++;
        else if (collision.gameObject.CompareTag("Enemy"))
        {

            isTheEnd = true;
            collision.gameObject.GetComponent<EnemyPawn>().TakeDamage(damage);

            hitGO.SetActive(true);
            hitAnimator.SetTrigger("Start");
            StartCoroutine(HitFXDelay());

        }
        else if (collision.gameObject.CompareTag("DestructionObject"))
        {



        }

    }

    IEnumerator HitFXDelay()
    {

        yield return new WaitForSeconds(0.5f);

        hitGO.SetActive(false);

    }

    private void OnEnable()
    {

        collisionCount = 0;
        isTheEnd = false;

    }

    public void LookAt2D(Vector3 lookTarget)
    {

        //Желаемое направление оси X, от которого устанавливаем ось Y. Z ось обращена к нам.
        xDirection = (lookTarget - transform.position).normalized;
        yDirection = Quaternion.Euler(0, 0, 90) * xDirection;
        zDirection = Vector3.forward;

        m_Transform.rotation = Quaternion.LookRotation(zDirection, yDirection);

    }

}

