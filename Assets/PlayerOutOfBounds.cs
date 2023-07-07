using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutOfBounds : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 playerOrigin;
    [SerializeField] Transform player;
    void Start()
    {
        playerOrigin = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.y < -7)
        {
            player.position = new Vector2(playerOrigin.x, playerOrigin.y);
        }

        else if (transform.position.y > 7)
        {
            player.position = new Vector2(playerOrigin.x, playerOrigin.y);
        }
    }
}
