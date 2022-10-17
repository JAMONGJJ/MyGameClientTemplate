using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTemplate.ResourceInfo;
using UnityEngine;

namespace ClientTemplate
{
    public partial class GamePlayManager : MonoManager<GamePlayManager>
    {
        public GameObject CharacterInitPosition { get; private set; }
        public MyPlayerController MyPlayerController { get; private set; }
        public CameraController MyCameraController { get; private set; }

        public override void Init()
        {
            
        }

        public override void Release()
        {

        }

        public override void ReSet()
        {

        }
        
        #region Character Init Position
        public void SetCharacterInitPosition()
        {
            
        }
        #endregion
        
        #region My Character Controller
        public async void SetMyPlayer()
        {
            
        }
        #endregion
        
        #region My Camera Controller
        public async void SetMyCamera()
        {
            
        }
        #endregion
        
        #region My Pet Controller

        public void SetMyPetController()
        {

        }

        #endregion
    }
}
