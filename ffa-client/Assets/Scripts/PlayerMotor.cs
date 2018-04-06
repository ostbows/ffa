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
    OnForwardAttack onForwardAttackBehaviour;

    int moveState;
    int lastMoveState;
    int moveStateIdle = 0;
    int moveStateWalk = 1;
    int moveStateRun = 2;
    int moveStateJump = 3;
    int moveStateLand = 4;

    int attackStateForward = 1;
    #endregion

    public override void OnStartLocalPlayer()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        onForwardAttackBehaviour = animator.GetBehaviour<OnForwardAttack>();
        onForwardAttackBehaviour.playerMotor = this;

        var cameraMotor = (CameraMotor)Camera.main.GetComponent<CameraMotor>();
        cameraMotor.player = gameObject;
        cameraMotor.SetOffset();
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

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

        SetMoveState();
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
        if (Input.GetMouseButtonDown(0) && moveState == moveStateRun)
        {
            canMove = false;
            isAttacking = true;
            speed *= 1.7f;

            animator.SetInteger("attack", attackStateForward);
        }
    }

    void SetMoveState()
    {
        if (lastMoveState != moveState)
        {
            lastMoveState = moveState;
            animator.SetInteger("movement", moveState);
        }
    }

    bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }

    public void Move(Vector3 direction)
    {
        if (!isServer) return;

        RpcMove(direction);
    }

    [ClientRpc]
    void RpcMove(Vector3 direction)
    {
        if (!isLocalPlayer) return;

        controller.Move(direction);
    }
}
