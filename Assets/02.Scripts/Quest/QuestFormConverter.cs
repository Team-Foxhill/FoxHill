using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoxHill.Quest
{
    /// <summary>
    /// Sheet를 ExcelImporter로 import하여 얻은 QuestFormRaw 형태의 sheet를 게임 내에서 사용하기 위한 QuestForm 형태로 Convert하는 클래스
    /// </summary>
    public static class QuestFormConverter
    {
        public static QuestForm Convert(QuestFormRaw sheet)
        {
            return new QuestForm
            {
                QuestNumber = sheet.QuestNumber,
                QuestShowTime = sheet.QuestShowTime,
                PreCondition = ParseCondition(sheet.PreCondition.Split('\n').ToList()),
                StartNPC = sheet.StartNPC,
                Objective = ParseComponents<Objective, ObjectiveType>(sheet.Objective.Split('\n').ToList()),
                TimeLimit = sheet.TimeLimit,
                EndNPC = sheet.EndNPC,
                StartDialogue = sheet.StartDialogue.Split('\n').ToList(),
                ProgressDialogue = sheet.ProgressDialogue.Split('\n').ToList(),
                SuccessDialogue = sheet.SuccessDialogue.Split('\n').ToList(),
                FailDialogue = sheet.FailDialogue.Split('\n').ToList(),
                Reward = ParseComponents<Reward, RewardType>(sheet.Reward.Split('\n').ToList()),
                Penalty = ParseComponents<Penalty, PenaltyType>(sheet.Penalty.Split('\n').ToList())
            };
        }

        private static List<Condition> ParseCondition(List<string> conditions)
        {
            return conditions.Select(condition =>
            {
                var parts = condition.Split('(');

                if (parts.Length != 2
                    || Enum.TryParse(parts[0], out ConditionType conditionType) == false
                    || int.TryParse(parts[1].Split(')')[0], out int parameter) == false)
                {
                    Debug.LogWarning($"Failed to parse condition: {condition}");
                    return null;
                }

                return new Condition
                {
                    Type = conditionType,
                    Value = parameter
                };
            }).Where(c => c != null).ToList();
        }

        private static List<T> ParseComponents<T, TEnum>(List<string> components)
            where TEnum : struct
        {
            return components.Select(component =>
            {
                var parts = component.Split('(');
                if (parts.Length != 2
                    || Enum.TryParse(parts[0], out TEnum componentType) == false)
                {
                    Debug.LogWarning($"Failed to parse {typeof(T).Name}: {component}");
                    return default;
                }

                var parameters = parts[1].Split(')')[0].Split(',');
                parameters = (string.IsNullOrWhiteSpace(parameters[0])) ? Array.Empty<string>() : parameters;

                if (parameters.Length > 2)
                {
                    Debug.LogWarning($"Invalid number of parameters in {typeof(T).Name}: {component}");
                    return default;
                }

                T newComponent;
                try
                {
                    newComponent = (T)Activator.CreateInstance(typeof(T));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create instance of {typeof(T).Name}: {ex.Message}");
                    return default;
                }

                var typeField = typeof(T).GetField("Type");
                var value1Field = typeof(T).GetField("Value1");
                var value2Field = typeof(T).GetField("Value2");

                if (typeField == null)
                {
                    Debug.LogWarning($"Invalid property structure in {typeof(T).Name}");
                    return default;
                }

                typeField.SetValue(newComponent, componentType);

                if (parameters.Length == 0)
                {
                    return newComponent;
                }

                if (float.TryParse(parameters[0], out float value1))
                {
                    value1Field.SetValue(newComponent, value1);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse Value1 in {typeof(T).Name}: {component}");
                    return default;
                }

                if (parameters.Length == 2 && value2Field != null)
                {
                    if (float.TryParse(parameters[1], out float value2))
                    {
                        value2Field.SetValue(newComponent, value2);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse Value2 in {typeof(T).Name}: {component}");
                    }
                }

                return newComponent;
            }).ToList();
        }
    }
}