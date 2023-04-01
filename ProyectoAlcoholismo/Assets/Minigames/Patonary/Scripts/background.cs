using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{

    public GameObject fondo;
    private SpriteRenderer spriteFondo;

    private void Start()
    {
        spriteFondo = fondo.GetComponent<SpriteRenderer>();
        if (spriteFondo != null)
        {
            transform.localScale = new Vector3(1, 1, 1);

            float width = spriteFondo.sprite.bounds.size.x;
            float height = spriteFondo.sprite.bounds.size.y;

            float worldScreenHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
        }
    }
}
