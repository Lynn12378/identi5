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
        [SerializeField] private int minRange = 0;
        [SerializeField] private int maxRange = 11;
        [SerializeField] private int initAmount = 0;
        [SerializeField] private int spawnAmount = 0;
        [SerializeField] private float spawnTime = 0.0f;
        [Networked] private TickTimer spawnTimer { get; set; }
        public override void Spawned()
        {
            var collider = gameObject.GetComponent<BoxCollider2D>();
            width = collider.bounds.extents.x;
            height = collider.bounds.extents.y;
            for (int i = 0; i < initAmount; i++)
            {
                RandomSpawn();
            }
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
            if(FindObjectsOfType<Item>().Length > 150){return;}
            int seed = Random.Range(minRange, maxRange);
            Vector3 position = transform.position + new Vector3(Random.Range(-width, width),Random.Range(-height, height),0);
            Runner.Spawn(item, position, Quaternion.identity).GetComponent<Item>().SetItemID_RPC(seed);
        }
        #endregion
    }
}