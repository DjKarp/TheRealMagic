using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMoveVerThree : MonoBehaviour
{

    [Header("Скорость движения")]
    [SerializeField]
    private float speed = 1.0f;

    private Transform m_Transform;
    private Rigidbody2D m_Rigidbody2D;

    private Vector3 mousePosition;

    private bool isStartMove = false;

    //для преобразования при повороте
    private Vector3 xDirection;
    private Vector3 yDirection;
    private Vector3 zDirection;

    private void Awake()
    {

        m_Transform = gameObject.transform;
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        
        if (!isStartMove)
        {

            if (Input.GetMouseButtonUp(0))
            {

                isStartMove = true;

                m_Rigidbody2D.AddForce((GetOurMouseMosition() - m_Transform.position) * speed * 10 * Time.deltaTime, ForceMode2D.Force);

                LookAt2D(GetOurMouseMosition());

            }

        }

    }

    private void LateUpdate()
    {

        if (isStartMove) LookAt2D(m_Rigidbody2D.velocity);

    }

    private Vector3 GetOurMouseMosition()
    {

        return mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


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

