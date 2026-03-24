using System;

namespace Ninsar.Inventory
{
    [Serializable]
    public class HealComponent : IItemComponent 
    { 
        public float Heal; 
        public float Time;
        public string Description =>$"Heal:({Heal}) -- Time:({Time})";
    }
}