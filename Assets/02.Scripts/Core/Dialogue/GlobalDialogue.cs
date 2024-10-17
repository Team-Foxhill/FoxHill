using FoxHill.Core.Pause;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoxHill.Core.Dialogue
{
    public class GlobalDialogue : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _text;

        private const float TEXT_INTERVAL = 3f;
        private WaitForSeconds _intervalWait = new WaitForSeconds(TEXT_INTERVAL);

        private void Awake()
        {
            _canvas ??= GetComponentInParent<Canvas>();
            _text ??= GetComponentInChildren<TMP_Text>();   
        }

        private void Start()
        {
            ToggleUI(false);
        }

        public void ToggleUI(bool toggle)
        {
            _canvas.enabled = toggle;
        }

        public void StartDialogue(List<string> texts)
        {
            StartCoroutine(C_StartDialogue(texts));
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