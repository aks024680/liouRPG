using System;
using UnityEngine;

namespace liou
{
    /// <summary>
    /// 敵人動畫控制器
    /// </summary>
    public class EnemyAnimationController : MonoBehaviour
    {
        Animator animator;

        private int hitTriggerHash, deadTriggerHash, revivalTriggerHash;
        EnemyHealth enemyHealth;

        private EnemyMovementController enemyMovementController;
        private int velYHash;



        private int attackHash;
        
        

        private EventHandler startAnimReceived, endAnimReceived;
        public event EventHandler StartAnimReceived
        {
            add { startAnimReceived += value; }
            remove { startAnimReceived -= value; }
        }

        public event EventHandler EndAnimReceived
        {
            add { endAnimReceived += value; }
            remove { endAnimReceived -= value; }
        }


        private void Start()
        {
            animator = GetComponent<Animator>();

            enemyHealth = GetComponentInParent<EnemyHealth>();
            enemyHealth.HitReceived += OnHit;
            enemyHealth.DeadReceived += OnDead;


            enemyMovementController = GetComponentInParent<EnemyMovementController>();
 
            enemyMovementController.Fire1Received += OnFire;
            attackHash = Animator.StringToHash("Attack1Trigger");
            
            
           

            hitTriggerHash = Animator.StringToHash("HitTrigger");
            deadTriggerHash = Animator.StringToHash("DeadTrigger");
           
        }
        void StartAttackAnimation()
        {
            startAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        void EndAttackAnimation()
        {
            endAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        private void Update()
        {
            OnMove();

           
        }

        private void OnMove()
        {
            
            animator.SetFloat(velYHash, enemyMovementController.DeltaPos.magnitude);
        }
       
        void OnFire(object sender, EventArgs e)
        {
            animator.SetTrigger(attackHash);
        }
    

        private void OnHit(object sender, EventArgs e) { animator.SetTrigger(hitTriggerHash); }
        private void OnDead(object sender, EventArgs e) { animator.SetTrigger(deadTriggerHash); }
      

    }
}

