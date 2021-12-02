using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;

    bool jump = false;

    bool crouch = false;

    bool walk = false;

    public Animator animator;

    public ParticleSystem dust;

    // Update is called once per frame
    void Update()
    {
        horizontalMove =  Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Walk") || Input.GetKeyDown(KeyCode.LeftShift))
        {
            walk = true;
        }
        else if (Input.GetButtonUp("Walk") || Input.GetKeyUp(KeyCode.LeftShift))
        {
            walk = false;
        }


    }

    public void OnLanding()
    {
        CreateDust();
        animator.SetBool("isJumping", false);
        animator.SetBool("jumpedAlready", false);
        animator.SetBool("isFloating", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("isCrouching", isCrouching);
    }

    public void OnWalking(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    private void FixedUpdate()
    {
        jump = false;
        // MOVE
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, walk);
    }

    void CreateDust()
    {
        dust.Play();
    }


}
