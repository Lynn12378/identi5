using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

public class Bullet : NetworkBehaviour//, IPredictedSpawnBehaviour
{
    [SerializeField] private NetworkRigidbody2D networkRigidbody = null;

    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletTime = 0.5f;
    [SerializeField] private int damage = 10;

    [Networked] private TickTimer life { get; set; }

    public Vector3 mousePosition;
    private List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
    private Vector3 interpolateFrom;
    private Vector3 interpolateTo;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, bulletTime);

        networkRigidbody.InterpolationTarget.gameObject.SetActive(true);

        networkRigidbody.Rigidbody.velocity = Vector2.zero;
    }

    public override void FixedUpdateNetwork()
    {
        Vector2 mouseVector = mousePosition.normalized;

        networkRigidbody.Rigidbody.velocity = mouseVector * bulletSpeed; //mouseVector * bulletSpeed;

        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        var player = collision.GetComponent<PlayerController>();

        if (collision != null)
        {
            player.TakeDamage(damage);
        }

        Runner.Despawn(Object);
    }
}