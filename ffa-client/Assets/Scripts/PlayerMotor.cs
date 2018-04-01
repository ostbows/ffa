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

    private CharacterController controller;
    private Vector3 lastMoveDirection = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalVelocity = 0.0f;
    private bool canMove = true;
    private bool isJumping = false;
    private bool isAttacking = false;
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

    #region Animator
    private Animator animator;

    private int moveState;
    private int moveStateIdle = 0;
    private int moveStateWalk = 1;
    private int moveStateRun = 2;
    private int moveStateJump = 3;
    private int moveStateLand = 4;

    private int attackState;
    private int attackStateForward = 1;
    #endregion

    public override void OnStartLocalPlayer()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        var cameraMotor = (CameraMotor)Camera.main.GetComponent<CameraMotor>();
        cameraMotor.player = gameObject;
        cameraMotor.SetOffset();
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        isAttacking = animator.GetInteger("attack") != 0;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            targetDistance = Vector3.Distance(transform.position, targetPosition);
            maxSpeed = targetDistance > minRunDistance ? runMaxSpeed : walkMaxSpeed;

            if (IsMinWalkDistance() && !isAttacking)
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

                moveState = speed > walkMaxSpeed ? moveStateRun : moveStateWalk;

                if (Input.GetMouseButtonDown(0))
                {
                    canMove = false;
                    isAttacking = true;
                    speed *= 1.7f;

                    animator.SetInteger("attack", attackStateForward);
                }
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

        animator.SetInteger("movement", moveState);
        controller.Move(moveDirection * Time.deltaTime);
    }

    private bool IsMinWalkDistance()
    {
        return targetDistance >= minWalkDistance;
    }
}
