using System.Collections.Generic;
using UnityEngine;

namespace Identi5
{
    public class PlayerOutputData : MonoBehaviour
    {
        public int playerId;
        public bool isFinished;

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
        
        public void Init()
        {
            playerId = -1;
            isFinished = false;
            outfitTime = 0;
            oufitChangedNo = 0;
            placeholderNo = 0;
            buildingVisit = new List<int>();
            manualTime = 0;
            failGameNo = 0;
            deathNo = 0;
            killNo = 0;
            organizeNo = 0;
            fullNo = 0;
            zombieInShelteNo = 0;
            surviveTime = 0;
            contribution = 0;
            messageSent = 0;
            teamCreated = 0;
            quitTeamNo = 0;
            totalVoiceDetectionDuration = 0;
            joinTeamNo = 0;
            giftNo = 0;
            rankClikedNo = 0;
            bulletOnLiving = 0;
            bulletOnPlayer = 0;
            interactNo = 0;
            collisionMapNo = 0;
            bulletOnCollisions = 0; 
            remainHP = new List<int>();
            remainBullet = new List<int>();
        }
    }
}