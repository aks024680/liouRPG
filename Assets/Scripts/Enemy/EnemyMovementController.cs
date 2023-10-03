
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace liou
{
    public class EnemyMovementController : MonoBehaviour
    {
        [Header("移動參數")]
        [SerializeField] private float maxMoveDistance = 10f; 
        [SerializeField] private float moveSpeed = 1.5f;

        public float MoveSpeed => moveSpeed;

        private Vector3 deltaPos;
        public Vector3 DeltaPos => deltaPos;

        [SerializeField] private float RotationSpeed = 5f;
        private float deltaVertical, deltaHorizontal;
        private Quaternion deltaRot;

        private Vector3 originPos;
        private EnemyHealth enemyHealth;
        private NavMeshAgent agent;
        private GameObject target;
        

        private CapsuleCollider capsuleCollider;
        private Rigidbody rb;
        private Camera cam;

        public Vector2 deltaInput;
        public Vector2 DeltaInput => deltaInput;

        [Header("攻擊參數")]
        [SerializeField] private float closeAttackDistance = 2.0f;
        [SerializeField] private Vector2 FireRandomInterval = new Vector2(1f, 2f);
        [SerializeField] private float Fire1Damage = 1.0f;
        [SerializeField] private float Fire1Force = 1.0f;
        private float fireCooldown = 0f;

        //private PlayerHealth playerHealth;

        

        private EventHandler startAnimReceived, endAnimReceived;
        public event EventHandler StartAnimReceived
        {
            add { startAnimReceived += value; }
            remove {  startAnimReceived -= value; }
        }
        public event EventHandler EndAnimReceived
        {
            add { startAnimReceived += value; }
            remove { startAnimReceived -= value; }
        }

        private EventHandler<FireEventArgs> fireReceived;
        public event EventHandler<FireEventArgs> FireReceived
        {
            add { fireReceived += value; }
            remove { fireReceived -= value; }
        }
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
            originPos = transform.position;
            enemyHealth = GetComponent<EnemyHealth>();
            enemyHealth.DeadReceived += OnDead;

            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
            agent.stoppingDistance = closeAttackDistance;

            fireCooldown = UnityEngine.Random.Range(FireRandomInterval.x, FireRandomInterval.y);

           //playerHealth = GetComponent<PlayerHealth>();
            //playerHealth.DeadReceived += OnDead;
            //playerHealth.RevivalReceived += OnRevival;
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
            agent.isStopped = true;
            enabled = true;
        }

        private void FixedUpdate()
        {
            if (deltaPos.magnitude != 0) rb.MovePosition(transform.position + deltaPos);
            if (deltaRot.eulerAngles.magnitude != 0) rb.MoveRotation(deltaRot);
        }
        private void Update()
        {
            if (target) 
            { 
            OnMove();
            OnFire();
            }
            else
            {
                if(Vector3.Distance(transform.position,originPos) >= maxMoveDistance || target == null)
                {
                    agent.SetDestination(originPos);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.gameObject;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.gameObject;
                //PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                //if(playerHealth.IsDead) target = null;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = null;
            }
        }

        private void OnMove()
        {
            agent.SetDestination(target.transform.position);
            deltaPos = agent.velocity;

            deltaVertical = deltaInput.y = Input.GetAxis("Vertical");
            deltaHorizontal = deltaInput.x = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(deltaHorizontal, 0f, deltaVertical);
            Quaternion noTiltRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            deltaPos = noTiltRotation * movement * MoveSpeed * Time.deltaTime;
        }




        void OnFire()
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance <= closeAttackDistance)
            {
                Quaternion targetRot = Quaternion.LookRotation(target.transform.position - agent.transform.position);
                targetRot = targetRot.normalized;
                targetRot.x = targetRot.z = 0;
                Quaternion deltaRot = Quaternion.Slerp(agent.transform.rotation, targetRot, agent.speed * Time.deltaTime);
                if (deltaRot.eulerAngles.magnitude != 0) agent.transform.rotation = targetRot;

                if (fireCooldown <= 0)
                {
                    FireEventArgs fireEventArgs = new FireEventArgs(Fire1Damage, Fire1Force);
                    fireReceived?.Invoke(this, fireEventArgs);
                    fireCooldown = UnityEngine.Random.Range(FireRandomInterval.x, FireRandomInterval.y);
                }


                if (fireCooldown > 0f)
                {
                    fireCooldown -= Time.deltaTime;
                }
            }

        }
       
    }
}