using UnityEngine;
using UnityEngine.UI;

public class SavedWeaponButtonUI : MonoBehaviour
{
    [SerializeField] private Image savedWeaponImage;
    [SerializeField] private Button button;
    
    private WeaponSaveSystem.WeaponSaveData _weaponSaveData;

    public void Initialize(WeaponSaveSystem.WeaponSaveData weaponSaveData, Sprite sprite, UnityEngine.Events.UnityAction<WeaponSaveSystem.WeaponSaveData> onClick)
    {
        _weaponSaveData = weaponSaveData;
        savedWeaponImage.sprite = sprite;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(_weaponSaveData));
    }
}
