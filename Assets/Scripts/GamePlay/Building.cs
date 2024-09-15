using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Identi5.GamePlay
{
    public class Building : MonoBehaviour
    {
        public int buildingID;
        [SerializeField] private string doc;

        public string GetDoc()
        {
            return doc;
        }
    }
}