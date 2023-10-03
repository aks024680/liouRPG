using System;
using System.Collections;
using UnityEngine;

namespace liou
{
/// <summary>
/// 玩家血量
/// </summary>
public class PlayerHealth : MonoBehaviour
{
        [SerializeField] private GameObject ref_FX;
        [SerializeField] private float maxHP = 10f;

        public float MaxHP => maxHP;
        private float hp;

        public float CurrentHP => hp;

        private float AfterDeadTime = 3f;
        private bool isDead = false;

        public bool IsDead =>isDead;

        private EventHandler deadReceived;
        public event EventHandler DeadReceived
        {
            add { deadReceived += value; }
            remove { deadReceived -= value; }
        }
        private EventHandler hitReceived;
        public event EventHandler HitReceived
        {
            add { hitReceived += value; }
            remove { hitReceived -= value; }
        }

        private EventHandler revivalReceived;
        public event EventHandler RevivalReceived
        {
            add { revivalReceived += value; }
            remove
            { revivalReceived -= value; }
        }
        private void Start()
        {
            hp = MaxHP;
        }
        private void Update()
        {
            if(Input.GetKeyDown("0")) {
                OnDamage(1);
            }
        }
        public void OnDamage(float damage)
        {
            hitReceived?.Invoke(this, EventArgs.Empty);
            hp -= damage;
            if(hp <= 0 && !isDead) { OnDead(); }
        }

        public void OnHealing(float heal)
        {
            hp += heal;
            if (hp >= MaxHP) { hp = MaxHP; }
        }

        void OnDead()
        {
            isDead = true;

            deadReceived?.Invoke(this, EventArgs.Empty);
            enabled = false;
            StartCoroutine(AfterDead());
        }

        IEnumerator AfterDead()
        {
            yield return new WaitForSeconds(AfterDeadTime);
            GameObject tempFx = Instantiate(ref_FX, transform.position, Quaternion.identity);
            tempFx.AddComponent<ParticleEffectController>();

            OnRevival();
        }

        public void OnRevival()
        {
            isDead = false;
            enabled = true;
            revivalReceived?.Invoke(this, EventArgs.Empty);
            hp = MaxHP;
        }
    }
}

