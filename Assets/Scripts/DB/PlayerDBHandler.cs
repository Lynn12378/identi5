using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

namespace Identi5.DB
{
    public class PlayerDBHandler : DBMgr
    {
        private float timer = 0;
        [SerializeField] private PanelMgr panelMgr;
        [SerializeField] private PlayerInfo playerInfo;
        [SerializeField] public PlayerOutputData playerOutputData;
        [SerializeField] private TMP_Text LPlayerNameTxt;
        [SerializeField] private TMP_Text LPlayerPasswordTxt;
        [SerializeField] private TMP_Text SPlayerNameTxt;
        [SerializeField] private TMP_Text SPlayerPasswordTxt;

        [SerializeField] private TMP_Text messageTxt;
        [SerializeField] private List<GameObject> dialog = new List<GameObject>();
        [SerializeField] private List<GameObject> CreateRolePanel = new List<GameObject>();

        void Update()
        {
            timer += Time.deltaTime;
        }

        public void Login()
        {
            action = "login";
            playerInfo.Player_name = LPlayerNameTxt.text.Trim('\u200b');
            playerInfo.Player_password = LPlayerPasswordTxt.text.Trim('\u200b');
            StartCoroutine(SendData());
        }

        public void SignUp()
        {
            action = "signUp";
            playerInfo.Player_name = SPlayerNameTxt.text.Trim('\u200b');
            playerInfo.Player_password = SPlayerPasswordTxt.text.Trim('\u200b');
            SetOufits();
            playerInfo.colorList[0] = Color.white;
            playerInfo.colorList[1] = Color.white;
            StartCoroutine(SendData());
        }

        public void Create()
        {
            action = "create";
            playerOutputData.outfitTime = timer;
            SetOufits();
            StartCoroutine(SendData());
        }

        public new IEnumerator SendData()
        {
            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("PlayerInfo", playerInfo.ToJson()),
                new MultipartFormDataSection("PlayerOutputData", playerOutputData.ToJson()),
            };

            SetForm(formData, "Player", action);
            yield return StartCoroutine(base.SendData());
            var response = base.GetResponseText();
            Debug.Log(response);
            JObject jsonResponse = JObject.Parse(response);

            if (!string.IsNullOrEmpty(response))
            {
                var status = jsonResponse["status"].ToString();
                if (status == "Success")
                {
                    switch(action)
                    {
                        case "signUp":
                            playerInfo.Player_id = Int32.Parse(jsonResponse["Player_id"].ToString());
                            panelMgr.OnActivePanel(CreateRolePanel);
                            timer = 0;
                            break;
                        case "login":
                            var playerInfoJS = jsonResponse["PlayerInfo"].ToString();
                            var playerOutputDataJS = jsonResponse["PlayerOutputData"].ToString();
                            playerInfo.FromJson(playerInfoJS);
                            playerOutputData.FromJson(playerOutputDataJS);
                            SceneManager.LoadScene("Lobby");
                            break;
                        case "create":
                            SceneManager.LoadScene("Lobby");
                            break;
                        case "Update":
                            break;
                    }
                    playerOutputData.playerId = playerInfo.Player_id;
                    GameMgr.playerOutputData = playerOutputData;
                }
                else
                {
                    messageTxt.text = jsonResponse["message"].ToString();
                    panelMgr.OnActivePanel(dialog);
                }
                GameMgr.playerInfo = playerInfo;
            }
            else
            {
                messageTxt.text = "無法連線到伺服器";
                panelMgr.OnActivePanel(dialog);
            }
        }

        private void SetOufits()
        {
            var playerOutfitsHandler = FindObjectsOfType<PlayerOutfitsHandler>();
            playerInfo.outfits = new List<string>();
            foreach(var resolver in playerOutfitsHandler[playerOutfitsHandler.GetLength(0) - 1].resolverList)
            {
                playerInfo.outfits.Add(resolver.GetLabel().ToString());
            }
        }
    }
}