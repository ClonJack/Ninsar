using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ninsar.Inventory
{
    public class DebugButtonView : MonoBehaviour
    {
        [Header("Settings")]
        [field: SerializeField]
        public int Count { get; private set; }
        
        [field: SerializeField]
        public BaseItem Item { get; private set; }
        
        [field: SerializeField]
        public EActionTypeButton ETypeButton { get; private set; }
        
        [Header("References")]
        [field: SerializeField]
        public TextMeshProUGUI Label { get; private set; }
        
        public event Action<DebugButtonView> OnClick;
        private Button _button;

        public void Awake()
        {
            _button = GetComponent<Button>();
            UpdateLabel();
            _button.onClick.AddListener(HandleClick);
        }
        
        public void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClick);
        }
        public void OnValidate()
        {
            UpdateLabel();
        }
        private void HandleClick() 
            => OnClick?.Invoke(this);
        private void UpdateLabel()
        {
            if (Label == null)
                return;

            switch (ETypeButton)
            {
                case EActionTypeButton.Add:
                    Label.text = Item != null ? $"ADD: {Item.Name} x{Count}" : "ADD: ?";
                    break;
                case EActionTypeButton.Remove:
                    Label.text = Item != null ? $"REMOVE: {Item.Name} x{Mathf.Abs(Count)}" : "REMOVE: ?";
                    break;
                case EActionTypeButton.RemoveSelect:
                    Label.text = $"REMOVE SELECT ITEM";
                    break;
                case EActionTypeButton.FilterAll:
                    Label.text = "FILTER: ALL";
                    break;
                case EActionTypeButton.FilterEquipment:
                    Label.text = "FILTER: EQUIPMENT";
                    break;
                case EActionTypeButton.FilterConsumable:
                    Label.text = "FILTER: CONSUMABLE";
                    break;
                case EActionTypeButton.FilterQuest:
                    Label.text = "FILTER: QUEST";
                    break;
                case EActionTypeButton.ChangeCapacity:
                    Label.text = $"CHANGE CAPACITY ON {Count}";
                    break;
                case EActionTypeButton.ClearAll:
                    Label.text = "CLEAR ALL";
                    break;
            }
        }
    }

    public enum EActionTypeButton
    {
        Add,
        Remove,
        RemoveSelect,
        FilterAll,
        FilterEquipment,
        FilterConsumable,
        FilterQuest,
        ChangeCapacity ,
        ClearAll
    }
}