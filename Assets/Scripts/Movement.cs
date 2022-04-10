using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private bool usePhysics = true;

    private float jumpSpeed = 200;

    private bool canJump;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Controls _controls;
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsRunningJumping = Animator.StringToHash("isRunningJumping");

    private void Awake()
    {
        _controls = new Controls();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        speed = walkSpeed;
    }

    private void Update()
    {
        if (!_controls.Player.Move.IsPressed()) {
            return;
        }

        Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
        Vector3 target = HandleInput(input);
        RotateCharacter(target);
        
    }

    private void FixedUpdate()
    {
        if (!usePhysics)
        {
            return;
        }

        else{
            speed = 10;
        }
        
        if (_controls.Player.Run.IsPressed()){
            _animator.SetBool(IsRunning, true);
            speed = runSpeed;
            CameraShake.Instance.ShakeCam(10f, 0.06f, 1f);
        }
        else{
            _animator.SetBool(IsRunning, false);
            speed = walkSpeed;
        }

        if (_controls.Player.Jump.IsPressed() & canJump){
            if (!_animator.GetBool(IsJumping))
            {
                _animator.SetBool(IsJumping, false);
            }
            _rb.AddForce(Vector3.up * jumpSpeed);
        }
        
        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        
        else
        {
            _animator.SetBool(IsWalking, false);
        }
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        
        return transform.position + direction * speed * Time.deltaTime;
    }

    private void Move(Vector3 target)
    {
        _rb.MovePosition(target);
    }
    void RotateCharacter(Vector3 target)
    {
        Vector3 lookAt = target - transform.position;
        lookAt.y = 0;
        Quaternion toRotation = Quaternion.LookRotation(lookAt);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            canJump = true;
            _animator.SetBool(IsJumping, false);
            _animator.SetBool(IsRunningJumping, false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            canJump = false;
            // if (_animator.GetBool(IsRunning)){
            //     _animator.SetBool(IsRunningJumping, true);
            //     _animator.SetBool(IsRunning, false);
            // }
            // else{
            //    _animator.SetBool(IsJumping,true); 
            // }
            _animator.SetBool(IsJumping,true); 
            
        }
    }

    private void MovePhysics(Vector3 target)
    {
        _rb.MovePosition(target); 
    }

    
}
