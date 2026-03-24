namespace Ninsar.Inventory
{
    public class InventoryItemFilter
    {
        private EItemCategory? _filter;
        
        public bool HasFilter => _filter.HasValue;
        public EItemCategory? CurrentFilter => _filter;
        public void SetFilter(EItemCategory? filter)
        {
            _filter = filter;
        }
        public void ClearFilter()
        {
            _filter = null;
        }
        public bool Matches(ItemData item)
        {
            if (!_filter.HasValue)
                return true;

            return item.Source.EItemCategory.HasFlag(_filter.Value);
        }
        
    }
}