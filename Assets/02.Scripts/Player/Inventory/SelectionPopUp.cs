using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Player.Inventory
{
    /// <summary>
    /// 슬롯을 select할 때 사용자에게 제공되는 팝업 UI를 관리합니다.
    /// </summary>
    public class SelectionPopUp : MonoBehaviour
    {
        public Option Selected => (Option)_currentIndex;

        public enum Option
        {
            UseItem,
            Cancel
        }

        private int _currentIndex = 0;

        [SerializeField] private Image[] _optionImages = new Image[2];
        private readonly Color COLOR_SELECTED = new Color(255f / 255f, 150f / 255f, 150f / 255f);

        private void Awake()
        {
            if (_optionImages[0] == null)
            {
                int index = 0;

                foreach (Transform child in transform)
                {
                    _optionImages[index++] = child.GetComponent<Image>();
                }
            }
        }

        private void Start()
        {
            Toggle(false);
        }

        public void Toggle(bool toggle)
        {
            gameObject.SetActive(toggle);

            if (toggle == true)
            {
                _currentIndex = 0;
                ToggleSlot(_currentIndex, true);
            }
            else
            {
                ToggleSlot(_currentIndex, false);
            }
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// 현재 선택 중인 옵션을 변경하고 사용자에게 표시합니다.
        /// </summary>
        /// <param name="isUpward">True면 상단의 옵션으로, False면 하단의 옵션으로 이동합니다.</param>
        public void SwitchOption(bool isUpward)
        {
            if (isUpward == true && CheckRange(_currentIndex - 1) == true)
            {
                ToggleSlot(_currentIndex, false);
                ToggleSlot(--_currentIndex, true);
            }

            else if (isUpward == false && CheckRange(_currentIndex + 1) == true)
            {
                ToggleSlot(_currentIndex, false);
                ToggleSlot(++_currentIndex, true);
            }

            bool CheckRange(int index)
            {
                return (0 <= index && index < _optionImages.Length);
            }
        }

        private void ToggleSlot(int index, bool toggle)
        {
            _optionImages[index].GetComponent<Image>().color
                = (toggle == true)
                ? COLOR_SELECTED
                : Color.white;
        }

    }
}
