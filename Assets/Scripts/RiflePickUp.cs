using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflePickUp : MonoBehaviour
{
    [Header("Rifle")] 
    public GameObject playerRifle;
    public GameObject RifletoPick;

    [Header("Rifle Assign Things")] 
    public PlayerController player;
    public float radius = 2.5f;
    
    

    private void Awake()
    {
        playerRifle.SetActive(false);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                playerRifle.SetActive(true);
                RifletoPick.SetActive(false);
                
                //objective complete
            }
        }
    }
}
