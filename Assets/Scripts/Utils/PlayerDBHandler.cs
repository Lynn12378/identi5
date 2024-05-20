using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

using DEMO;

namespace DEMO.Utils
{
    public struct PlayerData
    {
        public int Player_id;
        public string Player_name;
        public string Player_password;
    }

    public class PlayerDBHandler : DBMgr
    {
        private GameManager gameManager = null;
        public PlayerData playerData;
        
        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private TMP_Text playerPasswordTxt = null;
        
        private void Start()
        {
            gameManager = GameManager.Instance;
        }

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

        public void SetPlayerName()
        {
            playerData.Player_name = playerNameTxt.text.Trim('\u200b');
        }

        public void SetPlayerPassword()
        {
            playerData.Player_password = playerPasswordTxt.text.Trim('\u200b');
        }

        private new IEnumerator SendData()
        {
            SetPlayerName();
            SetPlayerPassword();

            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("Player_name", playerData.Player_name),
                new MultipartFormDataSection("Player_password", playerData.Player_password)
            };

            SetForm(formData, "Player", action);

            yield return StartCoroutine(base.SendData());

            string responseText = base.GetResponseText();
            JObject jsonResponse = JObject.Parse(responseText);

            // new
            if (!string.IsNullOrEmpty(responseText))
            {
                var status = jsonResponse["status"].ToString();

                Debug.Log(responseText);
                if (status == "Success")
                {
                    // playerData.Player_id = (int)jsonResponse["Player_id"];
                    // playerData.Player_name = jsonResponse["Player_name"].ToString();
                }
                var message = jsonResponse["message"].ToString();
                Debug.Log(message);
            }
            else
            {
                Debug.Log("Error: No response from server.");
            }
        }
    }
}