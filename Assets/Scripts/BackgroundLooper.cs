using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] Renderer backgroudRenderer;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        backgroudRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
    }
}
