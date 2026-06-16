using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum PartType
    {
        Scope,
        Handle
    }

    [Serializable]
    public class WeaponAttachmentSlot
    {
        public PartType partType;
        public Transform attachPoint;
        public List<WeaponPartSO> acceptedWeaponPartSOList;
        
        [HideInInspector] public int currentIndex;
        [HideInInspector] public GameObject spawnedPart;
    }
    
    [SerializeField] private string displayName;
    [SerializeField] private List<WeaponAttachmentSlot> attachmentSlotList;

    private readonly Dictionary<PartType, WeaponAttachmentSlot> _attachmentSlotDict = new();

    private void Awake()
    {
        foreach (var attachmentSlot in attachmentSlotList)
        {
            _attachmentSlotDict[attachmentSlot.partType] = attachmentSlot;
            attachmentSlot.currentIndex = -1;
        }
    }
    
    public string GetDisplayName() => displayName;

    public void SetPart(PartType partType)
    {
        if (!_attachmentSlotDict.TryGetValue(partType, out var attachmentSlot))
        {
            Debug.LogWarning($"No attachment slot with type {partType} found");
            return;
        }
        
        if (attachmentSlot.spawnedPart != null)
        {
            Destroy(attachmentSlot.spawnedPart);
            attachmentSlot.spawnedPart = null;
        }

        attachmentSlot.currentIndex = WeaponAttachmentSystem.Instance.GetNextIndex
            (
                attachmentSlot.currentIndex, _attachmentSlotDict[partType].acceptedWeaponPartSOList.Count
            );
        
        if (attachmentSlot.currentIndex == -1)
        {
            Debug.LogWarning($"Current weapon has no accepted weapon part with type {partType}");
            return;
        }

        WeaponPartSO weaponPartS0 = attachmentSlot.acceptedWeaponPartSOList[attachmentSlot.currentIndex];
        attachmentSlot.spawnedPart = Instantiate(weaponPartS0.prefab, attachmentSlot.attachPoint);

        if (weaponPartS0.weaponOffsets.Count > 0)
        {
            foreach (var weaponOffsetEntry in weaponPartS0.weaponOffsets)
            {
                if (weaponOffsetEntry.weapon.GetDisplayName() == displayName)
                {
                    attachmentSlot.spawnedPart.transform.localPosition = weaponOffsetEntry.offset;
                }
            }
        }
        else
        {
            attachmentSlot.spawnedPart.transform.localPosition = Vector3.zero;
        }
        
        attachmentSlot.spawnedPart.transform.localRotation = Quaternion.identity;
    }
}