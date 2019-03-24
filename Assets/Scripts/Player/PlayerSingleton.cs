using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    //work in progress, no commenting yet ignore this script because it doesn't do anything yet
    public static GameObject playerGameObject;

    private void Awake()
    {
        playerGameObject = gameObject;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            Debug.Break();
        }
#endif
    }
}
