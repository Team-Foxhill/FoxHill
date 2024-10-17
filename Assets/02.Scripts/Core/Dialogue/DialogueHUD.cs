using TMPro;
using UnityEngine;

namespace FoxHill.Core.Dialogue
{
    public class DialogueHUD : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
            _canvas ??= GetComponentInChildren<Canvas>();
            _text ??= GetComponentInChildren<TMP_Text>();
        }

        public void ToggleUI(bool toggle)
        {
            _spriteRenderer.enabled = toggle;
            _canvas.enabled = toggle;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}