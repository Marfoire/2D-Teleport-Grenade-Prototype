using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    public float GrenadeLiveTime = 1.5f;
    public float DeathTime = 1;

    Color colorBegin;
    Color colorEnd;

    void Start()
    {
        // set the color as the current color
        colorBegin = GetComponent<SpriteRenderer>().color;
        // set the end color as the same colour but with no alpha
        colorEnd = new Color(colorBegin.r, colorBegin.g, colorBegin.b, 0f);
        
    }
    void Update()
    {
        //when the grenade is created the lifetime is infilenced by the death time or how long it takes for the grenade to die
        GrenadeLiveTime += DeathTime*Time.deltaTime;
        //lerp from the start color to end color
        GetComponent<SpriteRenderer>().color = Color.Lerp(colorBegin, colorEnd, GrenadeLiveTime * 0.1f);

        // when the alhpa is equal to 0, kill the game object
        if (transform.GetComponent<SpriteRenderer>().color.a == 0)
        {
            GameObject.Destroy(gameObject);
        }

    }


}
