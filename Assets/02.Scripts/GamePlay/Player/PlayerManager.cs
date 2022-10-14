using System.Collections;
using System.Collections.Generic;
using KlayLand.DataManufactureInfo;
using UnityEngine;
using KlayLand.PlayerInfo;


namespace KlayLand
{
    public partial class DataManufactureManager
    {
        private IParseOutput ParsePlayerInfo(IParseInput input, IParseHelper helper = null)
        {
            PlayerInfo_Mockup inputData = input as PlayerInfo_Mockup;
            MyPlayerInfo info = new MyPlayerInfo();
            info.nickName = inputData.PLAYER_NICKNAME;
            info.level = inputData.LEVEL;
            info.hairPartsId = inputData.HAIR_PARTS_ID;
            info.facePartsId = inputData.FACE_PARTS_ID;
            info.bodyPartsId = inputData.BODY_PARTS_ID;
            info.handPartsId = inputData.HAND_PARTS_ID;
            info.footPartsId = inputData.FOOT_PARTS_ID;
            return info;
        }
    }

    public class PlayerManager
    {
        
    }
}
