using UnityEngine;
using Fusion.Addons.Physics;

namespace Identi5.GamePlay.Player
{
    public class PlayerMovementHandler : MonoBehaviour
    {
        private float moveSpeed = 5f;
        private float lastPlayTime;
        private float audioClipLength = 0.667f;
        [SerializeField] private Animator animator;
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        [SerializeField] private Transform Weapon = null;
        
        public void Move(NetworkInputData data)
        {
            Vector2 moveVector = data.movementInput.normalized;
            Vector2 newVelocity = moveVector * moveSpeed;
            playerNetworkRigidbody.Rigidbody.velocity = newVelocity;

            animator.SetBool("Walk",true);
            if(newVelocity.magnitude > 0)
            {
                if (Time.time - lastPlayTime >= audioClipLength)
                {
                    // AudioManager.Instance.Play("Walk");
                    lastPlayTime = Time.time;
                }
            }
        }

        public void SetRotation(Vector2 mousePosition)
        {
            float rotation = Vector2.SignedAngle(Vector2.up, mousePosition - new Vector2(transform.position.x, transform.position.y));
            Weapon.rotation = Quaternion.Euler(Vector3.forward * (rotation + 90));
        }
    }
}