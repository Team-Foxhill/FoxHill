using FoxHill.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FoxHill.Player.Inventory
{
    /// <summary>
    /// 플레이어의 인벤토리를 관리합니다.
    /// (주의) UI_Inventory의 최상단이 아닌 하위 계층 내의 Slots Gameobject에 위치합니다.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [HideInInspector] public UnityEvent<ItemData> OnUseRestorativeItem;
        [HideInInspector] public UnityEvent<ItemData> OnUseConstructiveItem;
        [HideInInspector] public UnityEvent<ItemData> OnUseQuestItem;

        [HideInInspector] public UnityEvent<int> OnGetItem;

        public Mode CurrentMode => _currentMode;
        public enum Mode
        {
            Switch,
            Selection
        }

        private const int MAX_SLOT = 12;
        private const int ROW_SIZE = 4;
        private const int COL_SIZE = 3;
        private readonly Color COLOR_EMPTY_SLOT = new Color(1f, 1f, 1f, 0f);

        private Canvas _outerCanvas;
        private Slot[] _slots = new Slot[MAX_SLOT];

        [SerializeField] private SelectionPopUp _popUp;
        [SerializeField] private ItemDescription _itemDescription;

        private Mode _currentMode = Mode.Switch;
        private int _currentSlotIndex = 0;

        private ReservedSlot _reservedSlot = new ReservedSlot();

        public struct ReservedSlot
        {
            public int Index;
            public int Amount;
        }

        private void Awake()
        {
            int index = 0;
            foreach (Transform slot in transform)
            {
                _slots[index] = slot.GetComponent<Slot>();
                _slots[index].BackgroundImage = slot.GetComponent<Image>();
                _slots[index].ItemImage = slot.GetChild(0).GetComponent<Image>();
                _slots[index].AmountText = slot.GetChild(1).GetComponent<TMP_Text>();

                index++;
            }

            _outerCanvas = transform.root.GetComponent<Canvas>();

            _popUp ??= transform.parent.Find("Selection").GetComponent<SelectionPopUp>();
            _itemDescription ??= transform.parent.Find("Description").GetComponent<ItemDescription>();
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
                _currentSlotIndex = 0;
                _currentMode = Mode.Switch;
                ToggleSlot(_currentSlotIndex, true);
                UpdateDescriptionUI(_slots[_currentSlotIndex]);
            }
            else // 인벤토리 닫을 때 수행할 동작
            {
                ToggleSlot(_currentSlotIndex, false);
                _popUp.Toggle(false);
            }
        }

        public void SwitchSlot(Vector2 input)
        {
            if (_currentMode == Mode.Switch)
            {
                if (input == Vector2.up)
                {
                    if (CheckRange(_currentSlotIndex - COL_SIZE) == true)
                    {
                        ToggleSlot(_currentSlotIndex, false);
                        _currentSlotIndex = _currentSlotIndex - COL_SIZE;
                        ToggleSlot(_currentSlotIndex, true);
                    }
                }
                else if (input == Vector2.down)
                {
                    if (CheckRange(_currentSlotIndex + COL_SIZE) == true)
                    {
                        ToggleSlot(_currentSlotIndex, false);
                        _currentSlotIndex = _currentSlotIndex + COL_SIZE;
                        ToggleSlot(_currentSlotIndex, true);
                    }
                }
                else if (input == Vector2.left)
                {
                    if (CheckRange(_currentSlotIndex - 1) == true)
                    {
                        ToggleSlot(_currentSlotIndex, false);
                        _currentSlotIndex = _currentSlotIndex - 1;
                        ToggleSlot(_currentSlotIndex, true);
                    }
                }
                else if (input == Vector2.right)
                {
                    if (CheckRange(_currentSlotIndex + 1) == true)
                    {
                        ToggleSlot(_currentSlotIndex, false);
                        _currentSlotIndex = _currentSlotIndex + 1;
                        ToggleSlot(_currentSlotIndex, true);
                    }
                }
                else
                {
                    return;
                }

                UpdateDescriptionUI(_slots[_currentSlotIndex]);

                bool CheckRange(int index)
                {
                    return (0 <= index && index < _slots.Length);
                }
            }

            else if (_currentMode == Mode.Selection)
            {
                if (input == Vector2.up)
                {
                    _popUp.SwitchOption(true);
                }
                else if (input == Vector2.down)
                {
                    _popUp.SwitchOption(false);
                }
            }
        }

        public void SelectSlot()
        {
            if (_currentMode == Mode.Switch) // 아이템 팝업 창 표시
            {
                if (_slots[_currentSlotIndex].Amount <= 0)
                {
                    return;
                }

                _currentMode = Mode.Selection;

                _popUp.Toggle(true);
                _popUp.SetPosition(_slots[_currentSlotIndex].transform.position);
            }
            else if (_currentMode == Mode.Selection)
            {
                if (_popUp.Selected == SelectionPopUp.Option.UseItem)
                {
                    UseItem(_slots[_currentSlotIndex]);

                    _currentMode = Mode.Switch;
                    _popUp.Toggle(false);
                }
                else if (_popUp.Selected == SelectionPopUp.Option.Cancel)
                {
                    DeselectSlot();
                }
            }
        }

        public void DeselectSlot()
        {
            if (_currentMode == Mode.Selection)
            {
                _popUp.Toggle(false);

                _currentMode = Mode.Switch;
            }
        }

        public bool PushItem(Items.Item newItem)
        {
            int indexToPush = -1;
            bool pushed = false;

            OnGetItem?.Invoke(newItem.Info.ItemNumber);

            for (int index = 0; index < MAX_SLOT; index++)
            {
                // 최초의 빈 slot 기억
                if (_slots[index].Amount == 0 && indexToPush == -1)
                {
                    indexToPush = index;
                }

                // 이미 같은 Item을 지닌 Slot이 있다면 Amount 증가
                if (_slots[index].Amount > 0 && _slots[index].ItemInfo.ItemNumber == newItem.Info.ItemNumber)
                {
                    _slots[index].Amount++;
                    pushed = true;

                    UpdateSlotUI(index);
                    return true;
                }
            }

            // 빈 Slot에 item 삽입
            if (pushed == false && indexToPush != -1)
            {
                _slots[indexToPush].ItemInfo = newItem.Info;
                _slots[indexToPush].ItemImage.sprite = newItem.Image;
                _slots[indexToPush].Amount++;

                UpdateSlotUI(indexToPush);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 특정 Slot의 아이템 정보 및 이미지를 UI에 표시합니다.
        /// </summary>
        /// <param name="index">UI에 표시할 Slot의 index</param>
        private void UpdateSlotUI(int index)
        {
            var slot = _slots[index];
            UpdateSlotUI(slot);
        }

        /// <summary>
        /// 특정 Slot의 아이템 정보 및 이미지를 UI에 표시합니다.
        /// </summary>
        /// <param name="slot">UI에 표시할 Slot</param>
        private void UpdateSlotUI(Slot slot)
        {
            if (slot.Amount > 0)
            {
                slot.ItemImage.color = Color.white;
                slot.AmountText.text = slot.Amount.ToString();
            }
            else
            {
                // slot.ItemImage.sprite = null;
                slot.ItemImage.color = COLOR_EMPTY_SLOT;
                slot.AmountText.text = string.Empty;
            }
        }

        private void UpdateDescriptionUI(Slot slot)
        {
            if (slot.Amount > 0)
                _itemDescription.UpdateDescription(slot.ItemInfo);
            else
                _itemDescription.ClearDescription();
        }

        /// <summary>
        /// 특정 Slot UI를 강조하여 사용자에게 현재 선택중인 Slot에 대한 정보를 제공합니다.
        /// </summary>
        /// <param name="toggle">True면 Activate, False면 Deactivate</param>
        private void ToggleSlot(int index, bool toggle)
        {
            _slots[index].BackgroundImage.color
                = (toggle == true)
                ? Color.red
                : Color.white;
        }

        /// <summary>
        /// 아이템을 사용합니다.
        /// 실제 게임 로직은 각 담당 클래스에서 수행합니다.
        /// </summary>
        /// <param name="item"></param>
        private void UseItem(Slot slot)
        {
            if (slot == null || slot.Amount <= 0)
            {
                return;
            }

            var item = slot.ItemInfo;
            ReserveSlot(slot);

            switch (item.ItemType)
            {
                case ItemType.RestorativeItem:
                    {
                        OnUseRestorativeItem?.Invoke(item);
                    }
                    break;
                case ItemType.ConstructiveItem:
                    {
                        OnUseConstructiveItem?.Invoke(item);
                    }
                    break;
                case ItemType.QuestItem:
                    {
                        OnUseQuestItem?.Invoke(item);
                    }
                    break;
            }

            slot.Amount--;
            UpdateSlotUI(slot);

            if (slot.Amount <= 0)
            {
                _itemDescription.ClearDescription();
            }
        }

        public bool HasItem(int itemIndex)
        {
            for(int i = 0; i < MAX_SLOT; i++)
            {
                if (_slots[i].Amount > 0 && _slots[i].ItemInfo.ItemNumber == itemIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public void UseItem(int itemIndex)
        {
            for (int i = 0; i < MAX_SLOT; i++)
            {
                if (_slots[i].Amount > 0 && _slots[i].ItemInfo.ItemNumber == itemIndex)
                {
                    UseItem(_slots[i]);
                }
            }
        }

        private void ReserveSlot(Slot slotToReserve)
        {
            _reservedSlot.Index = _currentSlotIndex;
            _reservedSlot.Amount = slotToReserve.Amount;
        }

        public void RestoreReservedSlot()
        {
            if (_slots[_reservedSlot.Index].Amount > 0)
            {
                _slots[_reservedSlot.Index].Amount++;

                UpdateSlotUI(_reservedSlot.Index);
            }
            else
            {
                _slots[_reservedSlot.Index].ItemImage.color = Color.white;
                _slots[_reservedSlot.Index].Amount++;

                UpdateSlotUI(_reservedSlot.Index);
            }
        }
    }
}