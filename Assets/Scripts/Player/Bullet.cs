using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

namespace DEMO.Player
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float bulletTime = 0.5f;
        [SerializeField] private int damage = 10;

        [Networked] private TickTimer life { get; set; }

        public Vector2 mousePosition;

        public void Init(Vector2 mousePosition)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;

            Debug.Log($"Init Bullet");
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(mousePosition * bulletSpeed);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }


        // private void OnTriggerEnter2D(Collider2D collision)
        // {
        //     Enemy enemy = collision.GetComponent<Enemy>();

        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(damage);
        //         Runner.Despawn(Object);
        //     }
        // }
    }
}