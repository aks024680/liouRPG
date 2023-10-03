using System;
using UnityEngine;

namespace liou
{
    /// <summary>
    /// 敵人受傷控制器
    /// </summary>
    public class Enemy_DamageColliderFunction : MonoBehaviour
    {
        [SerializeField] private GameObject ref_FX;

        private EnemyMovementController enemyMovementController;
        private EnemyAnimationController enemyAnimationController;
        private FireEventArgs fireEventArgs;
        private Collider col;

        private GameObject owner;
        private void Start()
        {
            col = GetComponent<Collider>();
            col.enabled = false;

            enemyMovementController = GetComponentInParent<EnemyMovementController>();
            enemyMovementController.Fire1Received += OnFire1;

            enemyAnimationController = GetComponentInParent<EnemyAnimationController>();
            enemyAnimationController.StartAnimReceived += OnStartAttackAnimation;
            enemyAnimationController.EndAnimReceived += OnEndAttackAnimation;

            owner = enemyMovementController.gameObject;

        }

        private void OnFire1(object sender, FireEventArgs e)
        {
            fireEventArgs = e;
        }
        void OnStartAttackAnimation(object sender, EventArgs e)
        {
            col.enabled = true;
        }
        void OnEndAttackAnimation(object sender, EventArgs e)
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
                    rb.AddForce(direction * fireEventArgs.FireForce, ForceMode.Impulse);
                    GameObject tempFX = Instantiate(ref_FX, contactPoint, Quaternion.identity);
                    tempFX.AddComponent<ParticleEffectController>();

                    if (!other.CompareTag("Enemy"))
                    {
                        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                        if (playerHealth)
                        {
                            playerHealth.OnDamage(fireEventArgs.FireDamage);
                        }
                    }
                }
            }
        }
    }
}

