using UnityEngine;
using Fusion.Addons.Physics;

namespace Identi5.GamePlay.Player
{
    public class PlayerMovementHandler : MonoBehaviour
    {
        [SerializeField] private Transform Weapon = null;
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        // [SerializeField] private Animator animator;
        [SerializeField] private AudioClip clip;
        public static float deltaTime;
        private float timer = 0;

        void Update()
        {
            timer += Time.deltaTime;
        }

        public void Move(NetworkInputData data)
        {
            Vector2 moveVector = data.movementInput.normalized;
            Vector2 newVelocity = moveVector * 5f;
            playerNetworkRigidbody.Rigidbody.velocity = newVelocity;

            if(newVelocity != Vector2.zero && timer > 0.03f)
            {
                GameMgr.Instance.source.clip = clip;
                GameMgr.Instance.source.Play();
                timer = 0;
                // animator.SetBool("Walk",true);
            }
        }

        public void SetRotation(Vector2 mousePosition)
        {
            float rotation = Vector2.SignedAngle(Vector2.up, mousePosition - new Vector2(transform.position.x, transform.position.y));
            Weapon.rotation = Quaternion.Euler(Vector3.forward * (rotation + 90));
        }
    }
}