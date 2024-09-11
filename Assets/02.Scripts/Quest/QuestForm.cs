using System.Collections.Generic;

namespace FoxHill.Quest
{
    public enum ConditionType
    {
        CheckItem,
        CheckNotItem,
        MinPlayerLevel,
        MaxPlayerLevel,
        ClearQuest,
        NotClearQuest
    }

    public enum ObjectiveType
    {
        Collect,
        Kill,
        Deliver,
        Escort,
        Talk
    }

    public enum RewardType
    {
        Buff,
        Item,
        Exp
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
        public List<Condition> PreCondition;
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

    public class Condition
    {
        public ConditionType Type;
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