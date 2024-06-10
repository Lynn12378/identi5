using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DEMO.UI.PlayerSprite
{
    public class ColorHandler : MonoBehaviour
    {
        public List<SpriteRenderer> skinSpriteList = new List<SpriteRenderer>();
        public List<SpriteRenderer> clothesSpriteList = new List<SpriteRenderer>();
        public List<SpriteRenderer> clothes2SpriteList = new List<SpriteRenderer>();
        public List<SpriteRenderer> clothes3SpriteList = new List<SpriteRenderer>();
        public List<SpriteRenderer> pantsSpriteList = new List<SpriteRenderer>();
        public List<SpriteRenderer> shoesSpriteList = new List<SpriteRenderer>();

        public HSVColorPicker colorPicker;
        public Color color;
        public Image btnImg;
        private Button colorBtn;

        public void SetSkinColor(Color color)
        {
            foreach(var sprite in skinSpriteList)
            {
                sprite.color = color;
            }
        }

        public void OnSkinColorBtnClicked()
        {
            colorPicker.OnSelectedColor += UpdateSelectedColor;
            colorPicker.selectedImg = btnImg;
            colorPicker.Init();
            colorPicker.obj.SetActive(true);
        }

        public void UpdateSelectedColor()
        {
            SetSkinColor(btnImg.color);
        }
    }
}