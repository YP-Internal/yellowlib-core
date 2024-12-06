using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowPanda.Core
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }
    }
}
