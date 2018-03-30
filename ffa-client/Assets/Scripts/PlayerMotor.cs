﻿using UnityEngine;

public class PlayerMotor : MonoBehaviour {

    #region Player Controller
    public float speed = 0.0f;
    public float maxSpeed = 0.0f;
    public float walkMaxSpeed = 6.0f;
    public float runMaxSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private CharacterController controller;
    private Vector3 lastMoveDirection = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private bool canMove = true;
    #endregion

    #region Raycast
    private Ray ray;
    private RaycastHit hit;
    private Vector3 targetPosition;
    private float targetDistance;

    public LayerMask layerMask;
    public float minWalkDistance = 0.1f;
    public float minRunDistance = 5.0f;
    #endregion

    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            targetDistance = Vector3.Distance(transform.position, targetPosition);
            maxSpeed = targetDistance > minRunDistance ? runMaxSpeed : walkMaxSpeed;

            if (IsMinWalkDistance())
            {
                transform.LookAt(targetPosition);
            }
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

                animator.SetInteger("movement", speed > walkMaxSpeed ? 2 : 1);
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

                animator.SetInteger("movement", 0);
            }

            moveDirection = lastMoveDirection * speed;

            if (Input.GetButtonDown("Jump"))
            {
                canMove = false;
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = -controller.stepOffset / Time.deltaTime;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveDirection * Time.deltaTime);
    }

    private bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }
}
