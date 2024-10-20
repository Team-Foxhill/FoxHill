using FoxHill.Core.Pause;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace FoxHill.Core.Dialogue
{
    public class GlobalDialogue : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _text;

        private const float TEXT_INTERVAL = 2f;
        private WaitForSeconds _intervalWait = new WaitForSeconds(TEXT_INTERVAL);

        private void Awake()
        {
            _canvas ??= GetComponentInParent<Canvas>();
            _text ??= GetComponentInChildren<TMP_Text>();   
        }

        private void Start()
        {
            ToggleUI(false);
            switch (GameManager.Instance.Language.CurrentLanguage)
            {
                case Settings.LanguageManager.LanguageType.Korean:
                    {
                        _text.fontSize = 50;
                    }
                    break;
                case Settings.LanguageManager.LanguageType.English:
                    {
                        _text.fontSize = 40;
                    }
                    break;
            }
        }

        public void ToggleUI(bool toggle)
        {
            _canvas.enabled = toggle;
        }

        public void StartDialogue(List<string> texts)
        {
            //StartCoroutine(C_StartDialogue(texts));
        }

        private IEnumerator C_StartDialogue(List<string> texts)
        {
            ToggleUI(true);
            PauseManager.Pause();
            
            foreach(string text in texts)
            {
                _text.text = text;

                yield return _intervalWait;
            }

            ToggleUI(false);
            PauseManager.Resume();
        }
    }
}