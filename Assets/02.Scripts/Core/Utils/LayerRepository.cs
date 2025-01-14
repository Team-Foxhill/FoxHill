using UnityEngine;

namespace FoxHill.Core.Utils
{
    public static class LayerRepository
    {
        public static readonly int LAYER_PLAYER = LayerMask.NameToLayer("Player");
        public static readonly int LAYER_ITEM = LayerMask.NameToLayer("Item");
        public static readonly int LAYER_EXP = LayerMask.NameToLayer("Exp");
        public static readonly int LAYER_PATH_FOLLOW_MONSTER = LayerMask.NameToLayer("PathFollowMonster");
        public static readonly int LAYER_BOSS_MONSTER = LayerMask.NameToLayer("BossMonster");
    }
}