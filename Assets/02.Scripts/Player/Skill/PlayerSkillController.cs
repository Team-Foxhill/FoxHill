using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player.Skill
{
    public class PlayerSkillController : MonoBehaviour
    {
        public UnityEvent<int> OnCooldownComplete; // 파라미터(int) : 쿨다운이 끝난 스킬의 인덱스

        public bool IsSkillUIActivated { get; private set; } = false;

        private const int MAX_SKILL_COUNT = 4;
        private const float COOLDOWN_SWITCH_SKILL = 0.4f;
        private const float MAX_UI_ACTIVATION_TIME = 3f;

        [Header("Skill prefabs (Match UI order)")]
        [SerializeField] private List<SkillBase> _skillPrefabs = new List<SkillBase>(4);

        private List<SkillContainer> _skills = new List<SkillContainer>(4); // 실제로 게임 중에 주로 사용될 스킬 정보 리스트

        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private SkillUI _skillUI;

        private readonly WaitForSecondsRealtime _switchingCooldownWait = new WaitForSecondsRealtime(COOLDOWN_SWITCH_SKILL);
        private int _currentSkillIndex = 0;
        private bool _isRotating = false;

        private float _uiActivatedTime = 0f;

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

            private PlayerManager _playerManager;

            public SkillContainer(SkillBase skill, PlayerManager playerManager, PlayerSkillController outerClass)
            {
                _skill = skill;
                _playerManager = playerManager;
                _index = _skillCount++;
                _onCooldownComplete = outerClass.OnCooldownComplete;
            }

            public static void Initialize()
            {
                _skillCount = 0;
            }

            public IEnumerator C_StartCooldown()
            {
                _cooldown = _skill.Stat.Cooldown;

                while (_cooldown > 0f)
                {
                    if (_playerManager.IsPaused == true)
                    {
                        yield return new WaitUntil(() => _playerManager.IsPaused == false);
                    }

                    _cooldown -= Time.deltaTime;

                    yield return null;
                }

                _cooldown = 0f;
                _onCooldownComplete?.Invoke(_index);
            }
        }


        private void Awake()
        {
            _playerManager ??= GetComponentInParent<PlayerManager>();

            SkillContainer.Initialize();

            foreach (var skill in _skillPrefabs)
            {
                _skills.Add(new SkillContainer(skill, _playerManager, this));
            }
        }

        private void Start()
        {
            _playerManager.OnSwitchSkill?.AddListener(SwitchSkill);
            _playerManager.OnCastSkill?.AddListener(CastSkill);
            OnCooldownComplete?.AddListener(_skillUI.EnableIcon);

            _skillUI.ToggleUI(false);
        }

        /// <summary>
        /// 타당성 검사 수행 이후 UI를 켜고 끕니다.
        /// </summary>
        /// <param name="toggle">>True면 인벤토리를 켜고, False면 인벤토리를 끕니다.</param>
        public void ToggleSkillUI(bool toggle)
        {
            if (IsSkillUIActivated ^ toggle == true)
            {
                IsSkillUIActivated = toggle;
                _skillUI.ToggleUI(toggle);
            }
        }

        /// <summary>
        /// 사용자로부터 일정 시간 스킬 Switch / Use 입력을 받지 않으면 UI를 끕니다.
        /// 이를 위해 입력을 받지 않은 시간을 체크합니다.
        /// </summary>
        private IEnumerator C_CheckUIActivatedTime()
        {
            while (_uiActivatedTime < MAX_UI_ACTIVATION_TIME)
            {
                _uiActivatedTime += Time.deltaTime;
                yield return null;
            }

            ToggleSkillUI(false);
            _uiActivatedTime = 0f;
        }

        public void SwitchSkill()
        {
            if (IsSkillUIActivated == false)
            {
                ToggleSkillUI(true);
                StartCoroutine(C_CheckUIActivatedTime());
                return;
            }

            if (_isRotating == true)
                return;

            StartCoroutine(C_SwitchSkill());
            _uiActivatedTime = 0f;
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
            if (IsSkillUIActivated == false)
            {
                return;
            }

            if (_skills[_currentSkillIndex].Cooldown > 0f)
            {
                return;
            }

            var castedSkillGO = Instantiate(_skillPrefabs[_currentSkillIndex].gameObject, transform.root.position, Quaternion.identity);

            SkillParameter parameters = new SkillParameter
            {
                Direction = _playerManager.Direction,
                Power = _playerManager.Stat.Power,
                Transform = _playerManager.transform
            };

            castedSkillGO.GetComponent<ISkill>().Cast(parameters);
            _skillUI.Cast();

            _uiActivatedTime = 0f;
            StartCooldown(_skills[_currentSkillIndex]);
        }

        private void StartCooldown(SkillContainer skill)
        {
            StartCoroutine(skill.C_StartCooldown());
        }
    }
}
