﻿using UnityEngine;
using UnityEngine.Networking;

class AnimatorHash
{
    public int movement;
    public int forwardAttack;
    public int hurt;
}

class MoveState
{
    public int idle = 0;
    public int walk = 1;
    public int run = 2;
    public int jump = 3;
}

public class PlayerMotor : NetworkBehaviour
{
    #region Controller
    [Header("Controller")]
    public CharacterController controller;
    public float walkMaxSpeed = 6.0f;
    public float runMaxSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float verticalVelocity;

    Vector3 lastMoveDirection;
    Vector3 moveDirection;
    bool canMove = true;
    bool isJumping = false;
    bool isAttacking = false;
    #endregion

    #region Raycast
    [Header("Raycast")]
    public LayerMask layerMask;
    public float rotationSpeed = 1000.0f;

    Ray ray;
    RaycastHit hit;
    Vector3 targetPosition;
    float targetDistance;

    const float minWalkDistance = 0.1f;
    const float minRunDistance = 5.0f;
    #endregion

    #region Animator
    [Header("Animator")]
    public NetworkAnimator networkAnimator;

    Animator animator;
    AnimatorHash animatorHash;
    MoveState moveStates;
    int moveState;
    #endregion

    void Awake()
    {
        animator = networkAnimator.animator;
        animator.GetBehaviour<OnForwardAttack>().playerMotor = this;

        moveStates = new MoveState();
        animatorHash = new AnimatorHash
        {
            movement = Animator.StringToHash("movement"),
            forwardAttack = Animator.StringToHash("forwardAttack"),
            hurt = Animator.StringToHash("hurt")
        };
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        if (isServer && isLocalPlayer)
        {
            gameObject.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        }
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

                moveState = speed > walkMaxSpeed ? moveStates.run : moveStates.walk;

                ForwardAttackCheck();
            }
            else
            {
                canMove = false;
                speed -= 20 * Time.deltaTime;

                if (speed <= 0)
                {
                    canMove = true;
                    speed = 0;
                }

                if (!isJumping && !isAttacking)
                {
                    moveState = moveStates.idle;
                }
            }

            moveDirection = lastMoveDirection * speed;

            if (Input.GetButtonDown("Jump") && canMove && !isAttacking)
            {
                canMove = false;
                isJumping = true;
                verticalVelocity = jumpSpeed;
                moveState = moveStates.jump;
            }
            else
            {
                verticalVelocity = -controller.stepOffset / Time.deltaTime;

                if (isJumping)
                {
                    isJumping = false;
                    moveState = moveStates.idle;
                }
            }

            animator.SetInteger(animatorHash.movement, moveState);
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void RotatePlayerCheck()
    {
        if (IsMinWalkDistance() && !isAttacking)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void ForwardAttackCheck()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            networkAnimator.SetTrigger(animatorHash.forwardAttack);

            isAttacking = true;
            canMove = false;
            speed *= 1.2f;
        }
    }

    bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

    public void Knockback(Vector3 direction)
    {
        networkAnimator.SetTrigger(animatorHash.hurt);

        canMove = false;
        lastMoveDirection = direction;
        speed = runMaxSpeed;
    }
}
