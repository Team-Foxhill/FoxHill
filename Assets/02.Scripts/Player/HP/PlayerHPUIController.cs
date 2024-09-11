using System.Collections;
using UnityEngine;

namespace FoxHill.Player.HP
{
    /// <summary>
    /// 플레이어의 체력에 따른 HUD의 상태와 Animation을 관리합니다.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class PlayerHPUIController : MonoBehaviour
    {
        private readonly Color DAMAGED_COLOR = new Color(255f / 255f, 47f / 255f, 47f / 255f);

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Color _initialColor;
        private Vector3 _initialScale;

        private readonly WaitForSeconds _colorChangeWait = new WaitForSeconds(0.2f);

        private float _scaleMultiplier = 1f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _initialColor = _spriteRenderer.color;
            _initialScale = transform.localScale;
        }

        /// <summary>
        /// PlayerManager의 OnPlayerDead event에 구독시킬 action
        /// </summary>
        public void OnPlayerDead()
        {
            _animator.SetTrigger("Dead");
            transform.localScale = Vector3.one * 2.5f;
        }

        public void OnPlayerDamaged()
        {
            StartCoroutine(C_ChangeColor());
            _scaleMultiplier -= 0.1f;

            if (_scaleMultiplier < 0f)
            {
                _scaleMultiplier = 0f;
            }

            transform.localScale = _initialScale * _scaleMultiplier;
        }

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = DAMAGED_COLOR;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
        }
    }
}
