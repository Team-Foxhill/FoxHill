using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Core
{
    public static class Debugger
    {
        public static void Log(object input)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(input);
#endif
        }
    }
}