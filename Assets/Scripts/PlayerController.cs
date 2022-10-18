using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")] 
    public float playerSpeed = 1.9f;
    public float playerSprint = 3f;
    public float mouseSensitivity = 5f;     //mouse speed

    [Header("Player Health")] 
    private float playerHealth = 100f;
    public float presentHealth;
    public TextMeshProUGUI playerHealthText;
    
    [Header("Player Animator and Gravity")]
    public CharacterController playerController;
    public float gravity = -9.81f;
    public Animator animator;
    public bool forwardPress;
    public bool isWalking;
    public bool runPress;
    public bool isRunning;
    public bool jump;


    [Header("Player Jumping and Velocity")]
    public float turnCalmTime = 0.1f;
    public float turnCalmVelocity;
    public float jumpRange = 1f;
    private Vector3 velocity;
    public Transform surfaceCheck;
    private bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;

    [Header("Zombie Killed By Player or Vice Versa")] 
    public float zombieKilled;
    public TextMeshProUGUI zombieKilledText;
    public float zombieLeft;
    public TextMeshProUGUI zombieLeftText;
    public GameObject allZombiesKilled;
    public GameObject playerKilled;

    [Header("Sounds")] 
    public AudioSource playerAudio;
    public AudioClip walk;


    public RifleController rifleController;
    public TextMeshProUGUI MagazineText;
    public TextMeshProUGUI BulletText;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        presentHealth = playerHealth;
        zombieKilled = 0;
        zombieLeft = 10;
    }

    private void Update()
    {
        //Gravity 
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);   //checks weather the player is on surface on not

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
        //
        
        //Health Text
        playerHealthText.text = "Health: " + presentHealth;
        
        //Animations
        forwardPress = Input.GetKey(KeyCode.W);
        isWalking = animator.GetBool("Walk");

        runPress = Input.GetKey(KeyCode.LeftShift);
        isRunning = animator.GetBool("Running");

        //if player is not walking and pressing w key
        if(!isWalking && forwardPress)
        {
            //then set the isWalking bool to be true 
            animator.SetBool("Walk" , true);
            //Walk Audio
            playerAudio.Stop ();
            playerAudio.loop = true;
            playerAudio.clip = walk;
            playerAudio.volume = 0.2f;
            playerAudio.Play();
            
        }

        //if player is walking and not pressing w key
        if(isWalking && !forwardPress && !jump)
        {
            playerAudio.Stop();
            //then set the isWalking bool to be false
            animator.SetBool("Walk" , false);
        }

        //if player is not running and (walking and press Left shift)
        if(!isRunning && (runPress && forwardPress))
        {
            //Run Audio
            playerAudio.Stop ();
            playerAudio.loop = true;
            playerAudio.clip = walk;
            playerAudio.volume = 0.2f;
            playerAudio.Play();
            //then set the isRunning bool to be true
            animator.SetBool("Running" , true);
        }

        //if the player is running and (not pressing left shift or stops walking)
        if(isRunning && (!runPress || !forwardPress) && !jump)
        {
            playerAudio.Stop();
            animator.SetBool("Running" , false);
        }
        


        PlayerMove();
        Jump();
        Sprint();
        
        
        
        
        Debug.Log("Zombie's Killed : " + zombieKilled);
        zombieKilledText.text = "Zombie's Killed: " + zombieKilled;      //zombies killed text
        zombieLeftText.text = "Zombie's Left: " + zombieLeft;          //zombie's Left text

        if (zombieKilled == 10)
        {
            allZombiesKilled.SetActive(true);
        }

        int presentAmmunation = rifleController.presentAmmunation;
        int mag = rifleController.mag;
        
        BulletText.text = "" + presentAmmunation;
        MagazineText.text = "" + mag;

    }


    void PlayerMove()
    {
        //Walking Inputs
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X") * 5 * Time.deltaTime;
        //transform.Rotate(Vector3.up * (Time.deltaTime * mouseX * 30));
        
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (direction.magnitude > 0.1f)
        {
            transform.Rotate(Vector3.up * (Time.deltaTime * horizontalInput * 100));
            transform.Translate(Vector3.forward * (Time.deltaTime * verticalInput * playerSpeed));
        }
        
    }

    void Jump()
    {
        if (Input.GetButton("Jump") && onSurface)
        {
            jump = true;
            animator.SetBool("Idle" , false);
            animator.SetTrigger("Jump");
            
            velocity.y = Mathf.Sqrt(jumpRange * -2f * gravity);
        } 
        else
        {
            jump = false;
            animator.SetBool("Idle" , true);
            animator.ResetTrigger("Jump");
        }
    }

    void Sprint()
    {
        if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            float verticalInput = Input.GetAxis("Vertical");      
            float horizontalInput = Input.GetAxis("Horizontal");

            Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            if (direction.magnitude > 0.1f)
            {
                transform.Rotate(Vector3.up * (Time.deltaTime * horizontalInput * 100));
                transform.Translate(Vector3.forward * (Time.deltaTime * verticalInput * playerSprint));
            }
            
        }
    }

    public void PlayerHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;
        if (presentHealth <= 0)
        {
            playerKilled.SetActive(true);
            PlayerDie();
        }
    }

    void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;
        Object.Destroy(gameObject,1f);
    }
}
