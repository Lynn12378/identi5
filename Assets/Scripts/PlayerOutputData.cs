using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Identi5
{
    public class PlayerOutputData : NetworkBehaviour
    {
        public int playerId = -1;

        #region - Openness -
        public float outfitTime;
        public int oufitChangedNo;
        public int placeholderNo;
        public List<int> buildingVisit = new List<int>();
        #endregion

        #region - Conscientiousness -
        public float manualTime;
        public int failGameNo;
        public int deathNo;
        public int killNo;
        public int organizeNo;
        public int fullNo;
        public int zombieInShelteNo;
        public float surviveTime;
        public int contribution;
        #endregion

        #region - Extraversion -
        public int messageSent;
        public int teamCreated;
        public int quitTeamNo;
        public float totalVoiceDetectionDuration;
        #endregion

        #region - Agreeableness -
        public int joinTeamNo;
        public int giftNo;
        public int rankClikedNo;
        public int bulletOnLiving;
        public int bulletOnPlayer;
        public int interactNo;

        #endregion

        #region - Neuroticism -
        public int collisionMapNo;
        public int bulletOnCollisions; 
        public List<int> remainHP = new List<int>();
        public List<int> remainBullet = new List<int>();
        #endregion
   
        #region - JSON -
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void FromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
        #endregion
    }
}
