using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    //work in progress, no commenting yet
    public static GameObject playerGameObject;

    private void Awake()
    {
        playerGameObject = gameObject;
    }
}
