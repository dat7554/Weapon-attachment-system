using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WeaponAttachmentSystem : MonoBehaviour
{
    public static WeaponAttachmentSystem Instance;

    public event Action OnWeaponChanged;
    public event Action<Weapon.PartType> OnWeaponAttachmentModified;
    public event Action OnWeaponVisualChanged;
    
    [SerializeField] private List<Weapon> weaponList;
    [SerializeField] private Weapon currentWeapon;

    private Tween _rotationTween;
    
    public List<Weapon> GetWeaponList => weaponList;
    public Weapon GetCurrentWeapon => currentWeapon;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
    }

    private void Start()
    {
        NotifyWeaponChanged();
        NotifyWeaponVisualChanged();
    }

    public void SelectWeapon(Weapon weapon)
    {
        if (IsCurrentWeapon(weapon))
            return;
        
        SpawnWeapon(weapon);
        
        NotifyWeaponChanged();
        NotifyWeaponVisualChanged();
    }
    
    public void LoadWeaponSave(WeaponSaveSystem.WeaponSaveData weaponSaveData)
    {
        if (!TryGetWeaponByDisplayName(weaponSaveData.weaponDisplayName, out var weapon)) 
            return;
        
        SpawnWeapon(weapon);
        ApplyAttachmentSaveData(weaponSaveData.attachments);
        
        NotifyWeaponChanged();
        NotifyWeaponVisualChanged();
    }
    
    public void SetPartByIndex(Weapon.PartType partType, int selectedIndex)
    {
        currentWeapon.SetPartByIndex(partType, selectedIndex);
        
        NotifyWeaponAttachmentModified(partType);
        NotifyWeaponVisualChanged();
    }
    
    public void RotateCurrentWeaponTo(Vector3 eulerAngles)
    {
        _rotationTween?.Kill();

        _rotationTween = currentWeapon.transform
            .DOLocalRotate(eulerAngles, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => _rotationTween = null);
    }
    
    public int GetNextIndex(int currentIndex, int count)
    {
        if (count == 0) return -1;
        if (currentIndex < 0) return 0;
        if (currentIndex == count - 1) return -1;
        
        return currentIndex + 1;
    }
    
    private bool IsCurrentWeapon(Weapon weapon)
    {
        return currentWeapon && weapon.GetDisplayName() == currentWeapon.GetDisplayName();
    }

    private bool TryGetWeaponByDisplayName(string displayName, out Weapon foundWeapon)
    {
        foreach (var weapon in weaponList)
        {
            if (weapon.GetDisplayName() != displayName) continue;

            foundWeapon = weapon;
            return true;
        }
        
        Debug.LogWarning($"No weapon found with display name: {displayName}");
        foundWeapon = null;
        return false;
    }
    
    private void SpawnWeapon(Weapon weapon)
    {
        Transform parent = currentWeapon.transform.parent;
        Vector3 localPosition = currentWeapon.transform.localPosition;
        
        Weapon spawnedWeapon = Instantiate(weapon, parent);
        spawnedWeapon.transform.localPosition = localPosition;
        spawnedWeapon.transform.localRotation = Quaternion.identity;
        
        Destroy(currentWeapon.gameObject);
        
        currentWeapon = spawnedWeapon;
    }

    private void ApplyAttachmentSaveData(Weapon.WeaponAttachmentSlotSaveData[] weaponAttachmentSlotSaveDataArray)
    {
        foreach (var weaponAttachmentSlotSaveData in weaponAttachmentSlotSaveDataArray)
        {
            if (!Enum.TryParse(weaponAttachmentSlotSaveData.partType, out Weapon.PartType partType))
            {
                Debug.LogWarning($"Invalid part type: {weaponAttachmentSlotSaveData.partType}");
                continue;
            }
            
            currentWeapon.SetPartByIndex(partType, weaponAttachmentSlotSaveData.selectedIndex);
        }
    }
    
    private void NotifyWeaponChanged() => OnWeaponChanged?.Invoke();

    private void NotifyWeaponAttachmentModified(Weapon.PartType partType) => OnWeaponAttachmentModified?.Invoke(partType);
    
    private void NotifyWeaponVisualChanged() => OnWeaponVisualChanged?.Invoke();
}
