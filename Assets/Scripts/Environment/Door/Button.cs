using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public float Timer = 10;
    public Animator anim;
    public bool ButtonPressed;
    // Start is called before the first frame update
    void Start()
    {
        ButtonPressed = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("BPressed", ButtonPressed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" || other.tag == "Grenade" || other.tag == "MoveableObject")
        {
            ButtonPressed = true;
            StartCoroutine(ButtonTimer());
        }
    }

    IEnumerator ButtonTimer()
    {
        yield return new WaitForSeconds(Timer);
        ButtonPressed = false;
    }
}
