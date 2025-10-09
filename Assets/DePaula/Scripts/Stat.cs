using UnityEngine;

[System.Flags]
public enum Stat
{
     Nothing = 0,
     Attack = 1 << 0,
     Health = 1 << 1
}
