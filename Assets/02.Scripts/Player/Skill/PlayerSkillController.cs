using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;

namespace FoxHill.Player.Skill
{
    public class PlayerSkillController : MonoBehaviour
    {
        public UnityEvent<int> OnCooldownComplete; // 파라미터(int) : 쿨다운이 끝난 스킬의 인덱스

        public bool IsRotating => _isRotating;
        

        [SerializeField] private PlayerManager _playerManager;

        private const int MAX_SKILL_COUNT = 4;
        private const float COOLDOWN_SWITCH_SKILL = 0.5f;

        [Header("Skill prefabs (Match UI order)")]
        [SerializeField] private List<SkillBase> _skillPrefabs = new List<SkillBase>(4);

        private List<SkillContainer> _skills = new List<SkillContainer>(4); // 실제로 게임 중에 주로 사용될 스킬 정보 리스트

        [SerializeField] private SkillUI _skillUI;

        private int _currentSkillIndex = 0;

        private bool _isRotating = false;
        private readonly WaitForSeconds _switchingCooldownWait = new WaitForSeconds(COOLDOWN_SWITCH_SKILL);


        /// <summary>
        /// 스킬 정보와 현재 상태 등을 담은 내부 클래스.
        /// 인게임 중에 필요한 정보나 상태에 초점을 둡니다. (ex. 쿨타임, 스킬 레벨)
        /// </summary>
        public class SkillContainer
        {
            public ISkill Skill => _skill;
            public float Cooldown => _cooldown;
            
            private ISkill _skill;
            private float _cooldown = 0f;
            private UnityEvent<int> _onCooldownComplete;

            private int _index = 0;
            private static int _skillCount = 0;

            public SkillContainer(SkillBase skill, PlayerSkillController outerClass)
            {
                _skill = skill;
                _index = _skillCount++;
                _onCooldownComplete = outerClass.OnCooldownComplete;
            }

            public IEnumerator C_StartCooldown()
            {
                _cooldown = _skill.Stat.Cooldown;

                while (_cooldown > 0f)
                {
                    _cooldown -= Time.deltaTime;
                    yield return null;
                }

                _cooldown = 0f;
                _onCooldownComplete?.Invoke(_index);
            }
        }


        private void Awake()
        {
            if (_playerManager == null)
                _playerManager = GetComponentInParent<PlayerManager>();

            foreach (var skill in _skillPrefabs)
            {
                _skills.Add(new SkillContainer(skill, this));
            }
        }

        private void Start()
        {
            _playerManager.OnSwitchSkill?.AddListener(SwitchSkill);
            _playerManager.OnCastSkill?.AddListener(CastSkill);
            OnCooldownComplete?.AddListener(_skillUI.EnableIcon);
        }

        public void Initialize(SkillUI ui)
        {
            _skillUI = ui;
        }

        public void SwitchSkill()
        {
            if (_isRotating == true)
                return;

            StartCoroutine(C_SwitchSkill());
        }

        private IEnumerator C_SwitchSkill()
        {
            _isRotating = true;

            _currentSkillIndex = (_currentSkillIndex + 1) % MAX_SKILL_COUNT;
            _skillUI.SwitchSkill();

            yield return _switchingCooldownWait;

            _isRotating = false;
        }

        /// <summary>
        /// 스킬 프리팹의 스킬을 런타임 중에 생성하고 Skill을 사용합니다.
        /// 로직에 따른 스킬 동작은 각 Skill Class에서 담당합니다.
        /// </summary>
        public void CastSkill()
        {
            if (_skills[_currentSkillIndex].Cooldown > 0f)
            {
                return;
            }

            var castedSkillGO = Instantiate(_skillPrefabs[_currentSkillIndex].gameObject, transform.root.position, Quaternion.identity);
            castedSkillGO.GetComponent<ISkill>().Cast();
            _skillUI.Cast();

            StartCooldown(_skills[_currentSkillIndex]);
        }

        private void StartCooldown(SkillContainer skill)
        {
            StartCoroutine(skill.C_StartCooldown());
        }
    }
}
