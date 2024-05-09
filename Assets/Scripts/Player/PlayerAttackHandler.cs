using System.Collections;
using UnityEngine;

using Fusion;

public class PlayerAttackHandler : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootPoint = null;


    public void Shoot(Vector3 mousePosition)
    {
        bulletPrefab.mousePosition = mousePosition;
   
        Runner.Spawn(bulletPrefab, shootPoint.position, transform.rotation, Object.InputAuthority);
    }
}