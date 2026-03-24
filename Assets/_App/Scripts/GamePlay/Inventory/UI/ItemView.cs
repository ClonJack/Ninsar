using System;
using UnityEngine;
using UnityEngine.UI;
using UnrealTeam.GamePlay.UI;

namespace Ninsar.Inventory
{
    public class ItemView : MonoBehaviour
    {
        [field: Header("Refs")]
        [field: SerializeField] public TextView LabelName { get; private set; }
        [field: SerializeField] public TextView LabelCount { get; private set; }
        [field: SerializeField] public Image ImageIcon { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }

        [field: Header("Properties")]
        [field: SerializeField] public ItemData ItemData { get; private set; }

        [field: Header("Selection")]
        [field: SerializeField] private float _selectedScale = 1.2f;
        [field: SerializeField] private float _deselectedScale = 1f;

        public event Action<ItemView> OnClick;
        public bool IsSelected { get; private set; }

        public void Awake()
        {
            Button.onClick.AddListener(() => OnClick?.Invoke(this));
        }

        public void OnDestroy()
        {
            Button.onClick.RemoveAllListeners();
        }

        public void Display(ItemData item)
        {
            if (item == null || item.Source == null)
            {
                gameObject.SetActive(false);
                return;
            }

            ItemData = item;
            gameObject.SetActive(true);
            
            ImageIcon.sprite = item.Source.Icon;
            LabelCount.SetText(item.Count.ToString());
            LabelName.SetText(item.Source.Name);
        }

        public void Select()
        {
            IsSelected = true;
            transform.localScale = Vector3.one * _selectedScale;
        }

        public void Unselect()
        {
            IsSelected = false;
            transform.localScale = Vector3.one * _deselectedScale;
        }
    }
}