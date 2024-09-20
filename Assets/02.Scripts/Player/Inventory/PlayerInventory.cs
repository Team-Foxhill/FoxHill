using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Player.Inventory
{
    /// <summary>
    /// 플레이어의 인벤토리를 관리합니다.
    /// (주의) UI_Inventory의 최상단이 아닌 하위 계층 내의 Slots Gameobject에 위치합니다.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public Mode CurrentMode => _currentMode;

        private const int MAX_SLOT = 12;
        private const int ROW_SIZE = 4;
        private const int COL_SIZE = 3;

        public enum Mode
        {
            Switch,
            Selection
        }

        private Canvas _outerCanvas;
        private Slot[] _slots = new Slot[MAX_SLOT];
        [SerializeField] private SelectionPopUp _popUp;

        private int _currentSlot = 0;
        private Mode _currentMode = Mode.Switch;

        private void Awake()
        {
            int index = 0;
            foreach (Transform slot in transform)
            {
                _slots[index++] = slot.GetComponent<Slot>();
            }

            _outerCanvas = transform.root.GetComponent<Canvas>();
            
            if (_popUp == null)
                _popUp = transform.parent.Find("Selection").GetComponent<SelectionPopUp>();
        }

        private void Start()
        {
            ToggleCanvas(false);
        }

        public void ToggleCanvas(bool toggle)
        {
            _outerCanvas.enabled = toggle;

            if (toggle == true) // 인벤토리 열 때 수행할 동작
            {
                _currentSlot = 0;
                _currentMode = Mode.Switch;
                ToggleSlot(_currentSlot, true);
            }
            else // 인벤토리 닫을 때 수행할 동작
            {
                ToggleSlot(_currentSlot, false);
            }
        }

        public void SwitchSlot(Vector2 input)
        {
            if (_currentMode == Mode.Switch)
            {

                if (input == Vector2.up)
                {
                    if (CheckRange(_currentSlot - COL_SIZE) == true)
                    {
                        ToggleSlot(_currentSlot, false);
                        _currentSlot = _currentSlot - COL_SIZE;
                        ToggleSlot(_currentSlot, true);
                    }
                }
                else if (input == Vector2.down)
                {
                    if (CheckRange(_currentSlot + COL_SIZE) == true)
                    {
                        ToggleSlot(_currentSlot, false);
                        _currentSlot = _currentSlot + COL_SIZE;
                        ToggleSlot(_currentSlot, true);
                    }
                }
                else if (input == Vector2.left)
                {
                    if (CheckRange(_currentSlot - 1) == true)
                    {
                        ToggleSlot(_currentSlot, false);
                        _currentSlot = _currentSlot - 1;
                        ToggleSlot(_currentSlot, true);
                    }
                }
                else if (input == Vector2.right)
                {
                    if (CheckRange(_currentSlot + 1) == true)
                    {
                        ToggleSlot(_currentSlot, false);
                        _currentSlot = _currentSlot + 1;
                        ToggleSlot(_currentSlot, true);
                    }
                }
                else
                {
                    return;
                }

                bool CheckRange(int index)
                {
                    return (0 <= index && index < _slots.Length);
                }
            }

            else if(_currentMode == Mode.Selection)
            {
                if(input == Vector2.up)
                {
                    _popUp.SwitchOption(true);
                }
                else if(input == Vector2.down)
                {
                    _popUp.SwitchOption(false);
                }
            }
        }

        public void SelectSlot()
        {
            if (_currentMode == Mode.Switch) // 아이템 팝업 창 표시
            {
                _currentMode = Mode.Selection;

                _popUp.Toggle(true);
                _popUp.SetPosition(_slots[_currentSlot].transform.position);
            }
            else if (_currentMode == Mode.Selection) 
            {
                if(_popUp.Selected == SelectionPopUp.Option.UseItem)
                {
                    Debug.Log($"Use Item in slot {_currentSlot}");
                    // TODO : 선택한 아이템 사용
                }
                else if (_popUp.Selected == SelectionPopUp.Option.Cancel)
                {
                    DeselectSlot();
                }
            }
        }

        public void DeselectSlot()
        {
            if(_currentMode == Mode.Selection)
            {
                _popUp.Toggle(false);

                _currentMode = Mode.Switch;
            }
        }

        /// <summary>
        /// 특정 Slot UI를 강조하여 사용자에게 현재 선택중인 Slot에 대한 정보를 제공합니다.
        /// </summary>
        /// <param name="toggle">True면 Activate, False면 Deactivate</param>
        private void ToggleSlot(int index, bool toggle)
        {
            _slots[index].GetComponent<Image>().color
                = (toggle == true)
                ? Color.red
                : Color.white;
        }
    }
}