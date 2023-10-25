using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace ClientTemplate
{
    public interface IManager
    {
        void Init();
        void Release();
        void ReSet();
    }

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
