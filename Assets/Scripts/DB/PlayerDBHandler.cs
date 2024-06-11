using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

using DEMO.Manager;
using DEMO.UI;

namespace DEMO.DB
{
    public class PlayerDBHandler : DBMgr
    {
        [SerializeField] private TMP_Text playerNameTxt;
        [SerializeField] private TMP_Text playerPasswordTxt;
        [SerializeField] private PlayerInfo playerInfo;
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        private PanelManager panelManager;

        public void Login()
        {
            action = "login";
            StartCoroutine(SendData());
        }
        
        public void SignUp()
        {
            action = "signUp";
            StartCoroutine(SendData());
        }

        private new IEnumerator SendData()
        {
            SetPlayerName();
            SetPlayerPassword();

            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("Player_name", playerInfo.Player_name),
                new MultipartFormDataSection("Player_password", playerInfo.Player_password)
            };

            SetForm(formData, "Player", action);

            yield return StartCoroutine(base.SendData());

            string responseText = base.GetResponseText();
            JObject jsonResponse = JObject.Parse(responseText);

            if (!string.IsNullOrEmpty(responseText))
            {
                var status = jsonResponse["status"].ToString();

                Debug.Log(responseText);
                if (status == "Success")
                {
                    int Player_id = Int32.Parse(jsonResponse["Player_id"].ToString());
                    SetPlayerID(Player_id);

                    GameManager.playerInfo = playerInfo;
                    panelManager.OnActivePanel(panelList);
                }
                var message = jsonResponse["message"].ToString();
                Debug.Log(message);
            }
            else
            {
                Debug.Log("Error: No response from server.");
            }
        }

        private void Start()
        {
            panelManager = FindObjectOfType<PanelManager>();
        }


        private void SetPlayerID(int Player_id)
        {
            playerInfo.Player_id = Player_id;
        }

        public void SetPlayerName()
        {
            playerInfo.Player_name = playerNameTxt.text.Trim('\u200b');
        }

        public void SetPlayerPassword()
        {
            playerInfo.Player_password = playerPasswordTxt.text.Trim('\u200b');
        }
    }
}