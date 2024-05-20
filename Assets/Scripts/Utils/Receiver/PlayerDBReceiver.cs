using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

using Utils;

namespace Utils.Receiver
{
    public class PlayerDBReceiver : DBReceiver
    {
        private void Start()
        {
            StartCoroutine(ReceiveData());
        }

        private new IEnumerator ReceiveData()
        {
            // 設置資料表名稱
            SetTable("Player", "getAll");

           yield return StartCoroutine(base.ReceiveData());

            // 在這裡處理從 MySQL 中獲取的資料
        }
    }
}