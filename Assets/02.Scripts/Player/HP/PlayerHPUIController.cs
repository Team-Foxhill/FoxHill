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
        private readonly Color COLOR_DAMAGED = new Color(255f / 255f, 47f / 255f, 47f / 255f);

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Color _initialColor;
        private Vector3 _initialScale;

        private readonly WaitForSeconds _colorChangeWait = new WaitForSeconds(0.2f);

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
            _spriteRenderer.enabled = false;
        }

        public void OnPlayerDamaged(float HPRatio)
        {
            StartCoroutine(C_ChangeColor());

            if (HPRatio < 0f)
            {
                HPRatio = 0f;
            }

            transform.localScale = _initialScale * HPRatio;
        }

        public void OnPlayerHealed(float HPRatio)
        {
            if (HPRatio > 1f)
            {
                HPRatio = 1f;
            }

            transform.localScale = _initialScale * HPRatio;
        }

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = COLOR_DAMAGED;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
        }
    }
}
