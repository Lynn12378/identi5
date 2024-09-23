using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

namespace Identi5.GamePlay
{
    public class Shelter : NetworkBehaviour
    {
        private string sceneName;
        private GameMgr gameMgr;
        private ChangeDetector changes;
        private UIManager uIManager;
        private int maxDurability = 100;
        [SerializeField] private GameObject door;
        [Networked] public bool IsOpen { get; set; } = false;
        [Networked] public bool isZombieInShelter { get; set; } = false;
        [Networked] public int durability { get; private set; }
        [Networked] private TickTimer durabilityTimer { get; set; }
        [Networked] private TickTimer failedTimer { get; set; }
        [Networked] private TickTimer endGameTimer { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            uIManager = FindObjectOfType<UIManager>();

            SetDurability_RPC(maxDurability);
            durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
            endGameTimer = TickTimer.CreateFromSeconds(Runner, 10);

            gameMgr.OnEndGame += EndGame;
            gameMgr.OnFailGame += FailGame;
        }

        public override void FixedUpdateNetwork()
        {
            if (endGameTimer.Expired(Runner))
            {
                EndGame();
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
                    gameMgr.dialogCell.SetInfo("基地耐久度不足! 請儘快補充物資!");
                }
                
                if(failedTimer.Expired(Runner))
                {
                    FailGame();
                }
            }
        }

        private void EndGame()
        {
            gameMgr.dialogCell.SetInfo("遊戲結束，即將導向問卷");
            GameMgr.playerOutputData.isFinished = true;
            gameMgr.ODHandler.UpdateOD();
            sceneName = "EndGame";
            Invoke("ChangeScene", 5);
            ChangeScene();
            gameMgr.OnEndGame -= EndGame;
        }

        public void FailGame()
        {
            gameMgr.dialogCell.SetInfo("遊戲失敗!請重新開始!");
            GameMgr.playerOutputData.failGameNo++;
            gameMgr.ODHandler.UpdateOD();
            sceneName = "Lobby";
            Invoke("ChangeScene", 3);
            ChangeScene();
            gameMgr.OnEndGame -= FailGame;
        }

        private void ChangeScene()
        {
            Runner.Shutdown();
            SceneManager.LoadScene(sceneName);
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
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetIsZombieInShelter_RPC(bool isZombieInShelter)
        {
            this.isZombieInShelter = isZombieInShelter;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetPlayerRef_RPC(PlayerRef playerRef)
        {
            this.playerRef = playerRef;
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
                        var time = (int)endGameTimer.RemainingTime(Runner);
                        uIManager.UpdateGameTimeTxt(time);
                        break;
                    case nameof(IsOpen):
                        door.SetActive(!IsOpen);
                        if(!IsOpen && isZombieInShelter && playerRef == Runner.LocalPlayer)
                        {
                            GameMgr.playerOutputData.zombieInShelterNo++;
                        }
                        SetIsZombieInShelter_RPC(false);
                        break;
                }
            }
        }
        #endregion
    }
}