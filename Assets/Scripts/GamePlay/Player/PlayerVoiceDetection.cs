using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;

namespace Identi5.GamePlay.Player
{
    public class PlayerVoiceDetection : NetworkBehaviour
    {
        public Recorder rec;
        private GameMgr gameMgr;
        public GameObject icon;

        [SerializeField] public VoiceNetworkObject voiceObject;
        [SerializeField] private PlayerNetworkData PND;
        [SerializeField] private List<PlayerController> playersInRange = new List<PlayerController>();

        private void Start()
        {
            gameMgr = GameMgr.Instance;
            gameMgr.playerVoiceList.Add(Object.InputAuthority, this);   

            if (Object.StateAuthority == Runner.LocalPlayer)
            {
                rec = voiceObject.RecorderInUse;
                rec.TransmitEnabled = false;
                rec.VoiceDetection = false;
            }
        }
        public void AudioCheck()
        {
            if(rec != null && rec.IsCurrentlyTransmitting)
            {
                icon.SetActive(true);
            }
            else
            {
                icon.SetActive(false);
            }
        }

        #region - Distance Limit -
        private void EnableMicrophone(PlayerController playerController, bool enable)
        {
            var speaker = playerController.GetPlayerVoiceDetection().voiceObject.SpeakerInUse;
            if(rec != null)
            {
                if(enable == false)
                {
                    if(rec.TransmitEnabled)
                    {
                        gameMgr.dialogCell.SetInfo("範圍內沒有其餘玩家，麥克風已關閉");
                    }
                    rec.TransmitEnabled = enable;
                    rec.VoiceDetection = enable;
                    speaker.enabled = enable;
                }
                else
                {
                    rec.TransmitEnabled = rec.TransmitEnabled;
                    rec.VoiceDetection = enable;
                    speaker.enabled = enable;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                var colliderPlayerController = collider.GetComponent<PlayerController>();
                playersInRange.Add(colliderPlayerController);
                EnableMicrophone(colliderPlayerController, true);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                var colliderPlayerController = collider.GetComponent<PlayerController>();
                playersInRange.Remove(colliderPlayerController);
                EnableMicrophone(colliderPlayerController, false);
            }
        }
        #endregion

        #region - Voice Detection -
        void Update()
        {
            if (rec != null && rec.TransmitEnabled && rec.VoiceDetection)
            {
                if (rec.LevelMeter.CurrentAvgAmp >= rec.VoiceDetectionThreshold)
                {
                    GameMgr.playerOutputData.totalVoiceDetectionDuration += Time.deltaTime;
                }
            }
        }
        #endregion
    }
}