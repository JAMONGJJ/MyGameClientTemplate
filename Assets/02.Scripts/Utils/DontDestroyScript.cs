using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KlayLand
{
    public class DontDestroyScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
