using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RifleController : MonoBehaviour
{
    [Header("Rifle Things")] 
    public new Camera camera;
    public float giveDamage = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    public float nextTimetoShoot = 0f;
    public PlayerController player;   //reference to the Playe Controller Script
    public Transform hand;
    public Animator rifleanimator;

    [Header("Rifle Ammunation and shooting")]
    public int maximumAmmunation = 32;
    public int mag = 10;
    public int presentAmmunation;
    public bool setReloading = false;
    

    [Header("Rifle Effect")] 
    public ParticleSystem muzzleSpark;
    public GameObject woodEffect;
    public GameObject goreEffect;
    
    [Header("Rifle UI")]
    public TextMeshProUGUI MagazineText;
    public TextMeshProUGUI BulletText;
    
    [Header("Sounds")] 
    public AudioSource rifleAudio;
    public AudioClip gunPickUp;
    public AudioClip gunReload;
    

    private void Awake()
    {
        transform.SetParent(hand);  
        presentAmmunation = maximumAmmunation;
    }

    private void Start()
    {
        //sound
        rifleAudio.PlayOneShot(gunPickUp , 1);
        //rifleanimator = player.animator;
    }

    private void Update()
    {
        if (setReloading)
            return;


        if (rifleanimator != null)
        {

            if (presentAmmunation <= 0)
            {
                rifleAudio.PlayOneShot(gunReload, 1); //gun reload sound
                StartCoroutine(Reload());
                rifleanimator.SetBool("Reloading", true);
                return;
            }



            if (Input.GetButton("Fire1") && Time.time >= nextTimetoShoot)
            {
                rifleanimator.SetBool("Fire", true);
                rifleanimator.SetBool("Idle", false);
                nextTimetoShoot = Time.time + 1f / fireCharge;
                Shoot();
            }
            else if (Input.GetButton("Fire2") && Input.GetKey(KeyCode.W))
            {
                rifleanimator.SetBool("Walk", false);
                rifleanimator.SetBool("FireWalk", true);
                rifleanimator.SetBool("Idle", false);
            }
            else if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
            {
                rifleanimator.SetBool("Idle", false);
                rifleanimator.SetBool("FireWalk", false);
                rifleanimator.SetBool("Walk", false);
                rifleanimator.SetBool("Reloading", false);
            }
            else
            {
                rifleanimator.SetBool("IdleAim", false);
                rifleanimator.SetBool("Idle", true);
                rifleanimator.SetBool("FireWalk", false);
            }
        }
    }

    void Shoot()
    {
        //Ammunation System
        //check for magazine
        if (mag == 0)
        {
            //show ammo out text
        }

        presentAmmunation--;
        if (presentAmmunation == 0)       
        {
            mag--;
        }
        //updating the magazine UI
        
        muzzleSpark.Play();       //rifle muzzle effect
        
        //Raycast bullet hit
        RaycastHit hitInfo;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo, shootingRange)) 
        {
            Debug.Log(hitInfo.transform.name);

            ObjectToHit objectToHit = hitInfo.transform.GetComponent<ObjectToHit>();     //getting an object from the script (which is hit and have the ObjectToHit Script)
            Zombie1 zombie1 = hitInfo.transform.GetComponent<Zombie1>();
                
            if (objectToHit != null)
            {
                objectToHit.ObjectHitDamage(giveDamage);      //Damage the object, thus decreasing its health in ObjectToHit Script
                GameObject impact = Instantiate(woodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));       //Effect to show bullet hit
                Destroy(impact, 2);
            }
            
            if (zombie1 != null)
            {
                zombie1.ZombieHitDamage(giveDamage);      //Damage the Zombie1, thus decreasing its health in Zombie1 Script
                GameObject impactZ = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));       //Effect to show bullet hit
                Destroy(impactZ, 2);
            }
        }
    }

    IEnumerator Reload()
    {
        player.playerSpeed = 0;       //stopping the player speed and sprint when reloading
        player.playerSprint = 0;
        setReloading = true;
        Debug.Log("Reloading......");
        //play reload sound

        yield return new WaitForSeconds(3);  //Reloading Time
        rifleanimator.SetBool("Reloading", false);    //Reload Animation

        presentAmmunation = maximumAmmunation;    
        player.playerSpeed = 1.9f;           //setting the speed and sprint of the player to normal after reloading
        player.playerSprint = 3f;
        setReloading = false;
    }



}
