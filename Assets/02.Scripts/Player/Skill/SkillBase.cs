using FoxHill.Core.Pause;
using System;
using UnityEngine;

namespace FoxHill.Player.Skill
{
    public abstract class SkillBase : MonoBehaviour, ISkill, IPausable
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
                PauseManager.Register(this);
            }
            else
            {
                Debug.LogError($"Cannot find data in skill : {this}");
            }
        }

        protected virtual void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        public abstract void Cast(SkillParameter parameters);


        #region Pausable
        protected bool _isPaused = false;

        public virtual void Pause()
        {
            _isPaused = true;
        }
        public virtual void Resume()
        {
            _isPaused = false;
        }

        #endregion
    }
}