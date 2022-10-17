using System.Collections;
using System.Collections.Generic;
using ClientTemplate.DataManufactureInfo;
using UnityEngine;

namespace ClientTemplate
{
    namespace PlayerInfo
    {
        public class PlayerInfo_Mockup : IParseInput
        {
            public PlayerInfo_Mockup()
            {
                PLAYER_NICKNAME = "Jimmy";
                LEVEL = 99999;
                HAIR_PARTS_ID = 10;
                FACE_PARTS_ID = 10;
                BODY_PARTS_ID = 10;
                HAND_PARTS_ID = 10;
                FOOT_PARTS_ID = 10;
            }

            public string PLAYER_NICKNAME;
            public long LEVEL;
            public long HAIR_PARTS_ID;
            public long FACE_PARTS_ID;
            public long BODY_PARTS_ID;
            public long HAND_PARTS_ID;
            public long FOOT_PARTS_ID;
        }

        public class MyPlayerInfo : IParseOutput
        {
            public string nickName;
            public long level;
            public long hairPartsId;
            public long facePartsId;
            public long bodyPartsId;
            public long handPartsId;
            public long footPartsId;
            
        }
    }
}
