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
        
        if (attachmentSlot.spawnedPart != null)
        {
            Destroy(attachmentSlot.spawnedPart);
            attachmentSlot.spawnedPart = null;
        }

        attachmentSlot.currentIndex = WeaponAttachmentSystem.Instance.GetNextIndex
            (
                attachmentSlot.currentIndex, _attachmentSlotDict[partType].acceptedWeaponPartSOList.Count
            );
        
        if (attachmentSlot.currentIndex == -1) return;

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
}