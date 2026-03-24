using TMPro;
using UnityEngine;

namespace Ninsar.Inventory
{
    public class TextView : MonoBehaviour
    {
        [field: SerializeField]
        public TextMeshProUGUI Text { get; private set; }

        public void Awake()
            => Text = GetComponent<TextMeshProUGUI>();
        
        public void SetText(string text) 
            => Text.text = text;
    }
}