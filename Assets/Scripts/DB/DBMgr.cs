using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Identi5.DB
{
    public class DBMgr : MonoBehaviour
    {
        protected List<IMultipartFormSection> formData;
        protected string table;
        protected string action;
        protected string responseText;

        protected void SetForm(List<IMultipartFormSection> formData, string table)
        {
            this.formData = formData;
            this.table = table;
        }

        protected void SetForm(List<IMultipartFormSection> formData, string table, string action)
        {
            this.formData = formData;
            this.table = table;
            this.action = action;
        }

        protected IEnumerator SendData()
        {
            var url = $"https://catfish-golden-man.ngrok-free.app/DEMO/{table}.php"+ (action != null ? $"?Action={action}" : "");
            UnityWebRequest www = UnityWebRequest.Post(url, formData);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data sent successfully!");
                responseText = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("Error: " + www.error);
                responseText = null;
            }
        }
        public string GetResponseText()
        {
            return responseText;
        }
    }
}