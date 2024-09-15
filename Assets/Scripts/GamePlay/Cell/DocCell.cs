using UnityEngine;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class DocCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt = null;

        public void SetInfo(string text)
        {
            txt.text = text;
            gameObject.SetActive(true);
        }
        public void OnBtnClicked()
        {
            gameObject.SetActive(false);
        }
    }
}