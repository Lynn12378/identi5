using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class TeamCell : NetworkBehaviour
    {
        private GameMgr gameMgr;
        private ChangeDetector changes;
        [SerializeField] private TMP_Text teamTxt;
        [Networked] public int teamID { get; set;}
        [Networked, OnChangedRender(nameof(OnCountChange))]
        public int count { get; set;}

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            gameMgr = GameMgr.Instance;
            gameMgr.teamList.Add(this);
            gameMgr.newTeamID = teamID > gameMgr.newTeamID ? teamID : gameMgr.newTeamID;
            SetInfo(teamID);
            gameMgr.UpdatedTeamList();
        }

        public void SetInfo(int id)
        {
            teamTxt.text = $"隊伍 {id}";
            SetTeamID_RPC(id);
        }

        public void OnJoinClicked()
        {
            SetCount_RPC(++count);
            GameMgr.playerNetworkData.SetPlayerTeamID_RPC(teamID);
            GameMgr.playerOutputData.joinTeamNo++;
        }

        public void OnCountChange()
        {
            if(teamID > 0 && count < 1)
            {
                Despawn_RPC();
            }
        }

        void OnDestroy()
        {
            gameMgr.teamList.Remove(this);
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetTeamID_RPC(int id)
        {
            teamID = id;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetCount_RPC(int count)
        {
            this.count = count;
		}

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void Despawn_RPC()
        {
            Runner.Despawn(Object);
        }
        #endregion
    }
}
