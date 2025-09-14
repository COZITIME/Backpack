using System;

[Flags]
public enum Trait
{
    None = 0,
    Player = 1 << 0,
    Melee = 1 << 1,
    Fireproof = 1 << 2,
    Burner = 1 << 3,
    Bomb = 1 << 4,
    Relic = 1 << 5,
    Morsel = 1 << 6,
}