using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;

using Identi5.DB;
using Identi5.GamePlay;
using Identi5.GamePlay.Cell;
using Identi5.GamePlay.Player;

namespace Identi5
{
    public class GameMgr : MonoBehaviour
    {
        #region  - Runner -
        public static GameMgr Instance { get; private set; }
        [SerializeField] private NetworkRunner runner = null;
        public NetworkRunner Runner
        {
            get
            {
                if (runner == null)
                {
                    runner = gameObject.AddComponent<NetworkRunner>();
                    runner.ProvideInput = true;
                }
                return runner;
            }
        }

        private void Awake()
        {
            Runner.ProvideInput = true;

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
        #endregion
        
        #region - playerInfo -
            public static PlayerInfo playerInfo = null;
            public Dictionary<PlayerRef, PlayerInfo> PIFList = new Dictionary<PlayerRef, PlayerInfo>();

            public event Action OnPIFListUpdated = null;
            public void UpdatePIFList()
            {
                OnPIFListUpdated?.Invoke();
            }
        #endregion

        #region - playerNetworkData -
            public static PlayerNetworkData playerNetworkData;
            public Dictionary<PlayerRef, PlayerNetworkData> PNDList = new Dictionary<PlayerRef, PlayerNetworkData>();
            
            public event Action OnInPNDListUpdated = null;
            public void UpdatedPNDList()
            {
                OnInPNDListUpdated?.Invoke();
            }
        #endregion

        #region - PlayerOutputData -
            public static PlayerOutputData playerOutputData = null;
        #endregion

        #region - Messages -
        public List<MessageCell> messageList = new List<MessageCell>();

        public event Action OnMessageListUpdated = null;
        public void UpdatedMessageList()
        {
            OnMessageListUpdated?.Invoke();
        }
        #endregion

        #region - TeamList -
            public int newTeamID = 0;
            public List<TeamCell> teamList = new List<TeamCell>();
            public event Action OnTeamListUpdated = null;
            public void UpdatedTeamList()
            {
                OnTeamListUpdated?.Invoke();
            }
            public void UpdatedPlayerMinimap()
            {
                OnTeamListUpdated?.Invoke();
            }
        #endregion

        #region - ItemList -
            public event Action OnItemListUpdated = null;
            public void UpdateItemList()
            {
                OnItemListUpdated?.Invoke();
            }
        #endregion
        #region - RankList -
            public event Action OnRankListUpdated = null;
            public void UpdateRankList()
            {
                OnRankListUpdated?.Invoke();
            }
        #endregion
        #region - outfits -
            public event Action OnOutfitsUpdated = null;
            public void UpdateOutfits()
            {
                OnOutfitsUpdated?.Invoke();
            }
        #endregion
        public AudioSource BGM;
        public AudioSource source;
        public OutputDBHandler ODHandler;
        public Shelter shelter;
        public DialogCell dialogCell;
        public DocCell docCell;
        public Dictionary<PlayerRef, PlayerVoiceDetection> playerVoiceList = new Dictionary<PlayerRef, PlayerVoiceDetection>();
    }
}