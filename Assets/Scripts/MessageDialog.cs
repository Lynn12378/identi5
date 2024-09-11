using UnityEngine;
using TMPro;

namespace Identi5
{
    public class MessageDialog : MonoBehaviour
    {
        [SerializeField] private TMP_Text txt = null;
        public static float deltaTime;
        private float timer=0;

        public void SetInfo(string message)
        {
            txt.text = message;
        }
        void Update()
        {
            timer += Time.deltaTime;
            if(timer > 5)
            {
                Destroy(gameObject);
            }
        }
    }
}