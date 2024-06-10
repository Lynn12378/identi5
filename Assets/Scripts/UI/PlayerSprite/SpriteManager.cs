using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DEMO.UI.PlayerSprite
{
    public class SpriteManager : MonoBehaviour
    {
        public List<GameObject> panelList = new List<GameObject>();
        public List<bool> boolList = new List<bool>();

        public void OnActivePanel(List<GameObject> list)
        {
            SetFalse();

            foreach(var obj in list)
            {
                int id = list.IndexOf(obj);
                boolList[id] = true;
                SetActive(list, true);
                
            }
        }

        private void SetFalse()
        {
            List<GameObject> list = new List<GameObject>();
            
            for( int i = 0; i < boolList.Count; i++)
            {
                if(boolList[i])
                {
                    list.Add(panelList[i]);
                }
            }

            SetActive(list, false);
        }

        private void SetActive(List<GameObject> list, bool _bool)
        {
            foreach(var panel in list)
            {
                panel.SetActive(_bool);
            }
        }
    }
}