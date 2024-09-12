using System.Collections.Generic;

namespace FoxHill.Quest
{
    public enum PreConditionType
    {
        HaveItem,
        NotHaveItem,
        MinPlayerLevel,
        MaxPlayerLevel,
        ClearQuest,
        NotClearQuest
    }

    public enum ObjectiveType
    {
        Collect, // 4
        Kill, // 2
        Deliver, // 3
        Escort, // 5
        Talk // 1
    }

    public enum RewardType
    {
        Buff, // 3
        Item, // 2
        Exp // 1
    }

    public enum PenaltyType
    {
        Spawn,
        Buff,
        Item,
        Exp
    }

    /// <summary>
    /// Excel Importer 사용하기 위해 자료형 변경한 클래스
    /// 실제 게임 내에서는 QuestForm class로 변환하여 사용
    /// </summary>
    [System.Serializable]
    public class QuestFormRaw
    {
        public int QuestNumber;
        public float QuestShowTime;
        public string PreCondition;
        public int StartNPC;
        public string Objective;
        public float TimeLimit;
        public int EndNPC;
        public string StartDialogue;
        public string ProgressDialogue;
        public string SuccessDialogue;
        public string FailDialogue;
        public string Reward;
        public string Penalty;
    }

    [System.Serializable]
    public class QuestForm
    {
        public int QuestNumber;
        public float QuestShowTime;
        public List<PreCondition> PreCondition;
        public int StartNPC;
        public List<Objective> Objective;
        public float TimeLimit;
        public int EndNPC;
        public List<string> StartDialogue;
        public List<string> ProgressDialogue;
        public List<string> SuccessDialogue;
        public List<string> FailDialogue;
        public List<Reward> Reward;
        public List<Penalty> Penalty;
    }

    public class PreCondition
    {
        public PreConditionType Type;
        public int Value;
    }

    public class Objective
    {
        public ObjectiveType Type;
        public float Value1;
        public float Value2;
    }

    public class Reward
    {
        public RewardType Type;
        public float Value1;
        public float Value2;
    }

    public class Penalty
    {
        public PenaltyType Type;
        public float Value1;
        public float Value2;
    }
}