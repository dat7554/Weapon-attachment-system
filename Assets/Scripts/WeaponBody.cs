using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBody : MonoBehaviour
{
    public enum PartType
    {
        Scope,
        Handle
    }

    [Serializable]
    public class PartSlot
    {
        public PartType partType;
        public Transform attachPoint;
        public List<GameObject> prefabs;
        
        [HideInInspector] public int currentIndex;
        [HideInInspector] public GameObject spawnedPart;
    }
    
    [SerializeField] private List<PartSlot> partSlotList;

    private readonly Dictionary<PartType, PartSlot> _attachedPartsDict = new();

    private void Awake()
    {
        foreach (var partSlot in partSlotList)
        {
            _attachedPartsDict[partSlot.partType] = partSlot;
            partSlot.currentIndex = -1;
        }
    }

    public void SetPart(PartType partType)
    {
        if (!_attachedPartsDict.TryGetValue(partType, out var partSlot))
        {
            Debug.LogWarning($"No part slot with type {partType} found");
            return;
        }
        
        if (partSlot.spawnedPart != null)
        {
            Destroy(partSlot.spawnedPart);
            partSlot.spawnedPart = null;
        }

        partSlot.currentIndex = WeaponAttachmentSystem.Instance.GetNextIndex
            (
                partSlot.currentIndex, _attachedPartsDict[partType].prefabs.Count
            );
        
        if (partSlot.currentIndex == -1) return;
        
        partSlot.spawnedPart = Instantiate(partSlot.prefabs[partSlot.currentIndex], partSlot.attachPoint);
        partSlot.spawnedPart.transform.localPosition = Vector3.zero;
        partSlot.spawnedPart.transform.localRotation = Quaternion.identity;
    }
}