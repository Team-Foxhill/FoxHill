using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Quest
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestSheet _sheet; // Quest ���� ��� ���� ��Ʈ�� ExcelImporter�� import�� ScriptableObject
        [SerializeField] private List<QuestForm> _forms; // ���� �Ľ� + �������� ���� Quest ������

        private void Awake()
        {
            InitializeQuestForms(_sheet);
        }

        private void InitializeQuestForms(QuestSheet sheet)
        {
            if (sheet == null)
            {
                throw new NullReferenceException("Cannot find sheet while InitializeQuestForms");                
            }

            _forms.Clear();

            foreach (var element in sheet.Sheet1)
            {
                _forms.Add(QuestFormConverter.Convert(element));
            }
        }
    }
}