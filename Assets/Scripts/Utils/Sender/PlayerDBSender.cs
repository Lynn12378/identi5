using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

using Utils;
using Utils.Data;

namespace Utils.Sender
{
    public class PlayerDBSender : DataSender
    {
        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private TMP_Text playerPasswordTxt = null;
        private string action;

        public PlayerData playerData;

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
                    Debug.Log("Login successful!");
                }
                else
                {
                    var message = jsonResponse["message"].ToString();
                    Debug.Log(message);
                }
            }
            else
            {
                Debug.Log("Error: No response from server.");
            }
        }
    }
}