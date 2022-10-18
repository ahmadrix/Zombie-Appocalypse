using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Camera o assign")] 
    public GameObject AimCamera;
    public GameObject AimCanvas;
    public GameObject TPSCamera;
    public GameObject TPSCanvas;
    public Animator animator;

    private void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Fire", true);
            TPSCamera.SetActive(false);
            TPSCanvas.SetActive(false);
            AimCamera.SetActive(true);
            AimCanvas.SetActive(true);
        }
        else
        {
            animator.SetBool("Fire", false);
            TPSCamera.SetActive(true);
            TPSCanvas.SetActive(true);
            AimCamera.SetActive(false);
            AimCanvas.SetActive(false);
        }
    }
}
