using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMotor : NetworkBehaviour
{
    #region Controller
    public CharacterController controller;

    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float verticalVelocity;

    public float walkMaxSpeed = 6.0f;
    public float runMaxSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    Vector3 lastMoveDirection;
    Vector3 moveDirection;
    
    bool canMove = true;
    bool isJumping = false;
    bool isAttacking = false;
    #endregion

    #region Raycast
    public LayerMask layerMask;

    Ray ray;
    RaycastHit hit;
    Vector3 targetPosition;
    float targetDistance;

    const float minWalkDistance = 0.1f;
    const float minRunDistance = 5.0f;
    #endregion

    #region Animator
    public Animator animator;

    int moveState;
    int lastMoveState;
    const int moveStateIdle = 0;
    const int moveStateWalk = 1;
    const int moveStateRun = 2;
    const int moveStateJump = 3;
    const int moveStateLand = 4;

    const int attackStateForward = 1;
    #endregion

    public PlayerHitbox playerHitbox;

    void Start()
    {
        var onForwardAttackBehaviour = animator.GetBehaviour<OnForwardAttack>();

        if (!isLocalPlayer)
        {
            Destroy(onForwardAttackBehaviour);
            Destroy(this);

            return;
        }

        onForwardAttackBehaviour.playerMotor = this;
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
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            playerHitbox.CmdSpawnHitbox(0, 5.0f, 0.5f, 0.25f);

            isAttacking = true;
            canMove = false;
            speed *= 1.2f;

            animator.SetInteger("attack", attackStateForward);
        }
    }

    bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
        animator.SetInteger("attack", 0);
    }

    public void Knockback(Vector3 direction)
    {
        canMove = false;
        lastMoveDirection = direction;
        speed = runMaxSpeed;
    }
}
