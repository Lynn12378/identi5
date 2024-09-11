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
        [Networked][Capacity(5)] public NetworkArray<string> member => default;

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            gameMgr = GameMgr.Instance;
            gameMgr.teamList.Add(this);
            SetInfo(teamID);
            gameMgr.newTeamID = teamID;
        }
        public override void FixedUpdateNetwork()
        {
            if(teamID != 0 && member[0] == "")
            {
                Despawned();
                gameMgr.UpdatedTeamList();
            }
        }

        public void Despawned()
        {
            gameMgr.teamList.Remove(this);
            Runner.Despawn(Object);
        }

        public void SetInfo(int id)
        {
            teamTxt.text = $"隊伍 {id}";
            SetTeamID_RPC(id);
        }

        public void OnJoinClicked()
        {        
            gameMgr.PNDList[Runner.LocalPlayer].SetPlayerTeamID_RPC(teamID);
        }

        #region - RPCs -
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public void SetTeamID_RPC(int id)
        {
            teamID = id;
		}

        public void SetMember(List<string> memberList)
        {
            member.Clear();
            for(int i = 0; i < memberList.Count; i++)
            {
                member.Set(i, memberList[i]);
            }
		}
        #endregion
    }
}
