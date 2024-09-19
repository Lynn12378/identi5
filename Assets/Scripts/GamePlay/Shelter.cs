using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

namespace Identi5.GamePlay
{
    public class Shelter : NetworkBehaviour
    {
        private ChangeDetector changes;
        private UIManager uIManager;
        private int maxDurability = 100;
        [SerializeField] private BoxCollider2D doorCollider;
        [Networked] public bool IsOpen { get; set; } = false;
        [Networked] public int durability { get; private set; }
        [Networked] private TickTimer durabilityTimer { get; set; }
        [Networked] private TickTimer failedTimer { get; set; }
        [Networked] private TickTimer endGameTimer { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            uIManager = FindObjectOfType<UIManager>();

            SetDurability_RPC(maxDurability);
            durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
            endGameTimer = TickTimer.CreateFromSeconds(Runner, 1800);
        }

        public override void FixedUpdateNetwork()
        {
            if (endGameTimer.Expired(Runner))
            {
                GameMgr.playerOutputData.isFinished = true;
                GameMgr.Instance.ODHandler.UpdateOD();
                Runner.Shutdown();
                SceneManager.LoadScene("EndGame");
            }

            if (durabilityTimer.Expired(Runner) && durability > 0)
            {
                SetDurability_RPC(durability - 1);
                durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
            }
            
            if(durability == 0)
            {
                if(!failedTimer.IsRunning)
                {
                    failedTimer = TickTimer.CreateFromSeconds(Runner, 60);
                    GameMgr.Instance.dialogCell.SetInfo("基地耐久度不足! 請儘快補充物資!");
                }
                
                if(failedTimer.Expired(Runner))
                {
                    GameMgr.playerOutputData.failGameNo++;
                    GameMgr.Instance.ODHandler.UpdateOD();
                    Runner.Shutdown();
                    Restart();
                }
            }
        }

        public async void Restart()
        {
			if (Runner.IsSceneAuthority) {
                await Runner.UnloadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("GamePlay")));
                await Runner.LoadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Lobby")), LoadSceneMode.Additive);
            }
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetDurability_RPC(int durability)
        {
            this.durability = durability < maxDurability ? durability :maxDurability;
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