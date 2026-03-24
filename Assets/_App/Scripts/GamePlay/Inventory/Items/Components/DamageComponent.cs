using System;

namespace Ninsar.Inventory
{
    [Serializable]
    public class DamageComponent : IItemComponent
    {
        public float Min; 
        public float Max;
        public string Description => $"Damage Min:({Min}) -- Max:({Max})";
    }
}