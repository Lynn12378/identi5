using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class MessageCell : NetworkBehaviour
    {
        private ChangeDetector changes;
        [SerializeField] private TMP_Text messageTxt = null;
        [Networked] public string playerName { get; set; }
        [Networked] TickTimer timer { get; set; }
        [Networked, OnChangedRender(nameof(OnMessageChange))]
        public string message { get; set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            timer = TickTimer.CreateFromSeconds(Runner, 10f);
            GameMgr.Instance.messageList.Add(this);
            OnMessageChange();
        }

        public override void FixedUpdateNetwork()
        {
            if (timer.Expired(Runner))
            {
                GameMgr.Instance.messageList.Remove(this);
                Runner.Despawn(Object);
            }
        }

        public void OnMessageChange()
        {
            messageTxt.text = $"{playerName}:{message}";
            GameMgr.Instance.UpdatedMessageList();
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetMessage_RPC(string playerName, string message)
        {
            this.playerName = playerName;
            this.message = message;
		}

        #endregion
    }
}