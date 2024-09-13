using System;
using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill
{
    public class Skill_VermilionBird : SkillBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast()
        {
            StartCoroutine(C_TESTMOVE());
        }

        // test code
        private IEnumerator C_TESTMOVE()
        {
            float elapsedTime = 0f;
            while (elapsedTime < 1.5f)
            {
                gameObject.transform.Translate(Vector3.left * 2f * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}