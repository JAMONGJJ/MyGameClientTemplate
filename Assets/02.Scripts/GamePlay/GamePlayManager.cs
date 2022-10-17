using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using KlayLand.ResourceInfo;
using UnityEngine;

namespace KlayLand
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
            CharacterInitPosition = GameObject.FindWithTag("CharacterInitPosition");
            if (CharacterInitPosition == null)
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Character init position object is null!");
            }
        }
        #endregion
        
        #region My Character Controller
        public async void SetMyPlayer()
        {
            string address = Core.System.Resource.GetAddressByType(PrefabAssetType.TestCharacterRig);
            if (string.IsNullOrEmpty(address) == false)
            {
                var handle = Core.System.Resource.LoadGameObject(address);
                UIManager.Instance.OpenWaitingWindow();
                while (handle.IsDone == false)
                {
                    await Task.Delay(10);
                }
                UIManager.Instance.CloseWaitingWindow();
                GameObject myCharacter = Instantiate(handle.Result, CharacterInitPosition.transform.position, Quaternion.identity);
                MyPlayerController = myCharacter.GetComponent<MyPlayerController>();
            }
            else
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Player character address is empty!");
            }
        }
        #endregion
        
        #region My Camera Controller
        public async void SetMyCamera()
        {
            string address = Core.System.Resource.GetAddressByType(PrefabAssetType.CameraObject);
            if (string.IsNullOrEmpty(address) == false)
            {
                var handle = Core.System.Resource.LoadGameObject(address);
                UIManager.Instance.OpenWaitingWindow();
                while (handle.IsDone == false)
                {
                    await Task.Delay(10);
                }
                UIManager.Instance.CloseWaitingWindow();
                GameObject myCamera = Instantiate(handle.Result);
                MyCameraController = myCamera.GetComponent<CameraController>();
                UIManager.Instance.SetOverlayCamera();
            }
            else
            {
                LogManager.LogError(LogManager.LogType.DEFAULT, "Camera object address is empty!");
            }
        }
        #endregion
        
        #region My Pet Controller

        public void SetMyPetController()
        {

        }

        #endregion
    }
}
