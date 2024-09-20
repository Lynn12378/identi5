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
            timer = 0;
            txt.text = text;
            gameObject.SetActive(true);
        }
        void Update()
        {
            timer += Time.deltaTime;
            if(timer > 3 && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                timer = 0;
            }
        }
    }
}