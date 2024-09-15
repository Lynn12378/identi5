using UnityEngine;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class DialogCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt = null;
        public static float deltaTime;
        private float timer = 0;

        public void SetInfo(string text)
        {
            txt.text = text;
            gameObject.SetActive(true);
        }
        void Update()
        {
            timer += Time.deltaTime;
            if(timer > 5 && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                timer = 0;
            }
        }
    }
}