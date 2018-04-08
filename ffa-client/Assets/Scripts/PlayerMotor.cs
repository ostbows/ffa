﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMotor : NetworkBehaviour
{
    #region Controller
    public float speed = 0.0f;
    public float maxSpeed = 0.0f;
    public float walkMaxSpeed = 6.0f;
    public float runMaxSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    CharacterController controller;
    Vector3 lastMoveDirection = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    float verticalVelocity = 0.0f;
    bool canMove = true;
    bool isJumping = false;
    bool isAttacking = false;
    #endregion

    #region Raycast
    Ray ray;
    RaycastHit hit;
    Vector3 targetPosition;
    float targetDistance;

    public LayerMask layerMask;
    public float minWalkDistance = 0.1f;
    public float minRunDistance = 5.0f;
    #endregion

    #region Animator
    Animator animator;

    int moveState;
    int lastMoveState;
    int moveStateIdle = 0;
    int moveStateWalk = 1;
    int moveStateRun = 2;
    int moveStateJump = 3;
    int moveStateLand = 4;

    int attackStateForward = 1;
    #endregion

    PlayerHitbox playerHitbox;

    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        playerHitbox = GetComponent<PlayerHitbox>();
        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();
        animator.GetBehaviour<OnForwardAttack>().playerMotor = this;
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            targetDistance = Vector3.Distance(transform.position, targetPosition);
            maxSpeed = targetDistance > minRunDistance ? runMaxSpeed : walkMaxSpeed;

            RotatePlayerCheck();
        }

        if (controller.isGrounded)
        {
            if (Input.GetMouseButton(1) && IsMinWalkDistance() && canMove)
            {
                lastMoveDirection = transform.TransformDirection(Vector3.forward);
                speed += 10 * Time.deltaTime;

                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }

                moveState = speed > walkMaxSpeed ? moveStateRun : moveStateWalk;

                ForwardAttackCheck();
            }
            else
            {
                canMove = false;
                speed -= 20 * Time.deltaTime;

                if (speed < 0)
                {
                    canMove = true;
                    speed = 0;
                }

                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    moveState = moveStateIdle;
                }
            }

            moveDirection = lastMoveDirection * speed;

            if (Input.GetButtonDown("Jump") && canMove && !isAttacking)
            {
                canMove = false;
                isJumping = true;
                verticalVelocity = jumpSpeed;
                moveState = moveStateJump;
            }
            else
            {
                verticalVelocity = -controller.stepOffset / Time.deltaTime;

                if (isJumping)
                {
                    isJumping = false;
                    moveState = moveStateLand;
                }
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        if (lastMoveState != moveState)
        {
            lastMoveState = moveState;
            animator.SetInteger("movement", moveState);
        }

        controller.Move(moveDirection * Time.deltaTime);
    }

    void RotatePlayerCheck()
    {
        if (IsMinWalkDistance() && !isAttacking)
        {
            transform.LookAt(targetPosition);
        }
    }

    void ForwardAttackCheck()
    {
        if (Input.GetMouseButtonDown(0) && 
            moveState == moveStateRun && !isAttacking)
        {
            isAttacking = true;
            canMove = false;
            speed *= 1.2f;

            StartCoroutine("ForwardAttack");
        }
    }

    private IEnumerator ForwardAttack()
    {
        animator.SetInteger("attack", attackStateForward);

        yield return new WaitForSeconds(0.4f);

        playerHitbox.CmdSpawnHitbox(0, 5.0f, 0.2f);
    }

    bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }

    public void ResetIsAttacking()
    {
        if (isLocalPlayer)
        {
            isAttacking = false;
            animator.SetInteger("attack", 0);
        }
    }

    public void Knockback(Vector3 direction)
    {
        canMove = false;
        lastMoveDirection = direction;
        speed = runMaxSpeed;
    }
}
