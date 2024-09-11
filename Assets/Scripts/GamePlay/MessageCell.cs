using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace Identi5.GamePlay
{
    public class MessageCell : NetworkBehaviour
    {
        private ChangeDetector changes;
        [SerializeField] private TMP_Text messageTxt = null;
        [Networked] public string playerName { get; set; }
        [Networked] public string message { get; set; }
        [Networked] TickTimer timer { get; set; }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            timer = TickTimer.CreateFromSeconds(Runner, 10f);
            messageTxt.text = $"{playerName}:{message}";
            GameMgr.Instance.UpdatedMessageList();
            GameMgr.Instance.messageList.Add(this);
        }

        public override void FixedUpdateNetwork()
        {
            if (timer.Expired(Runner))
            {
                GameMgr.Instance.messageList.Remove(this);
                Runner.Despawn(Object);
            }
        }

        #region - RPCs -

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetMessage_RPC(string playerName, string message)
        {
            this.playerName = playerName;
            this.message = message;
		}

        #endregion

        #region - OnChanged Events -

            public override void Render()
            {
                foreach (var change in changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
                {
                    switch (change)
                    {
                        case nameof(message):
                            messageTxt.text = $"{playerName}:{message}";
                            GameMgr.Instance.UpdatedMessageList();
                            break;
                    }
                }
            }
        
        #endregion
    }
}