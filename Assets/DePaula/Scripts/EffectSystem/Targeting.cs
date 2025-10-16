using UnityEngine;

[System.Flags]
public enum Targeting
{
    Special = 0,
    Self = 1 << 0,
    OwnHero = 1 << 1,
    EnemyHero = 1 << 2,
    AdjacentAllies = 1 << 3,
    EnemyInFront = 1 << 4,
    AllEnemyCards = 1 << 5
}
