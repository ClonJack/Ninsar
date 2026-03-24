using System.Collections.Generic;
using Ninsar.Locator;
using UnityEngine;
using UnrealTeam.GamePlay.UI;

namespace Ninsar.Inventory
{
    public class InventoryPresenter : MonoBehaviour
    {
        [field: SerializeField] public Transform Content { get; private set; }
        [field: SerializeField] public TextView DescriptionLabel { get; private set; }
        [field: SerializeField] public TextView PropertyLabel { get; private set; }
        [field: SerializeField] public TextView FilterLabel { get; private set; }

        [SerializeField] private List<DebugButtonView> _debugButtons = new();
        
        private readonly InventoryItemPool _pool = new();
        private readonly InventoryItemFilter _filter = new();

        private InventoryStorage _storage;
        private ItemView _selectedItem;

        public void Start()
        {
            _storage = ServicesLocator.Instance.GetServices<InventoryStorage>();

            _pool.Create(Content, _storage.MaxCapacity);

            foreach (var view in _pool.Pool)
                view.OnClick += HandleItemClick;

            foreach (var debugButton in _debugButtons)
                debugButton.OnClick += HandleClickDebugButton;

            _storage.OnItemAdded += OnItemAdded;
            _storage.OnItemRemoved += OnItemRemoved;
            _storage.OnCapacityChanged += OnCapacityChanged;

            UpdateFilterLabel();
            UpdateView();
        }
        public void OnDestroy()
        {
            foreach (var debugButton in _debugButtons)
                debugButton.OnClick -= HandleClickDebugButton;

            if (_storage != null)
            {
                _storage.OnItemAdded -= OnItemAdded;
                _storage.OnItemRemoved -= OnItemRemoved;
                _storage.OnCapacityChanged -= OnCapacityChanged;
            }

            foreach (var view in _pool.Pool)
                view.OnClick -= HandleItemClick;

            _pool.Clear();
        }
        private void OnItemAdded(ItemData item) 
            => UpdateView();
        private void OnItemRemoved(ItemData item) 
            => UpdateView();
        private void OnCapacityChanged(int used, int max) 
            => UpdateView();
        private void UpdateView()
        {
            var index = 0;

            foreach (var item in _storage.Items)
            {
                if (!_filter.Matches(item))
                    continue;

                var instance = _pool.Get(index);
                instance.Display(item);
                index++;
            }

            _pool.HideFrom(index);

            if (_selectedItem != null) 
                _selectedItem.Unselect();
            
            _selectedItem = null;
        }
        private void SetFilter(EItemCategory? filter)
        {
            _filter.SetFilter(filter);
            UpdateFilterLabel();
            UpdateView();
        }
        private void ClearFilter()
        {
            _filter.ClearFilter();
            UpdateFilterLabel();
            UpdateView();
        }
        private void UpdateFilterLabel()
        {
            if (FilterLabel == null)
                return;

            var filterText = _filter.CurrentFilter switch
            {
                EItemCategory.Equipment => "FILTER: EQUIPMENT",
                EItemCategory.Consumable => "FILTER: CONSUMABLE",
                EItemCategory.Quest => "FILTER: QUEST",
                _ => "FILTER: ALL"
            };

            FilterLabel.SetText(filterText);
        }
        private void HandleItemClick(ItemView itemView)
        {
            foreach (var view in _pool.Pool)
                view.Unselect();

            itemView.Select();

            if (itemView.ItemData?.Source != null)
            {
                DescriptionLabel.SetText($"Description: {itemView.ItemData.Source.Description}");

                var propertyText = string.Empty;
                foreach (var component in itemView.ItemData.Source.Property)
                    propertyText += $"{component.Description}\n";
                
                PropertyLabel.SetText(propertyText);
            }

            _selectedItem = itemView;
        }
        private void ClearSelection()
        {
            if (_selectedItem != null)
            {
                _selectedItem.Unselect();
                _selectedItem = null;
                DescriptionLabel.SetText(string.Empty);
                PropertyLabel.SetText(string.Empty);
            }
        }
        private void HandleClickDebugButton(DebugButtonView buttonView)
        {
            switch (buttonView.ETypeButton)
            {
                case EActionTypeButton.Add:
                    _storage.Add(buttonView.Item, buttonView.Count);
                    break;

                case EActionTypeButton.Remove:
                    _storage.Remove(buttonView.Item, Mathf.Abs(buttonView.Count));
                    break;

                case EActionTypeButton.RemoveSelect:
                    if (_selectedItem != null)
                        _storage.RemoveFromStack(_selectedItem.ItemData, Mathf.Abs(buttonView.Count));
                    break;

                case EActionTypeButton.FilterAll:
                    ClearFilter();
                    ClearSelection();
                    break;

                case EActionTypeButton.FilterEquipment:
                    SetFilter(EItemCategory.Equipment);
                    ClearSelection();
                    break;

                case EActionTypeButton.FilterConsumable:
                    SetFilter(EItemCategory.Consumable);
                    ClearSelection();
                    break;

                case EActionTypeButton.FilterQuest:
                    SetFilter(EItemCategory.Quest);
                    ClearSelection();
                    break;

                case EActionTypeButton.ClearAll:
                    _storage.Clear();
                    ClearSelection();
                    break;
            }

            UpdateView();
        }
    }
}