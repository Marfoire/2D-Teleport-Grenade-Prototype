using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCollection : MonoBehaviour
{

    public PlayerController playerCon;

    public GameObject[] GrenadeArray1;
    public GameObject[] GrenadeArray2;
    public GameObject[] GrenadeArray3;
    // Start is called before the first frame update
    void Start()
    {
        playerCon = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerCon.grenadeTypePrefabs = GrenadeArray1;
    }
}
