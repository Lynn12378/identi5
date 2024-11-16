using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections;

namespace Identi5.GamePlay
{
    public class Shelter : NetworkBehaviour
    {
        private string sceneName;
        private GameMgr gameMgr;
        private ChangeDetector changes;
        private UIManager uIManager;
        private string name="";
        private int maxDurability = 100;
        [SerializeField] private GameObject door;
        [Networked] public bool IsOpen { get; set; } = false;
        [Networked] public bool isZombieInShelter { get; set; } = false;
        [Networked] public int durability { get; private set; }
        [Networked] private TickTimer durabilityTimer { get; set; }
        [Networked] private TickTimer failedTimer { get; set; }
        [Networked] private TickTimer endGameTimer { get; set; }
        [Networked] public PlayerRef playerRef { get; private set; }

        [Networked, OnChangedRender(nameof(OnGameStateChanged))]
        public NetworkBool gameEnded { get; set; }

        [Networked, OnChangedRender(nameof(OnGameStateChanged))]
        public NetworkBool gameFailed { get; set; }

        [Networked, OnChangedRender(nameof(SetName))] private string nextSceneName{ get; set; }

        public override void Spawned()
        {
            gameMgr = GameMgr.Instance;
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            uIManager = FindObjectOfType<UIManager>();

            SetDurability_RPC(maxDurability);
            durabilityTimer = TickTimer.CreateFromSeconds(Runner, 5);
            endGameTimer = TickTimer.CreateFromSeconds(Runner, 600);
        }

        private void SetName()
        {
            name = nextSceneName;
        }
        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (endGameTimer.Expired(Runner))
                {
                    gameEnded = true;
                    SetSceneName_RPC("EndGame");
                }

                if (durability == 0)
                {
                    if (!failedTimer.IsRunning)
                    {
                        failedTimer = TickTimer.CreateFromSeconds(Runner, 60);
                        gameMgr.dialogCell.SetInfo("基地耐久度不足! 請儘快補充物資!");
                    }

                    if (failedTimer.Expired(Runner))
                    {
                        gameFailed = true;
                        SetSceneName_RPC("Lobby");
                    }
                }
            }
        }

       public void OnGameStateChanged()
        {
            if (gameEnded)
            {
                HandleEndGame();
            }
            else if (gameFailed)
            {
                HandleFailGame();
            }
        }

        private void HandleEndGame()
        {
            gameMgr.dialogCell.SetInfo("遊戲結束，即將導向問卷");
            GameMgr.playerOutputData.isFinished = true;
            gameMgr.ODHandler.UpdateOD();
            StartCoroutine(DelayedSceneChange());
        }

        private void HandleFailGame()
        {
            gameMgr.dialogCell.SetInfo("遊戲失敗!請重新開始!");
            GameMgr.playerOutputData.failGameNo++;
            gameMgr.ODHandler.UpdateOD();
            StartCoroutine(DelayedSceneChange());
        }

        private IEnumerator DelayedSceneChange()
        {
            float delay = 3f;
            yield return new WaitForSeconds(delay);
    
            if (Runner != null)
            {
                yield return new WaitForEndOfFrame();
                FindObjectOfType<GPMgr>().Leave(name);
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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetSceneName_RPC(string name)
        {
            nextSceneName = name;
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