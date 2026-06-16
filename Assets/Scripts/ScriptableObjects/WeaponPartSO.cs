using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponPartSO : ScriptableObject
{
    [Serializable]
    public struct WeaponOffsetEntry
    {
        public Weapon weapon;
        public Vector3 offset;
    }
    
    public Weapon.PartType partType;
    public string displayName;
    public GameObject prefab;
    public List<WeaponOffsetEntry> weaponOffsets;
}
