using System;
using UnityEngine;

namespace FoxHill.Player.Skill
{
    public abstract class SkillBase : MonoBehaviour, ISkill
    {
        public SkillModel Stat
        {
            get
            {
                if(_model == null)
                {
                    if (_data != null)
                    {
                        _model = new SkillModel(_data);
                    }
                    else
                    {
                        Debug.LogError($"Cannot find data in skill : {this}");
                    }
                }
                return _model;
            }
        }

        [SerializeField] protected SkillData _data;
        protected SkillModel _model { get; private set; } = null;

        protected virtual void Awake()
        {
            if(_data != null)
            {
                _model = new SkillModel(_data);
            }
            else
            {
                Debug.LogError($"Cannot find data in skill : {this}");
            }
        }

        public abstract void Cast();
    }
}