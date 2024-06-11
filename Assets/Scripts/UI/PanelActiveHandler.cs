using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMO.UI
{
    public class PanelActiveHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        private PanelManager panelManager;

        private void Start()
        {
            panelManager = FindObjectOfType<PanelManager>();
        }

        public void OnActivePanel()
        {
            panelManager.OnActivePanel(panelList);
        }
    }
}