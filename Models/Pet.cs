using System;
using System.Collections.Generic;

namespace TamagotchiAPI.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; } = DateTime.UtcNow;
        public int HungerLevel { get; set; } = 0;
        public int HappinessLevel { get; set; } = 0;
        public DateTime LastInteractedWithDate { get; set; } = DateTime.UtcNow;
        public bool IsDead { get; set; }

        public List<Playtime> Playtimes { get; set; }
        public List<Feeding> Feedings { get; set; }
        public List<Scolding> Scoldings { get; set; }

        public void IsDeadMethod() { if ((DateTime.UtcNow - LastInteractedWithDate).Days > 3) { IsDead = true; } }
    }
}