using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IRespawn
{
    Vector3 m_LocalDirection;
    public float m_Speed;

    float GravityAccel = -10.0f;
    public Vector3 m_Velocity = Vector3.zero;
    Vector3 m_GroundVelocity = Vector3.zero;
    float m_RotationSpeed = 3.14159f;

    float m_MaxGroundMoveSpeed = 15.0f;
    public float GroundCheckStartOffsetY = 0.5f;
    public float CheckForGroundRadius = 0.5f;
    int m_GroundCheckMask;
    Vector3 m_GroundNormal = new Vector3(0, 1.0f, 0);
    public float GroundResolutionOverlap = 0.1f;

    public bool InstantStepUp = false;
    public float StepUpEaseSpeed = 10.0f;
    public float MinAllowedSurfaceAngle = 15.0f;
    float m_CenterHeight;

    float InAirMoveAccel = 10.0f;
    float InAirMaxHorizSpeed = 15.0f;
    float InAirMaxVertSpeed = 15.0f;
    float m_TimeInAir;

    float m_TimeLeftToAllowMidAirJump;
    float m_JumpTimeLeft;
    public float MaxMidAirJumpTime = 0.3f;
    bool m_bIsJumping;
    bool m_AllowJump;
    public float JumpSpeed = 10.0f;
    public float MaxJumpHoldTime = 0.5f;
    public float JumpPushOutOfGroundAmount = 0.5f;

    public PlayerController m_Controller;
    Rigidbody m_RigidBody;
    public GameObject m_Camera;
    CapsuleCollider m_Collider;
    PlayerPowerManager m_PowerManager;

    Vector3 m_SpawnPosition = Vector3.zero;

    


    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<PlayerController>();

        m_RigidBody = GetComponent<Rigidbody>();

        m_Collider = GetComponent<CapsuleCollider>();

        m_PowerManager = GetComponent<PlayerPowerManager>();

        m_bIsJumping = false;
        m_AllowJump = true;

        m_GroundCheckMask = ~LayerMask.GetMask("Player", "Ignore Raycast");
    }

    private void Awake()
    {
        m_SpawnPosition = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        // Currently does nothing

        m_Controller.UpdateCanMouseControl();

        
    }

    void FixedUpdate()
    {
        m_Velocity = m_RigidBody.velocity;

        m_LocalDirection = m_Controller.GetMoveInput();

        m_LocalDirection.Normalize();

        m_bIsJumping = m_Controller.IsJumping();

        CheckOnGroundState();

        Vector3 rotation = m_Controller.GetLookInput();
        rotation.x = 0.0f;
        transform.Rotate(rotation * m_RotationSpeed);

        // Check state
        if (m_MovementState == MovementState.InAir)
        {
            UpdateInAir();
        }
        else if (m_MovementState == MovementState.OnGround)
        {
            UpdateGroundMovement();
        }

    }

    void UpdateGroundMovement()
    {
        if (m_LocalDirection.sqrMagnitude > MathUtils.CompareEpsilon)
        {
            Vector3 localVelocity = m_Velocity; // Not accounting for ground movement

            Vector3 worldDirection = Vector3.zero;

            if (m_LocalDirection.x > 0.001f || m_LocalDirection.x < -0.001f)
            {
                worldDirection += m_Camera.transform.right * m_LocalDirection.x;
            }

            if (m_LocalDirection.z > 0.001f || m_LocalDirection.z < -0.001f)
            {
                worldDirection += m_Camera.transform.forward * m_LocalDirection.z;
            }

            worldDirection.Normalize();

            Debug.DrawLine(transform.position, transform.position + worldDirection * 3.0f, Color.green);

            Vector3 groundTangent = worldDirection - Vector3.Project(worldDirection, m_GroundNormal); // TODO: Calc normal more effectivly later
            groundTangent.Normalize();
            Vector3 moveAccel = groundTangent;

            //The velocity along the movement direction
            Vector3 velAlongMoveDir = Vector3.Project(localVelocity, moveAccel);

            //If we are changing direction, come to a stop first.  This makes the movement more responsive
            //since the stopping happens a lot faster than the acceleration typically allows
            if (Vector3.Dot(velAlongMoveDir, moveAccel) > 0.0f) // if dot product is positive than we are going forward
            {
                //Use a similar method to stopping to ease the movement to just be in the desired move direction
                //This makes turning more responsive
                localVelocity = MathUtils.LerpTo(10.0f, localVelocity, velAlongMoveDir, Time.fixedDeltaTime);
            }
            else //slow down when direction is changed
            {
                localVelocity = MathUtils.LerpTo(10.0f, localVelocity, Vector3.zero, Time.fixedDeltaTime);
            }

            moveAccel *= 10.0f;

            localVelocity += moveAccel;

            localVelocity = Vector3.ClampMagnitude(localVelocity, m_MaxGroundMoveSpeed);

            m_Velocity = localVelocity;

        }
        else
        {
            //Slow to a stop
            m_Velocity = MathUtils.LerpTo(1000.0f, m_Velocity, m_GroundVelocity, Time.deltaTime);
        }

        //Handle jump input
        if (m_bIsJumping)
        {
            ActivateJump();
        }
        else
        {
            m_AllowJump = true;
        }

        //Move the character controller
        ApplyVelocity(m_Velocity);

        //Ease the height up to the step up height
        Vector3 playerCenter = transform.position;

        if (InstantStepUp) //Adjust capsule vertical position instantly, depending on raycast returned surface
        {
            playerCenter.y = m_CenterHeight;
        }
        else
        {
            playerCenter.y = MathUtils.LerpTo(StepUpEaseSpeed, playerCenter.y, m_CenterHeight, Time.deltaTime);
        }

        transform.position = playerCenter;
        transform.position += new Vector3(0, 0.1f, 0);

        //Reset time in air
        m_TimeInAir = 0.0f;
    }


    void UpdateInAir()
    {
        if (m_LocalDirection.sqrMagnitude > MathUtils.CompareEpsilon)
        {

            Vector3 worldDirection = Vector3.zero;

            if (m_LocalDirection.x > 0.001f || m_LocalDirection.x < -0.001f)
            {
                worldDirection += m_Camera.transform.right * m_LocalDirection.x;
            }

            if (m_LocalDirection.z > 0.001f || m_LocalDirection.z < -0.001f)
            {
                worldDirection += m_Camera.transform.forward * m_LocalDirection.z;
            }

            worldDirection.Normalize();

            worldDirection *= InAirMoveAccel;

            m_Velocity += worldDirection * Time.fixedDeltaTime;

        }

        // Clamp velocity
        m_Velocity = MathUtils.HorizontalClamp(m_Velocity, InAirMaxHorizSpeed);

        m_Velocity.y = Mathf.Clamp(m_Velocity.y, -InAirMaxVertSpeed, InAirMaxVertSpeed);

        //Update mid air jump timer and related jump.  This timer is to make jump timing a little more forgiving 
        //by letting you still jump a short time after falling off a ledge.
        if (m_JumpTimeLeft <= 0.0f)
        {
            if (m_TimeLeftToAllowMidAirJump > 0.0f)
            {
                m_TimeLeftToAllowMidAirJump -= Time.fixedDeltaTime;

                if (m_bIsJumping)
                {
                    ActivateJump();
                }
                else
                {
                    m_AllowJump = true;
                }
            }
        }
        else
        {
            m_TimeLeftToAllowMidAirJump = 0.0f;
        }

        //Update gravity and jump height control
        if (m_JumpTimeLeft > 0.0f && m_bIsJumping)
        {
            m_JumpTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            m_Velocity.y += GravityAccel * Time.fixedDeltaTime;

            m_JumpTimeLeft = 0.0f;
        }

        //Move the character controller
        ApplyVelocity(m_Velocity);

        //Increment time in air
        m_TimeInAir += Time.deltaTime;
    }


    void ActivateJump()
    {
        //The allowJump bool is to prevent the player from holding down the jump button to bounce up and down
        //Instead they will have to release the button first.
        if (m_AllowJump)
        {
            //Set the vertical speed to be the jump speed + the ground velocity
            m_Velocity.y = JumpSpeed + m_GroundVelocity.y;

            //This is to ensure that the player wont still be touching the ground after the jump
            transform.position += new Vector3(0.0f, JumpPushOutOfGroundAmount, 0.0f);

            //Set the jump timer
            m_JumpTimeLeft = MaxJumpHoldTime;

            m_AllowJump = false;

        }
    }


    void CheckOnGroundState()
    {
        // Check if on ground or not

        // Sphere cast down

        float halfCapsuleHeight = m_Collider.height * 0.5f;

        Vector3 rayStart = transform.position;
        rayStart.y += GroundCheckStartOffsetY;

        Vector3 rayDirection = Vector3.down;

        float rayDistance = halfCapsuleHeight + GroundCheckStartOffsetY - CheckForGroundRadius * 0.5f;

        RaycastHit[] hitInfos = Physics.SphereCastAll(rayStart, CheckForGroundRadius, rayDirection, rayDistance, m_GroundCheckMask);

        // get closest surface
        RaycastHit groundHitInfo = new RaycastHit();
        bool validGroundFound = false;
        float minGroundDistance = float.MaxValue;

        foreach (RaycastHit hitInfo in hitInfos)
        {
            float surfaceAngle = MathUtils.CalcVerticalAngle(hitInfo.normal);
            if (surfaceAngle < MinAllowedSurfaceAngle || hitInfo.distance <= 0.0f)
            {
                continue;
            }

            if (hitInfo.distance < minGroundDistance)
            {
                minGroundDistance = hitInfo.distance;
                groundHitInfo = hitInfo;
                validGroundFound = true;
            }
        }

        if (!validGroundFound)
        {
            if (m_MovementState != MovementState.Disable)
            {
                SetMovementState(MovementState.InAir);
            }
            return;
        }

        //Step up
        Vector3 bottomAtHitPoint = MathUtils.ProjectToBottomOfCapsule(
          groundHitInfo.point,
          transform.position,
          halfCapsuleHeight * 2.0f,
          CheckForGroundRadius
          );

        float stepUpAmount = groundHitInfo.point.y - bottomAtHitPoint.y;
        m_CenterHeight += stepUpAmount - GroundResolutionOverlap;

        //Setting Ground Normal based on object normal we cought trough rayCast.
        m_GroundNormal = groundHitInfo.normal;

        if (m_MovementState != MovementState.Disable)
        {
            SetMovementState(MovementState.OnGround);
        }
    }

    void ApplyVelocity(Vector3 velocity)
    {
        Vector3 velocityDiff = velocity - m_RigidBody.velocity;

        m_RigidBody.AddForce(velocityDiff, ForceMode.VelocityChange);
    }



    public void Respawn()
    {
        transform.position = m_SpawnPosition;
        m_Velocity = Vector3.zero;

        if (m_PowerManager != null)
        {
            m_PowerManager.m_CurrentPower = m_PowerManager.m_MaxPower;
        }
    }

    enum MovementState
    {
        OnGround,
        InAir,
        Disable
    }

    MovementState m_MovementState;

    void SetMovementState(MovementState newState)
    {
        m_MovementState = newState;
    }
}


