using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random; 

public class Zombie1 : MonoBehaviour
{
    [Header("Zombie Health and Damage")] 
    public float giveDamage = 5f;
    private float zombieHealth = 100f;
    public float presentZombieHealth;
    
    
    [Header("Zombie Things")] 
    public NavMeshAgent zombieAgent;
    public Transform lookPoint;
    public LayerMask playerLayer;
    public Camera attackingRaaycastArea;

    [Header("Zombie Attacking")] 
    public float timeBtwAttack;
    private bool previousAttack;

    [Header("Zomie Guarding")] 
    public Transform playerBody;
    public GameObject[] walkPoints;
    private int currentZombiePosition = 0;
    public float zombieSpeed;
    private float walkingPointRadius = 2;

    [Header("Zombie States")] 
    public float visionRadius;
    public float attackingRadius;
    public bool playerInVisionRadius;
    public bool playerInAttackingRadius;

    [Header("Zombie Animations")] 
    public Animator animator;
    
    [Header("Sounds")] 
    public AudioSource ZombieAudio;
    public AudioClip zombieDie;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        presentZombieHealth = zombieHealth;
    }

    private void Update()
    {
        playerInVisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);      //Checks if player in vision Radius
        playerInAttackingRadius = Physics.CheckSphere(transform.position, attackingRadius, playerLayer);      //Checks if player in attacking Radius

        if (!playerInVisionRadius && !playerInAttackingRadius)
        {
            Guard();
        }
        else if (playerInVisionRadius && !playerInAttackingRadius)
        {
            PursuePlayer();
        }
        else if (playerInVisionRadius && playerInAttackingRadius)
        {
            AttackPlayer();
        }
        
        
        
    }

    void Guard()
    {
        zombieSpeed = 1;
        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position , transform.position) < walkingPointRadius)
        {
            currentZombiePosition = Random.Range(0, walkPoints.Length);
            if (currentZombiePosition >= walkPoints.Length)
            {
                currentZombiePosition = 0;
            }
        }
        
        transform.position = Vector3.MoveTowards(transform.position , walkPoints[currentZombiePosition].transform.position, Time.deltaTime * zombieSpeed);
        //change zombie facing to Walk Point
        transform.LookAt(walkPoints[currentZombiePosition].transform.position);
    }

    void PursuePlayer()
    {
        zombieSpeed = 1;
        
        //animations

        zombieAgent.SetDestination(playerBody.position);  //zombie will follow the player
    }

    void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(lookPoint);

        if (!previousAttack)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(attackingRaaycastArea.transform.position, attackingRaaycastArea.transform.forward,
                    out hitInfo, attackingRadius))
            {
                Debug.Log("Attacking" + hitInfo.transform.name );

                PlayerController playerBody = hitInfo.transform.GetComponent<PlayerController>();    //getting the playerController script if the hitInfo has the playerController Script, means it is the player

                if (playerBody != null)
                {
                    animator.SetBool("ZombieAttack", true);
                    playerBody.PlayerHitDamage(giveDamage);
                }
            }

            previousAttack = true;
            Invoke(nameof(ActiveAttacking), timeBtwAttack);    //zombie attack the player after given time
        }
    }

    void ActiveAttacking()
    {
        previousAttack = false;
    }

    public void ZombieHitDamage(float takeDamage)
    {
        presentZombieHealth -= takeDamage;
        if (presentZombieHealth == 0)
        {
            ZombieDie();
        }
    }

    void ZombieDie()
    {
        
        animator.SetBool("ZombieDie", true);
        zombieAgent.SetDestination(transform.position);     //stopping the zombie where he dies
        zombieSpeed = 0;
        attackingRadius = 0;
        visionRadius = 0;
        playerInAttackingRadius = false;
        playerInVisionRadius = false;
        
        //sound
        ZombieAudio.PlayOneShot(zombieDie , 1);
        
        Object.Destroy(gameObject, 5f);
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.zombieKilled = player.zombieKilled + 1;
        player.zombieLeft = player.zombieLeft - 1;
    }
}
