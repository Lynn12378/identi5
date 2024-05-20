using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils{
    public class DBReceiver : MonoBehaviour
    {
        protected string table;
        protected string action;

        // 設置資料表名稱
        protected void SetTable(string table, string action)
        {
            this.table = table;
            this.action= action;
        }

        protected IEnumerator ReceiveData()
        {
            if (string.IsNullOrEmpty(table))
            {
                Debug.LogError("Data table name is not set.");
                yield break;
            }
            
            // PHP 腳本的網址
            string url = "http://localhost/"+ table +".php?Action=" + action;

            // 使用 UnityWebRequest 替代 WWW
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string json = www.downloadHandler.text;
                    Debug.Log("Received: " + json);
                }
                else
                {
                    Debug.Log("WWW Error: " + www.error);
                }
            }
        }
    }
}