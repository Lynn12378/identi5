using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Identi5
{
    public class PlayerOutputData : NetworkBehaviour
    {
        public int playerId;

        #region - Openness -
        public float outfitTime;
        public int oufitBoughtNo;
        public int placeholderNo;

        #endregion

        #region - Conscientiousness -
        public int failGame;
        public int deathNo;
        public int fullNo;
        public int organizeNo;
        public int zombieInShelteNo;
        public int killNo;
        public float surviveTime;
        #endregion

        #region - Extraversion -
        public int messageSent;
        public int teamCreated;
        public int quitTeamNo;

        #endregion

        #region - Agreeableness -
        public int joinTeamNo;
        public int giftNo;
        public int rankClikedNo;
        public int bulletOnLiving;
        public int interactNo;

        #endregion

        #region - Neuroticism -
        public int bulletOnCollisions; 
        public int collisionNo;
        public List<int> remainHP = new List<int>();
        public List<int> remainBullet = new List<int>();
        #endregion

        public float totalVoiceDetectionDuration;           // Duration of voice detected on player's mic
                                   // No. of placeholder items that player pick up
        public int repairQuantity;                          // Quantity of player given to repair shelter
        public int usePlaceholderNo;                        // No. of player use badge at right building
        public float durationOfRound;

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
