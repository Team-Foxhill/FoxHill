using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Core.Effect
{
    public class EffectManager : MonoBehaviour
    {
        public enum FeedbackType
        {
            Impulse
        }

        private static Dictionary<FeedbackType, MMF_Player> _feedbacks = new Dictionary<FeedbackType, MMF_Player>();

        private void OnEnable()
        {
            _feedbacks.Clear();

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<MMF_Player>(out var player) == true)
                {
                    if (Enum.TryParse(child.name, out FeedbackType type) == true)
                    {
                        _feedbacks.Add(type, player);
                    }
                    else
                    {
                        DebugFox.LogError($"Failed to parse effect. Type : {type} / Player : {player}");
                    }
                }
            }
        }

        public static void Play(FeedbackType type)
        {
            if (_feedbacks.TryGetValue(type, out MMF_Player player) == true)
            {
                player.PlayFeedbacks();
            }
        }
    }
}