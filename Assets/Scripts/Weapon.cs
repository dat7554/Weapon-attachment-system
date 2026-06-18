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
    
    [Serializable]
    public class WeaponAttachmentSlotSaveData
    {
        public string partType;
        public int selectedIndex;
        public string selectedPartName;
    }
    
    [SerializeField] private string displayName;
    [SerializeField] private List<WeaponAttachmentSlot> attachmentSlotList;

    private MouseRotate _mouseRotate;
    private readonly Dictionary<PartType, WeaponAttachmentSlot> _attachmentSlotDict = new();

    private void Awake()
    {
        _mouseRotate = GetComponent<MouseRotate>();
        
        foreach (var attachmentSlot in attachmentSlotList)
        {
            _attachmentSlotDict[attachmentSlot.partType] = attachmentSlot;
            attachmentSlot.currentIndex = -1;
        }
    }
    
    public string GetDisplayName() => displayName;
    
    public void DisableMouseRotate() => _mouseRotate.enabled = false;

    public void SetPart(PartType partType)
    {
        if (!_attachmentSlotDict.TryGetValue(partType, out var attachmentSlot))
        {
            Debug.LogWarning($"No attachment slot with type {partType} found");
            return;
        }

        int nextIndex = WeaponAttachmentSystem.Instance.GetNextIndex
            (
                attachmentSlot.currentIndex, 
                _attachmentSlotDict[partType].acceptedWeaponPartSOList.Count
            );

        SetPartByIndex(partType, nextIndex);
    }

    public void SetPartByIndex(PartType partType, int selectedIndex)
    {
        // Find attachment slot by partType
        if (!_attachmentSlotDict.TryGetValue(partType, out var attachmentSlot))
        {
            Debug.LogWarning($"No attachment slot with type {partType} found");
            return;
        }
        
        // Destroy old spawned part if it exists
        if (attachmentSlot.spawnedPart != null)
        {
            Destroy(attachmentSlot.spawnedPart);
            attachmentSlot.spawnedPart = null;
        }

        attachmentSlot.currentIndex = selectedIndex;
        
        if (attachmentSlot.currentIndex == -1) 
            return;
        
        if (attachmentSlot.currentIndex >= attachmentSlot.acceptedWeaponPartSOList.Count)
        {
            Debug.LogWarning($"Index {selectedIndex} is out of range");
            return;
        }

        // Spawn acceptedWeaponPartSOList[selectedIndex]
        WeaponPartSO weaponPartS0 = attachmentSlot.acceptedWeaponPartSOList[attachmentSlot.currentIndex];
        attachmentSlot.spawnedPart = Instantiate(weaponPartS0.prefab, attachmentSlot.attachPoint);

        // Apply offsets
        ApplyPartOffset(attachmentSlot.spawnedPart, weaponPartS0);
        
        attachmentSlot.spawnedPart.transform.localRotation = Quaternion.identity;
    }
    
    public List<WeaponAttachmentSlotSaveData> GetAttachmentSaveDataList()
    {
        List<WeaponAttachmentSlotSaveData> attachmentSaveDataList = new();

        foreach (var attachmentSlot in attachmentSlotList)
        {
            string selectedPartName = string.Empty;
            if (attachmentSlot.currentIndex >= 0 &&
                attachmentSlot.currentIndex < attachmentSlot.acceptedWeaponPartSOList.Count)
            {
                WeaponPartSO weaponPartSO = attachmentSlot.acceptedWeaponPartSOList[attachmentSlot.currentIndex];
                selectedPartName = weaponPartSO.displayName;
            }
            
            attachmentSaveDataList.Add(new WeaponAttachmentSlotSaveData
            {
                partType = attachmentSlot.partType.ToString(),
                selectedIndex = attachmentSlot.currentIndex,
                selectedPartName = selectedPartName
            });
        }

        return attachmentSaveDataList;
    }

    private void ApplyPartOffset(GameObject spawnedPart, WeaponPartSO weaponPartSO)
    {
        if (weaponPartSO.weaponOffsets.Count > 0)
        {
            foreach (var weaponOffsetEntry in weaponPartSO.weaponOffsets)
            {
                if (weaponOffsetEntry.weapon.GetDisplayName() == displayName)
                {
                    spawnedPart.transform.localPosition = weaponOffsetEntry.offset;
                }
            }
        }
        else
        {
            spawnedPart.transform.localPosition = Vector3.zero;
        }
    }
}