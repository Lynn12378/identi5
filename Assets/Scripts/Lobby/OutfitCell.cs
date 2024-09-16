using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.Animation;

namespace Identi5.Lobby
{
    public class OutfitCell : MonoBehaviour
    {
        [SerializeField] private PlayerOutfitsHandler playerOutfitsHandler;
        private SpriteResolver resolver;
        private SpriteRenderer sprite;
        private Image image;

        public void OnClickOufitBtn()
        {
            playerOutfitsHandler.ChangeOutfit(resolver.GetCategory().ToString(),resolver.GetLabel().ToString());
        }

        private void Start()
        {
            resolver = GetComponent<SpriteResolver>();
            sprite = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            image.sprite = sprite.sprite;
        }
    }
}