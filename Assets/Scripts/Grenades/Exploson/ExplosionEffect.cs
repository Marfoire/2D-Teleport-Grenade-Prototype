using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    public float GrenadeLiveTime = 1.5f;
    public float DeathTime = 1;
    // Start is called before the first frame update
    Color colorBegin;
    Color colorEnd;

    void Start()
    {
        colorBegin = GetComponent<SpriteRenderer>().color;
        colorEnd = new Color(colorBegin.r, colorBegin.g, colorBegin.b, 0f);
        
    }
    void Update()
    {
        GrenadeLiveTime += DeathTime*Time.deltaTime;
        GetComponent<SpriteRenderer>().color = Color.Lerp(colorBegin, colorEnd, GrenadeLiveTime * 0.1f);

        if (transform.GetComponent<SpriteRenderer>().color.a == 0)
        {
            GameObject.Destroy(gameObject);
        }

    }


}
