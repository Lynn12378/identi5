using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

using Identi5.DB;

namespace Identi5
{
    public class PlayerOutfitsHandler : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> skinSRD = new List<SpriteRenderer>();
        [SerializeField] private SpriteRenderer hairSRD;
        public List<SpriteResolver> resolverList = new List<SpriteResolver>();

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            resolverList.Clear();
            foreach(var resolver in GetComponentsInChildren<SpriteResolver>())
            {
                resolverList.Add(resolver);
            }
        }

        public void SetSkinColor(Color color)
        {
            foreach(var sprite in skinSRD)
            {
                sprite.color = color;
            }
        }

        public void SetHairColor(Color color)
        {
            hairSRD.color = color;
        }

        public void ChangeOutfit(string category,string label)
        {
            foreach(var resolver in resolverList)
            {
                if(resolver.GetCategory() == category)
                {
                    resolver.SetCategoryAndLabel(category, label);
                }
            }
        }
    }
}