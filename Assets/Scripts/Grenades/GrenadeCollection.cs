using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCollection : MonoBehaviour
{

    public PlayerController playerCon;

    public GameObject[] GrenadeArray;

    public GameObject GrenadeDum;
    public GameObject GrenadeTel;
    public GameObject GrenadeRif;
    public GameObject GrenadeGey;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            if (this.gameObject.tag == "GP1")
            {
                playerCon.grenadeTypePrefabs = new GameObject[GrenadeArray.Length];
                playerCon.grenadeTypePrefabs[0] = GrenadeDum;
                playerCon.grenadeTypePrefabs[1] = GrenadeTel;
                
            }
            if (this.gameObject.tag == "GP2")
            {
                playerCon.grenadeTypePrefabs = new GameObject[GrenadeArray.Length];
                playerCon.grenadeTypePrefabs[0] = GrenadeDum;
                playerCon.grenadeTypePrefabs[1] = GrenadeTel;
                playerCon.grenadeTypePrefabs[2] = GrenadeGey;
                
            }
            if (this.gameObject.tag == "GP3")
            {
                playerCon.grenadeTypePrefabs = new GameObject[GrenadeArray.Length];
                playerCon.grenadeTypePrefabs[0] = GrenadeDum;
                playerCon.grenadeTypePrefabs[1] = GrenadeTel;
                playerCon.grenadeTypePrefabs[2] = GrenadeGey;
                playerCon.grenadeTypePrefabs[3] = GrenadeRif;

                
            }
            Destroy(this.gameObject);
        }
    }
}
