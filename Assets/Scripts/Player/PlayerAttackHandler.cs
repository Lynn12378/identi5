using System.Collections;
using UnityEngine;

using Fusion;

public class PlayerAttackHandler : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootPoint = null;


    public void Shoot(Vector2 mousePosition)
    {
        bulletPrefab.mousePosition = mousePosition - new Vector2(transform.position.x, transform.position.y);

        Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles - Vector3.forward * 90);

        Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority);
    }
}