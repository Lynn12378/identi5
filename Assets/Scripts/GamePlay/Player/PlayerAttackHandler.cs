using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Player
{
    public class PlayerAttackHandler : NetworkBehaviour
    {
        [SerializeField] private Bullet bulletPrefab = null;
        [SerializeField] private Transform shootPoint = null;

        public void Shoot(Vector2 mousePosition)
        {
            Quaternion rotation = Quaternion.Euler(shootPoint.rotation.eulerAngles);
            Runner.Spawn(bulletPrefab, shootPoint.position, rotation, Object.InputAuthority,
                (Runner, NO) => NO.GetComponent<Bullet>().Init(mousePosition));
        }
    }
}