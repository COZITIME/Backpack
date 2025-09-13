using System;
using UnityEngine;

public static class TraitExtensions
{
    public static string GetTraitDescription(this Trait trait)
    {
        return trait switch
        {
            Trait.None => "No Traits",
            Trait.Player => "This is you",
            Trait.Melee => "Does melee damage to play if it is next to it",
            Trait.Relic => "A relic that gives the player effects when in their belly, it cannot be damaged",
            Trait.Fireproof => "Immune to fire",
            Trait.Burner => "Deals fire damage to surrounding entities. Immune to fire",
            Trait.Bomb => "Explodes on death damaging all within 2 tiles, takes 1 damage every turn",

            _ => $"No Description for Trait: {trait}",
        };
    }

    public static Color GetTraitColor(this Trait trait)
    {
        return trait switch
        {
            Trait.Player => Color.forestGreen,
            Trait.Melee => Color.darkRed,
            Trait.Fireproof => Color.darkRed,
            Trait.Burner => Color.darkRed,
            Trait.Bomb => Color.darkRed,
            Trait.Relic => Color.yellowNice,
            _ => Color.orangeRed
        };
    }

    public static bool HasFlagFast(this Trait value, Trait flag)
    {
        return (value & flag) != 0;
    }
}