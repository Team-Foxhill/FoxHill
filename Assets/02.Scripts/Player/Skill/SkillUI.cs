using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Player.Skill
{
    public class SkillUI : MonoBehaviour
    {
        #region Rotation
        private const float ROTATION_Z_AMOUNT = -90f;
        private const float ROTATION_Z_SPEED = 3f;
        #endregion


        #region Icon
        private const int MAX_SKILL_COUNT = 4;

        private List<SpriteRenderer> _icons = new List<SpriteRenderer>(4);
        private int _iconIndex = 0;
        #endregion

        private void Awake()
        {
            foreach (Transform skillUI in transform)
            {
                _icons.Add(skillUI.GetComponent<SpriteRenderer>());
            }
        }

        public void Cast()
        {
            Debug.Log(_iconIndex);

            DisableIcon(_iconIndex);
        }

        public void SwitchSkill()
        {
            _iconIndex = (_iconIndex + 1) % MAX_SKILL_COUNT;

            StartCoroutine(C_SwitchSkill());
        }

        private IEnumerator C_SwitchSkill()
        {
            var rotationBuffer = transform.rotation.eulerAngles;
            float initialRotationZ = rotationBuffer.z;

            while (rotationBuffer.z - initialRotationZ > ROTATION_Z_AMOUNT) // ROTATION_Z_AMOUNT만큼 회전
            {
                rotationBuffer.z
                    = (rotationBuffer.z - initialRotationZ > ROTATION_Z_AMOUNT)
                    ? rotationBuffer.z - ROTATION_Z_SPEED
                    : initialRotationZ - ROTATION_Z_AMOUNT;

                transform.rotation = Quaternion.Euler(rotationBuffer);

                yield return null;
            }
        }


        public void EnableIcon(int index)
        {
            _icons[_iconIndex].color = Color.white;
        }

        public void DisableIcon(int index)
        {
            _icons[index].color = Color.gray;
        }
    }
}
