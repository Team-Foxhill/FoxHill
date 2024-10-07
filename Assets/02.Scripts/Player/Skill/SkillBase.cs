using FoxHill.Core.Pause;
using System;
using UnityEngine;

namespace FoxHill.Player.Skill
{
    /// <summary>
    /// 스킬을 관리하는 추상 클래스이며, 세부 동작은 자식 클래스에서 정의합니다.
    /// 반드시 하위 게임오브젝트로 SpriteRender를 가진 Model이 필요합니다.
    /// </summary>
    public abstract class SkillBase : MonoBehaviour, ISkill, IPausable
    {
        public SkillModel Stat
        {
            get
            {
                if (_model == null)
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
        [SerializeField] protected Animator _animator;

        protected virtual void Awake()
        {
            if (_data != null)
            {
                _model = new SkillModel(_data);
                PauseManager.Register(this);
            }
            else
            {
                Debug.LogError($"Cannot find data in skill : {this}");
            }

            _animator ??= transform.Find("Model").GetComponent<Animator>();
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

            _animator.speed = 0f;
        }
        public virtual void Resume()
        {
            _isPaused = false;

            _animator.speed = 1f;
        }

        #endregion
    }
}