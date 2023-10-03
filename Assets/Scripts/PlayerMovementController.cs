
using System;
using Unity.VisualScripting;
using UnityEngine;
namespace liou
{
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("移動參數")]
        [SerializeField] private float moveSpeed = 3f;

        public float MoveSpeed => moveSpeed;

        private Vector3 deltaPos;
        public Vector3 DeltaPos => deltaPos;

        [SerializeField] private float RotationSpeed = 5f;
        private float deltaVertical, deltaHorizontal;
        private Quaternion deltaRot;

        [SerializeField] private float JumpForce = 5f;
        private float groundCheckDistance = 0.1f;

        [Header("雷射設置")]
        [SerializeField] private Vector3 GetPoint;
        private Ray ScreenPointToRay;

        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private Camera cam;

        public Vector2 deltaInput;
        public Vector2 DeltaInput => deltaInput;

        [Header("攻擊1參數")]
        [SerializeField] private float Fire1Interval = 0.1f;
        [SerializeField] private float Fire1Damage = 1.0f;
        [SerializeField] private float Fire1Force = 1.0f;
        private float fire1Cooldown = 0f;

        private PlayerHealth playerHealth;

        private EventHandler jumpReceived;
        public event EventHandler JumpReceived
        {
            add { jumpReceived += value; }
            remove { jumpReceived -= value; }
        }

        private EventHandler<FireEventArgs> fire1Received, fire2Received;
        public event EventHandler<FireEventArgs> Fire1Received
        {
            add { fire1Received += value; }
            remove { fire1Received -= value; }
        }

        private void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            playerHealth.DeadReceived += OnDead;
            playerHealth.RevivalReceived += OnRevival;
            cam = Camera.main;
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
        }
        private void OnDead(object sender, EventArgs e)
        {
            enabled = false;
        }

        private void OnRevival(object sender, EventArgs e)
        {
            enabled = true;
        }

        private void FixedUpdate()
        {
            if (deltaPos.magnitude != 0) rb.MovePosition(transform.position + deltaPos);
            if (deltaRot.eulerAngles.magnitude != 0) rb.MoveRotation(deltaRot);
        }
        private void Update()
        {
            OnMove();
            OnRotation();
            OnJump();
            OnFire();
        }
        private void OnMove()
        {
            deltaVertical = deltaInput.y = Input.GetAxis("Vertical");
            deltaHorizontal = deltaInput.x = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(deltaHorizontal, 0f, deltaVertical);
            Quaternion noTiltRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            deltaPos = noTiltRotation * movement * MoveSpeed * Time.deltaTime;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawRay(ScreenPointToRay.origin, ScreenPointToRay.direction * 100f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(GetPoint, 0.5f);
        }

        private void OnRotation()
        {
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition); ScreenPointToRay = cameraRay;
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;
            if (groundPlane.Raycast(cameraRay, out rayLength))
            {
                Vector3 targetPoint = cameraRay.GetPoint(rayLength);
                GetPoint = targetPoint;
                Quaternion targetRot = Quaternion.LookRotation(targetPoint, transform.position);
                targetRot = targetRot.normalized;
                targetRot.x = targetRot.z = 0;
                deltaRot = Quaternion.Slerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            }
        }
        void OnJump()
        {
            if(Input.GetButtonDown("Jump") && IsGrounded()) {
               jumpReceived?.Invoke(this, EventArgs.Empty);
                rb.AddForce(transform.up * JumpForce,ForceMode.Impulse);
            }
        }
        public bool IsGrounded()
        {
            
            if (Physics.Raycast(capsuleCollider.bounds.center, Vector3.down, capsuleCollider.bounds.extents.y + groundCheckDistance))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        void OnFire()
        {
            if(Input.GetButtonDown("Fire1") && fire1Cooldown <= 0f)
            {
                FireEventArgs fireEventArgs = new FireEventArgs(Fire1Damage, Fire1Force);
                fire1Received?.Invoke(this, fireEventArgs);
                fire1Cooldown = Fire1Interval;
            }
            if(fire1Cooldown > 0f) { fire1Cooldown -= Time.deltaTime; } 
        }
       
        private void OnDestroy()
        {
            jumpReceived = null;
        }
    }
}