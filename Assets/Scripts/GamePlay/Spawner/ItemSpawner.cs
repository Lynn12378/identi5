using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Identi5.GamePlay.Spawner
{
    public class ItemSpawner : NetworkBehaviour
    {
        private float width;
        private float height;

        #region - item -
        [SerializeField] private NetworkObject item;
        [SerializeField] private int spawnAmount = 20;
        [SerializeField] private float spawnTime = 300.0f;
        [Networked] private TickTimer spawnTimer { get; set; }
        public override void Spawned()
        {
            var collider = gameObject.GetComponent<BoxCollider2D>();
            width = collider.bounds.extents.x;
            height = collider.bounds.extents.y;
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnTime);
        }
        public override void FixedUpdateNetwork()
        {
            if (spawnTimer.Expired(Runner))
            {
                for (int i = 0; i < spawnAmount; i++)
                {
                    RandomSpawn();
                }
                spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnTime);
            }
        }
        public void RandomSpawn()
        {
            int seed = Random.Range(0, 5);
            Vector3 position = transform.position + new Vector3(Random.Range(-width, width),Random.Range(-height, height),0);
            Runner.Spawn(item, position, Quaternion.identity).GetComponent<Item>().SetItemID_RPC(seed);
        }
        #endregion
    }
}