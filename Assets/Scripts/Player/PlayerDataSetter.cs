using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using DEMO;

namespace DEMO.Lobby
{
    public class PlayerDataSetter : MonoBehaviour
    {
        private GameManager gameManager = null;
        private void Start()
        {
            gameManager = GameManager.Instance;
        }
        public void OnPlayerNameInputFieldChange(string value)
        {
            gameManager.PlayerName = value;

            gameManager.SetPlayerNetworkData();
        }
    }

}
