using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Quest
{
    [ExcelAsset]
    public class QuestSheet : ScriptableObject
    {
        //public List<EntityType> Sheet1; // Replace 'EntityType' to an actual type that is serializable.
        public List<QuestFormRaw> Sheet1;
    }
}