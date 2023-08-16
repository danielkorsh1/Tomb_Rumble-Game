using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{

    [SerializeField] private float AnimBlendSpeed = 8f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform Camera;
    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottomLimit = 60f;
    [SerializeField] private float MouseSensitivity = 21.9f;
    [SerializeField] private float JumpFactor = 260f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] public float AirResistance = 0.8f;
        private Rigidbody _PlayerRigidbody;
    

        public Animator _Animator;
        private Climb climb;
        private CapsuleCollider capsule;
        private BoxCollider boxCollider;
        public GameObject slowRunAudio;
        public GameObject fastRunAudio;
        public GameObject crouchWalkAudio;
        public GameObject Escape;

        public bool _grounded = false;
        private bool _HasAnimator;
        private bool boxIsColliding;
        private float groundCheckDis = 0.5f;


        
        private int _xvelHash;
        private int _yvelHash;
        private int _jumpHash;
        public int _groundHash;
        public int _fallingHash;
        private int _zVelHash;
        private int _crouchHash;
        public int _climbHash;
        private float crouchYScale;
        private float startHeightYScale;
        private float crouchHightYScale;
        private float startCenterYScale;
        private float _xRotation; 
        private InputManager _InputManager;
        public float _walkSpeed = 1;
        public float _runSpeed = 1;
        private Vector2 _currentInputVector;
        private Vector2 _smoothInputVelocity;
        [SerializeField] private float _smoothInputSpeed = .2f;

        private float time = 0;
        private float downTime = 1.5f;




    void Start()
    {
        _HasAnimator = TryGetComponent<Animator>(out _Animator);
        _PlayerRigidbody = GetComponent<Rigidbody>();
        _InputManager = GetComponent<InputManager>();

        capsule = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _xvelHash = Animator.StringToHash("X_Velocity");
        _yvelHash = Animator.StringToHash("Y_Velocity");
        _jumpHash = Animator.StringToHash("Jump");
        _groundHash = Animator.StringToHash("Grounded");
        _fallingHash = Animator.StringToHash("Falling");
        _zVelHash = Animator.StringToHash("Z_Velocity");
        _crouchHash = Animator.StringToHash("Crouch");
        _climbHash = Animator.StringToHash("Climb");

        startCenterYScale = capsule.center.y;
        crouchYScale = startCenterYScale / 2;

        startHeightYScale = capsule.height;
        crouchHightYScale = startHeightYScale / 2;

        climb = GetComponent<Climb>();
        boxCollider = GetComponent<BoxCollider>();

       
    }
    private void Update()
    {
         
    }


    private void FixedUpdate()
        {

            Move();
            HandleJump();
            SampleGround();
            HandleCrouch();
            Esc();

        }

    private void LateUpdate()
    {
        CameraMovement();

    }

    

     private void Move()
    {
        if (!_HasAnimator) return;


        float targetSpeed = _InputManager.Run ? _runSpeed : _walkSpeed;

        if (_InputManager.Crouch) targetSpeed = 1.5f;
        if (_InputManager.Move == Vector2.zero)
        { 
            targetSpeed = 0;
            slowRunAudio.SetActive(false);
            fastRunAudio.SetActive(false);
            crouchWalkAudio.SetActive(false);
            
        }
        if (!climb.getOnLadder())
        {
            
            if (_grounded)
            {

                time += Time.deltaTime;
                _currentInputVector = Vector2.SmoothDamp(_currentInputVector, _InputManager.Move, ref _smoothInputVelocity, _smoothInputSpeed);

                _currentInputVector.x = Mathf.Lerp(_currentInputVector.x, _InputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentInputVector.y = Mathf.Lerp(_currentInputVector.y, _InputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);


                var xVelDiffrence = _currentInputVector.x - _PlayerRigidbody.velocity.x;
                var zVelDiffrence = _currentInputVector.y - _PlayerRigidbody.velocity.z;
                var yVelDiffrence = 0f;




                _PlayerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDiffrence, yVelDiffrence, zVelDiffrence)), ForceMode.VelocityChange);
                if(_InputManager.Run)
                {
                    fastRunAudio.SetActive(true);
                    slowRunAudio.SetActive(false);
                }
                else if (_InputManager.Crouch)
                {
                    crouchWalkAudio.SetActive(true);
                    fastRunAudio.SetActive(false);
                    slowRunAudio.SetActive(false);
                }
                else 
                {
                    fastRunAudio.SetActive(false);
                    slowRunAudio.SetActive(true);
                }
                


            }


            else
            {

                time = 0;
                _PlayerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentInputVector.x * AirResistance, 0, _currentInputVector.y * AirResistance)), ForceMode.VelocityChange);
            }
        }

        _Animator.SetFloat(_xvelHash, _currentInputVector.x);
        _Animator.SetFloat(_yvelHash, _currentInputVector.y);
    }

    private void CameraMovement()
    {
        if (!_HasAnimator) return;

        var Mouse_X = _InputManager.Look.x;
        var Mouse_Y = _InputManager.Look.y;
        Camera.position = CameraRoot.position;

        _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
        _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

        Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        _PlayerRigidbody.MoveRotation(_PlayerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        //transform.Rotate(Vector3.up, Mouse_X * MouseSensitivity * Time.smoothDeltaTime);
    }

    private void Crouch()
    { 
        capsule.height = crouchHightYScale;
        capsule.center = new Vector3(0, crouchYScale, 0);
    }

    private void StopCrouch()
    {
        capsule.height = startHeightYScale;
        capsule.center = new Vector3(0, startCenterYScale, 0);
    }



    private void HandleCrouch() 
    {
        if (_InputManager.Crouch) Crouch();

        else StopCrouch();

        _Animator.SetBool(_crouchHash, _InputManager.Crouch);

    }
        

    private void HandleJump()
    {
        if (!_HasAnimator) return;
        if (!_InputManager.Jump) return;
        if (climb.ladderFront) return;
        else
        {

            // Only call JumpAddForce if the player is grounded, the jump timer has expired,
            // and the player is in the base state
            if (_grounded && JumpCoolDown())
            {
                _PlayerRigidbody.useGravity = true;
                time = 0;
                JumpAddForce();
                _Animator.SetTrigger(_jumpHash);
                Debug.Log("lol");


            }
        }
     
    }




    //jumping logic
    private void JumpAddForce()
    {
        // Store the result of Vector3.up in a local variable
        var up = Vector3.up;

        // Use the Rigidbody.AddForce function to apply an impulse force in the upward direction
        _PlayerRigidbody.AddForce(up * JumpFactor, ForceMode.Force);

        // Reset the jumping trigger after each cycle
        _Animator.ResetTrigger(_jumpHash);
    }


    private void OnCollisionEnter(Collision collision)
    {
        boxIsColliding = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        boxIsColliding = false;
    }


    //Landing Logic. cast an array from the midlle of the player to downward an ee whendoes the player collide with the ground.
    private void SampleGround()
    {
        if (!_HasAnimator) return;

        // Use a pre-allocated array of RaycastHit objects to avoid creating new objects on every call
        RaycastHit[] hitInfos = new RaycastHit[1];
        Debug.DrawRay(transform.position, -transform.up, Color.green);
        // Cast a ray from the player's position downwards to check for the ground
        if (Physics.RaycastNonAlloc(transform.position, -transform.up, hitInfos, groundCheckDis) > 0)
        {
            // The player is grounded
            _grounded = true;
        }
        else if(boxIsColliding)
        {
            // The player is falling
            _grounded = true;
            
        }
        else
        {
             _grounded = false;
            _Animator.SetFloat(_zVelHash, _PlayerRigidbody.velocity.y);
        }

        // Set the grounding state in the animator
        SetAnimationGrounding();
    }

    //set animation patameter.
    private void SetAnimationGrounding()
    {
        _Animator.SetBool(_fallingHash, !_grounded);
        _Animator.SetBool(_groundHash, _grounded);
    }
    private bool JumpCoolDown()
    {
        if (time >= downTime)
        {
            return true;
        }
        return false;
    }
    private void Esc()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("lolllol");
            Escape.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
       
    }


}




