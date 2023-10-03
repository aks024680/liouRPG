
using System;
using Unity.VisualScripting;
using UnityEngine;
namespace liou
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        Animator animator;

        private int hitTriggerHash, deadTriggerHash, revivalTriggerHash;
        PlayerHealth playerHealth;

        private PlayerMovementController playerMovementController;
        private int velXHash,velYHash;

        private int jumpTriggerHash, isGroundedHash;

        private int attack1Index = 0, attack1MaxIndex = 2;
        private int attack1Hash, attack1IndexHash;

        private EventHandler startAnimReceived, endAnimReceived;
        public event EventHandler StartAnimReceived
        {
            add { startAnimReceived += value; }
            remove{startAnimReceived -= value;}
        }

        public event EventHandler EndAnimReceived
        {
            add { endAnimReceived += value; }
            remove { endAnimReceived -= value;}
        }

        private void Start()
        {
            animator = GetComponent<Animator>();

            playerHealth = GetComponentInParent<PlayerHealth>();
            playerHealth.HitReceived += OnHit;
            playerHealth.DeadReceived += OnDead;
            playerHealth.RevivalReceived += OnRevival;

            playerMovementController = GetComponentInParent<PlayerMovementController>();
            playerMovementController.JumpReceived += OnJump;
            playerMovementController.Fire1Received += OnFire1;
            attack1Hash = Animator.StringToHash("Attack1Trigger");
            attack1IndexHash = Animator.StringToHash("Attack1Index");
            velXHash = Animator.StringToHash("VelX");
            velYHash = Animator.StringToHash("VelY");
            jumpTriggerHash = Animator.StringToHash("JumpTrigger");
            isGroundedHash = Animator.StringToHash("isGrounded");

            hitTriggerHash = Animator.StringToHash("HitTrigger");
            deadTriggerHash = Animator.StringToHash("DeadTrigger");
            revivalTriggerHash = Animator.StringToHash("RevivalTrigger");
        }
        private void Update()
        {
            OnMove();
            
            animator.SetBool(isGroundedHash, playerMovementController.IsGrounded());
        }

        private void OnMove()
        {
            Vector3 localMoveDirection = transform.InverseTransformDirection(playerMovementController.DeltaPos);
            Vector3 normalVel = Vector3.Normalize(localMoveDirection);
            Vector3 targetVel = Vector3.Lerp(localMoveDirection,normalVel,playerMovementController.deltaInput.magnitude); print(localMoveDirection+"\t"+ normalVel+ "\t"+playerMovementController.deltaInput.magnitude);



            animator.SetFloat(velXHash,targetVel.x);
            animator.SetFloat(velYHash, targetVel.z);
        }
        void OnJump(object sender,EventArgs e)
        {

            
            animator.SetTrigger(jumpTriggerHash);
        }
        void OnFire1(object sender,EventArgs e)
        {
            attack1Index++;
            if (attack1Index > attack1MaxIndex) attack1Index = 0;
            animator.SetTrigger(attack1Hash);
            animator.SetInteger(attack1IndexHash, attack1Index);
        }
        public void StartAttackAnimation()
        {
            startAnimReceived?.Invoke(this,EventArgs.Empty);
        }

        public void EndAttackAnimation()
        {
            endAnimReceived?.Invoke(this,EventArgs.Empty);
        }

        private void OnHit(object sender, EventArgs e) { animator.SetTrigger(hitTriggerHash); }
        private void OnDead(object sender, EventArgs e) { animator.SetTrigger(deadTriggerHash); }
        private void OnRevival(object sender, EventArgs e) { animator.SetTrigger(revivalTriggerHash); }

    }

}
