using UnityEngine;
using TMPro;

namespace Identi5.GamePlay.Cell
{
    public class RankCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameTxt = null;
        [SerializeField] private TMP_Text scoreTxt = null;
        [SerializeField] private TMP_Text rankTxt = null;

        public void SetInfo(string playerName,float score, int rank)
        {
            playerNameTxt.text = playerName;
            scoreTxt.text = score.ToString();
            rankTxt.text = $"排名:{rank}";
        }
    }
}