using UnityEngine;
using UnityEngine.UI;
namespace Identi5
{
    public class AudioHandler : MonoBehaviour
    {
        #region - Audio -
            public Slider BGMSlider;
            public Slider AudioSlider;
            private void Start()
            {
                BGMSlider.onValueChanged.AddListener (delegate {BGMChangeCheck ();});
                AudioSlider.onValueChanged.AddListener (delegate {AudioChangeCheck ();});
            }
            public void BGMChangeCheck()
            {
                GameMgr.Instance.BGM.volume = BGMSlider.value;
            }
            public void AudioChangeCheck()
            {
                GameMgr.Instance.source.volume = AudioSlider.value;
            }
        #endregion
    }
}