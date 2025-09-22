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
            Trait.Melee => "Does melee damage to the player if it is next to it",
            Trait.Relic => "Strange indigestible fragments of old power. Each relic appears in the maw stationary until consumed. Their effects vary: some grant new abilities, some warp enemy behaviour, and some expand your stomach beyond its starting three slots.",
            Trait.Fireproof => "Immune to fire",
            Trait.Burner => "Deals fire damage to surrounding entities. Immune to fire",
            Trait.Bomb => "Explodes on death damaging all within 2 tiles, takes 1 damage every turn",
            Trait.Morsel => "Can be consumed in the belly, grants sustenance to level up the player",
            Trait.Projectile => "Damages what it touches, can be fired back at enemies",
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
            Trait.Relic => Color.cornflowerBlue,
            Trait.Morsel => Color.rosyBrown,
            Trait.Projectile => Color.purple,
            _ => Color.orangeRed
        };
    }

    public static bool HasFlagFast(this Trait value, Trait flag)
    {
        return (value & flag) != 0;
    }
}