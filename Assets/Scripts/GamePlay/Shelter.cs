using UnityEngine;
using Fusion;

namespace Identi5.GamePlay
{
    public class Shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private UIManager uIManager;
        private int maxDurability = 100;
        public int repair = 20;
        
        [SerializeField] private BoxCollider2D doorCollider;
        [Networked] public bool IsOpen { get; set; } = false;
        [Networked] public int durability { get; private set; }
        [Networked] private TickTimer durabilityTimer { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            uIManager = FindObjectOfType<UIManager>();

            SetDurability_RPC(maxDurability);
            durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
        }

        public override void FixedUpdateNetwork()
        {
            if (durabilityTimer.Expired(Runner) && durability > 0)
            {
                SetDurability_RPC(durability - 1);
                durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
            }
            
            if(durability == 0)
            {
                GameMgr.playerOutputData.failGame++;
            }
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetDurability_RPC(int durability)
        {
            this.durability = durability;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsOpen_RPC()
        {
            this.IsOpen = !IsOpen;
            if(IsOpen)
            {
                playerRef = Runner.LocalPlayer;
            }
        }
        #endregion

        #region - OnChanged Events -
        public override void Render()
        {
            foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (change)
                {
                    case nameof(durability):
                        uIManager.UpdateDurabilitySlider(durability, maxDurability);
                        break;
                    case nameof(IsOpen):
                        doorCollider.isTrigger = IsOpen;
                        
                        break;
                }
            }
        }
        #endregion
    }
}