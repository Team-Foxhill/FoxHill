using UnityEngine;
using UnityEditor;
using FoxHill.Monster;
using System.Collections.Generic;
using FoxHill.Core.Damage;
using FoxHill.Core;

namespace FoxHill.Editor
{
#if UNITY_EDITOR
public class DamageTester : EditorWindow
{
        private MonoBehaviour _damageableGameObject;
        private float _damageAmount;

        private void OnGUI()
        {
            DrawGiveDamagePanel();
        }

        [MenuItem("CustomTools/Damage/Run DamageTester %m")]
        public static void Run()
        {
            GetWindow<DamageTester>();
        }

        private void DrawGiveDamagePanel()
        {
            _damageableGameObject = (MonoBehaviour)EditorGUILayout.ObjectField("Damageable Object", _damageableGameObject, typeof(MonoBehaviour), true);
            _damageAmount = EditorGUILayout.FloatField("DamageAmount", _damageAmount);
            if (_damageableGameObject != null && _damageableGameObject.GetComponent<PathFollowMonsterController>() is IDamageable)
            {
                EditorGUILayout.LabelField("Selected object implements IDamageable", EditorStyles.boldLabel);
            }
            else if (_damageableGameObject != null)
            {
                EditorGUILayout.HelpBox("Selected object does not implement IDamageable", MessageType.Warning);
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("GiveDamage", GUILayout.Width(150f), GUILayout.Height(50f)) == true)
            {
                if (_damageableGameObject.GetComponent<PathFollowMonsterController>() is IDamageable damageable)
                {
                    damageable.TakeDamage(null, _damageAmount);
                }
                else
                {
                    DebugFox.LogError("Please put IDamageable GameObject");
                }
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}