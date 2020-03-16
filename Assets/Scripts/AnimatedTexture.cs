using UnityEngine;
using System.Collections;
/// <summary>
/// Скрипт простенькой анимации через добавление фреймов в инспекторе
/// </summary>
public class AnimatedTexture : MonoBehaviour
{
    public float fps = 30.0f;
    public Sprite[] frames;

    private int frameIndex;
    private SpriteRenderer m_SpriteRenderer;

    public bool isSlow = true;
    private float slowOnTimer;
    private float slowOffTimer;
    private float timer = 0.0f;


    private void Awake()
    {

        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        slowOffTimer = 1 / fps;
        slowOnTimer = slowOffTimer * 2;

    }

    void Update()
    {

        if (!isSlow)
        {

            if (timer < slowOffTimer) timer += Time.deltaTime;
            else
            {

                m_SpriteRenderer.sprite = frames[frameIndex];
                frameIndex = (frameIndex + 1) % frames.Length;
                timer = 0.0f;

            }

        }
        else
        {

            if (timer < slowOnTimer) timer += Time.deltaTime;
            else
            {

                m_SpriteRenderer.sprite = frames[frameIndex];
                frameIndex = (frameIndex + 1) % frames.Length;
                timer = 0.0f;

            }
            
        }

    }

    public void SetIsSlow(bool m_IsSlow)
    {

        isSlow = m_IsSlow;

    }

}