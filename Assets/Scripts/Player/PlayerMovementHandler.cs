using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

namespace DEMO.Player
{
    public class PlayerMovementHandler : MonoBehaviour
    {
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        [SerializeField] private Transform Weapon = null;
        [SerializeField] private float moveSpeed = 5f;

        public void Move(Vector2 movementInput)
        {
            Vector2 moveVector = movementInput.normalized;
            playerNetworkRigidbody.Rigidbody.velocity = moveVector * moveSpeed;
        }

        public void SetRotation(Vector2 mousePosition)
        {
            float rotation = Vector2.SignedAngle(Vector2.up, mousePosition);
            Weapon.rotation = Quaternion.Euler(Vector3.forward * (rotation + 90));
        }
    }
}