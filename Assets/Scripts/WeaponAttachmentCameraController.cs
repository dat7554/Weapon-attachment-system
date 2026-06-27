using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class WeaponAttachmentCameraController : MonoBehaviour
{
    [Serializable]
    private class AttachmentCamera
    {
        public Weapon.PartType partType;
        public CinemachineCamera camera;
    }
    
    [SerializeField] private CinemachineCamera defaultCamera;
    [SerializeField] private CinemachineCamera weaponModifyCamera;
    [SerializeField] private List<AttachmentCamera> attachmentCameraList;
    
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;
    
    private void Awake()
    {
        ShowDefaultCamera();
    }
    
    public void ShowDefaultCamera()
    {
        SetAllCamerasInactive();
        
        defaultCamera.Priority = activePriority;
    }

    public void ShowWeaponModifyCamera()
    {
        SetAllCamerasInactive();

        weaponModifyCamera.Priority = activePriority;
    }
    
    public void ShowAttachmentCamera(Weapon.PartType partType)
    {
        SetAllCamerasInactive();

        foreach (var attachmentCamera in attachmentCameraList)
        {
            if (attachmentCamera.partType != partType)
                continue;

            attachmentCamera.camera.Priority = activePriority;
            return;
        }

        Debug.LogWarning($"No Cinemachine camera found for attachment slot: {partType}");
        ShowDefaultCamera();
    }
    
    private void SetAllCamerasInactive()
    {
        defaultCamera.Priority = inactivePriority;
        weaponModifyCamera.Priority = inactivePriority;

        foreach (var attachmentCamera in attachmentCameraList)
        {
            attachmentCamera.camera.Priority = inactivePriority;
        }
    }
}