using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace ClientTemplate
{
    /// <summary>
    /// Dependency Injection으로 작업됨.
    /// 상황에 맞는 객체를 파라미터로 넘겨서 초기화하면
    /// 다른 코드의 수정없이, 간편하게 모든 동작을 원하는 버전으로 변경할 수 있음.
    /// </summary>
    public interface IManager
    {
        void Init();
        void Release();
        void ReSet();
    }

    /// <summary>
    /// Singleton 패턴으로 구현함.
    /// 코드의 다양한 부분에서 사용되어야 할 기능들을 UiManager, GamePlayManager에 나누어 구현할 예정.
    /// 단, UIManager와 GamePlayManager에 코드가 방대하게 몰리는 현상을 조심해야 함.
    /// </summary>
    public class MonoManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject instanceObject = new GameObject($"{typeof(T)}");
                    instance = instanceObject.AddComponent<T>();
                    DontDestroyOnLoad(instanceObject);
                }
                return instance;
            }
        }

        public void DestroySelf()
        {
            T selfGameObject = FindObjectOfType<T>();
            Destroy(selfGameObject.gameObject);
            instance = null;
        }
        
        public virtual void Init()
        {
        }

        public virtual void Release()
        {
        }

        public virtual void ReSet()
        {
        }
    }
}
