using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallistaWeapon : MonoBehaviour
{

    [SerializeField]
    private Transform shootPoint;

    [SerializeField]
    private Transform sprite;

    [SerializeField]
    private Image shootPowerSlider;

    [SerializeField]
    private float maxShootStrange = 10.0f;
    [SerializeField]
    private float minShootStrange = 0.0f;

    private float currentShootStrange;

    [SerializeField]
    private float timeShootLoad = 5.0f;
    private float currentTimeShootLoad = 0.0f;

    private Vector3 mousePosition;
    private Vector3 startPosition;

    private Rigidbody2D m_Rigidbody2D;

    private Transform m_Transform;



    protected virtual void Awake()
    {

        currentShootStrange = minShootStrange;

        ChangePowerSlider();

        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();

        m_Transform = gameObject.transform;

        startPosition = m_Transform.position;

    }

    protected virtual void Update()
    {

        //sprite.position = shootPoint.position;

        if (Input.GetMouseButtonDown(0))
        {

            StartCoroutine(ChangeStrangeShootPower());

        }

    }

    IEnumerator ChangeStrangeShootPower()
    {

        m_Rigidbody2D.isKinematic = true;
     
        while (!Input.GetMouseButtonUp(0))
        {

            //m_Transform.position = shootPoint.position;

            if (currentTimeShootLoad < timeShootLoad) currentTimeShootLoad += Time.deltaTime;

            currentShootStrange = minShootStrange + (((maxShootStrange - minShootStrange) * currentTimeShootLoad) / timeShootLoad);

            ChangePowerSlider();

            yield return null;

        }

        m_Rigidbody2D.isKinematic = false;

        m_Rigidbody2D.AddForce((GetOurMouseMosition() - m_Transform.position) * currentShootStrange * Time.deltaTime, ForceMode2D.Impulse);

        currentTimeShootLoad = 0.0f;
        currentShootStrange = minShootStrange;
        ChangePowerSlider();

    }

    protected virtual void ChangePowerSlider()
    {

        shootPowerSlider.fillAmount = (currentShootStrange - minShootStrange) / (maxShootStrange - minShootStrange);

    }

    private Vector3 GetOurMouseMosition()
    {

        return mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

}
