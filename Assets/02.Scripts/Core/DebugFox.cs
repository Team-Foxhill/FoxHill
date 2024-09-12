using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Core
{
    public static class DebugFox
    {
        public static void Log(object input)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(input);
#endif
        }
        public static void LogError(object input)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(input);
#endif
        }
        public static void LogWarning(object input)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(input);
#endif
        }
    }
}