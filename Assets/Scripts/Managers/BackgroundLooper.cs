using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public static BackgroundLooper Instance => m_instance;
    private static BackgroundLooper m_instance;

    float intialSpeed = 0.1f;

    private float speed;
    [SerializeField] Renderer backgroudRenderer;
    // Start is called before the first frame update

    private void Start()
    {
        m_instance = this;
        speed = intialSpeed;
    }
    // Update is called once per frame
    void FixedUpdate()
    {   if (GameManager.Instance.GameState()) //If game is playing, loop background
        {
            backgroudRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
        }
    }

    public void ChangeSpeed(int val)
    {
        intialSpeed = speed;
        speed = val;
    }

    public void ChangePerSpeed(float amount)
    {
        intialSpeed += amount;
        speed = intialSpeed;

    }

    public void ResetSpeed()
    {
        speed = intialSpeed;
    }
}
