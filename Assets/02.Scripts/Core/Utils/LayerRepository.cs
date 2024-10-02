using UnityEngine;

namespace FoxHill.Core.Utils
{
    public static class LayerRepository
    {
        public static readonly int LAYER_ITEM = LayerMask.NameToLayer("Item");
        public static readonly int LAYER_PATH_FOLLOW_MONSTER = LayerMask.NameToLayer("PathFollowMonster");
        public static readonly int LAYER_PATH_BOSS_MONSTER = LayerMask.NameToLayer("BossMonster");
    }
}