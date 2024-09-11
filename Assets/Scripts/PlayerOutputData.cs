using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Identi5
{
    public class PlayerOutputData : NetworkBehaviour
    {
        public int playerId;
        public float outfitTime;
        public int killNo;                                  // No. of enemy killed
        public int deathNo;                                 // No. of death
        public float surviveTime;                           // Longest survival time
        public int collisionNo;                             // No. of player's collision with buildings
        public int bulletCollision;                         // No. of player bullet's collision with buildings
        public int bulletCollisionOnLiving;                 // No. of player shoot another player or animals              
        public List<int> remainHP = new List<int>();            // HP amount remained when refill HP
        public List<int> remainBullet = new List<int>();        // Bullet amount remained when refill bullet
        public float totalVoiceDetectionDuration;           // Duration of voice detected on player's mic
        public int organizeNo;                              // No. of player organize inventory
        public int fullNo;                                  // No. of player's inventory full
        public int placeholderNo;                           // No. of placeholder items that player pick up
        public int rankNo;                                  // No. of player open rank panel
        public int giftNo;                                  // No. of player gift items to others
        public int createTeamNo;                            // No. of player create new team
        public int joinTeamNo;                              // No. of player join team
        public int quitTeamNo;                              // No. of player quit team
        public int repairQuantity;                          // Quantity of player given to repair shelter
        public int restartNo;                               // No. of times shelter durability = 0
        public int usePlaceholderNo;                        // No. of player use badge at right building
        public int petNo = 0;                                // No. of player pet livings
        public int sendMessageNo;                           // No. of player send text message
        public float durationOfRound;
        
        #region - JSON -
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        #endregion
    }
}
