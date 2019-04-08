using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCollectables : MonoBehaviour
{
    public int totalCollected;
    public Text CoinText;

    public void IncrementCollected() {
        //increment total by 1 nwhen collected
        totalCollected++;
        // change the text when the coins are collected
        IncrementCounter();
    }

    public void IncrementCounter()
    {

        CoinText.text = ("Coins: " + totalCollected);

    }
}
