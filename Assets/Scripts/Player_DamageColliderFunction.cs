
using System;
using UnityEngine;

namespace liou
{
    public class Player_DamageColliderFunction : MonoBehaviour
    {
        [SerializeField] private GameObject ref_FX;

        private PlayerMovementController playerMovementController;
        private PlayerAnimatorController playerAnimatorController;
        private FireEventArgs fireEventArgs;
        private Collider col;

        private GameObject owner;
        private void Start()
        {
            col = GetComponent<Collider>();
            col.enabled = false;

            playerMovementController = GetComponentInParent<PlayerMovementController>();
            playerMovementController.Fire1Received += OnFire1;

            playerAnimatorController = GetComponentInParent<PlayerAnimatorController>();
            playerAnimatorController.StartAnimReceived += OnStartAttackAnimation;
            playerAnimatorController.EndAnimReceived += OnEndAttackAnimation;

            owner = playerMovementController.gameObject;
        
                }

        private void OnFire1(object sender, FireEventArgs e)
        {
            fireEventArgs = e;
        }
        void OnStartAttackAnimation(object sender , EventArgs e)
        {
            col.enabled = true;
        }
        void OnEndAttackAnimation (object sender , EventArgs e)
        {
            col.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
            {
                Rigidbody rb = other.attachedRigidbody;
                if (rb)
                {
                    Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                    Vector3 direction = Vector3.Normalize(other.gameObject.transform.position - owner.transform.position);
                    rb.AddForce(direction * fireEventArgs.FireForce,ForceMode.Impulse);
                    GameObject tempFX = Instantiate(ref_FX,contactPoint,Quaternion.identity);
                    tempFX.AddComponent<ParticleEffectController>();

                    if (!other.CompareTag("Player"))
                    {
                        float TotalDamage = fireEventArgs.FireDamage;
                        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                        if (enemyHealth)
                        {
                            enemyHealth.OnDamage(TotalDamage);
                        }
                    }
                }
            }
        }
    }

}

