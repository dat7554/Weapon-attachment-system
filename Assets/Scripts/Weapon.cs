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
    [SerializeField] private WeaponAttachmentSlotsUI weaponAttachmentSlotsUI;
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

    public bool TryGetAcceptedWeaponPartsFromPartType(PartType partType, out List<WeaponPartSO> acceptedWeaponPartSOList)
    {
        acceptedWeaponPartSOList = null;

        if (!TryGetAttachmentSlotFromPartType(partType, out var attachmentSlot))
            return false;


        acceptedWeaponPartSOList = attachmentSlot.acceptedWeaponPartSOList;
        return true;
    }
    
    public void EnableMouseRotate() => _mouseRotate.enabled = true;
    
    public void DisableMouseRotate() => _mouseRotate.enabled = false;

    public int GetSelectedPartIndex(PartType partType)
    {
        if (!TryGetAttachmentSlotFromPartType(partType, out var attachmentSlot))
            return -1;
        
        return attachmentSlot.currentIndex;
    }
    
    public void SetPartByIndex(PartType partType, int selectedIndex)
    {
        if (!TryGetAttachmentSlotFromPartType(partType, out WeaponAttachmentSlot attachmentSlot))
            return;
        
        ClearSpawnedPart(attachmentSlot);

        attachmentSlot.currentIndex = selectedIndex;
        
        if (attachmentSlot.currentIndex == -1) 
            return;
        
        if (attachmentSlot.currentIndex >= attachmentSlot.acceptedWeaponPartSOList.Count)
        {
            Debug.LogWarning($"Index {selectedIndex} is out of range");
            return;
        }
        
        WeaponPartSO weaponPartS0 = attachmentSlot.acceptedWeaponPartSOList[attachmentSlot.currentIndex];
        attachmentSlot.spawnedPart = Instantiate(weaponPartS0.prefab, attachmentSlot.attachPoint);

        ApplyPartOffset(attachmentSlot.spawnedPart, weaponPartS0);
    }
    
    public List<WeaponAttachmentSlotSaveData> GetAttachmentSaveDataList()
    {
        List<WeaponAttachmentSlotSaveData> attachmentSaveDataList = new();

        foreach (var attachmentSlot in attachmentSlotList)
        {
            string selectedPartName = string.Empty;
            
            if (IsValidPartIndex(attachmentSlot, attachmentSlot.currentIndex))
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
    
    public void ShowAttachmentSlotsUI() => weaponAttachmentSlotsUI.gameObject.SetActive(true);
    
    public void HideAttachmentSlotsUI() => weaponAttachmentSlotsUI.gameObject.SetActive(false);

    private bool TryGetAttachmentSlotFromPartType(PartType partType, out WeaponAttachmentSlot attachmentSlot)
    {
        if (_attachmentSlotDict.TryGetValue(partType, out attachmentSlot))
            return true;

        Debug.LogWarning($"No attachment slot with type {partType} found");
        return false;
    }
    
    private void ClearSpawnedPart(WeaponAttachmentSlot attachmentSlot)
    {
        if (attachmentSlot.spawnedPart == null)
            return;

        Destroy(attachmentSlot.spawnedPart);
        attachmentSlot.spawnedPart = null;
    }
    
    private bool IsValidPartIndex(WeaponAttachmentSlot attachmentSlot, int selectedIndex)
    {
        return selectedIndex >= 0 &&
               selectedIndex < attachmentSlot.acceptedWeaponPartSOList.Count;
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
        
        spawnedPart.transform.localRotation = Quaternion.identity;
    }
}