using FoxHill.Core.Pause;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FoxHill.UI
{
    public class TutorialUI : MonoBehaviour, TutorialInputAction.ITutorialActionActions
    {
        public class InfoCard
        {
            public GameObject Root;
            public Image Image;
            public TMP_Text Text;
        }

        private TutorialInputAction _inputAction;

        private List<InfoCard> _infoCards = new List<InfoCard>(10);
        private int _currentIndex = 0;

        private void OnDestroy()
        {
            _inputAction?.Dispose();
        }

        public void Initialize()
        {
            _inputAction = new TutorialInputAction();
            _inputAction.TutorialAction.AddCallbacks(this);

            _infoCards.Clear();
            // Initialize
            foreach (Transform child in transform)
            {
                var newInfo = new InfoCard();
                newInfo.Root = child.gameObject;
                newInfo.Image = child.Find("Image").GetComponent<Image>();
                newInfo.Text = child.Find("Text (TMP)").GetComponent<TMP_Text>();

                _infoCards.Add(newInfo);
                HideCard(newInfo);
            }
        }

        public void Show()
        {
            PauseManager.Pause(true);

            _inputAction.TutorialAction.Enable();

            HideCard();
            _currentIndex = 0;
            ShowCard();
        }

        public void Hide()
        {
            _inputAction.TutorialAction.Disable();
            HideCard();

            PauseManager.Resume(true);
        }

        public void ShowCard()
        {
            _infoCards[_currentIndex].Root.SetActive(true);
        }

        public void ShowCard(InfoCard card)
        {
            card.Root.SetActive(true);
        }

        public void HideCard()
        {
            _infoCards[_currentIndex].Root.SetActive(false);
        }

        public void HideCard(InfoCard card)
        {
            card.Root.SetActive(false);
        }

        public void Swipe(bool isLeftward)
        {
            if (isLeftward == true && _currentIndex == 0)
                return;

            if (isLeftward == false && _currentIndex == _infoCards.Count - 1)
                return;

            HideCard();
            if(isLeftward == true)
            {
                _currentIndex--;
            }
            else
            {
                _currentIndex++;
            }
            ShowCard();
        }

        public void OnSwipe(InputAction.CallbackContext context)
        {
            if (context.started == true)
            {
                var input = context.ReadValue<Vector2>();

                if (input == Vector2.left)
                {
                    Swipe(true);
                }
                else if (input == Vector2.right)
                {
                    Swipe(false);
                }
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if(context.started == true)
            {
                Hide();
            }
        }
    }
}