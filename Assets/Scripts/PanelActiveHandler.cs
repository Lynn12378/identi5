using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Identi5
{
    public class PanelActiveHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> panelList = new List<GameObject>();
        [SerializeField] private PanelMgr panelMgr = null;

        public void OnActivePanel()
        {
            if (panelMgr == null)
            {
                panelMgr = FindObjectOfType<PanelMgr>();
            }
            panelMgr.OnActivePanel(panelList);
        }
    }
}