using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;

namespace Identi5.DB
{
    public class OutputDBHandler : DBMgr
    {
        public void UpdateOD()
        {
            action = "Update";
            StartCoroutine(SendData());
        }

        public new IEnumerator SendData()
        {
            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("PlayerOutputData", GameMgr.playerOutputData.ToJson()),
            };
            SetForm(formData, "OutputData", action);
            yield return StartCoroutine(base.SendData());
            var response = base.GetResponseText();
            Debug.Log(response);
        }

    }
}