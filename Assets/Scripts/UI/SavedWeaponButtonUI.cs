using UnityEngine;
using UnityEngine.UI;

public class SavedWeaponButtonUI : MonoBehaviour
{
    [SerializeField] private Image savedWeaponImage;
    [SerializeField] private Button button;
    
    private WeaponSaveSystem.WeaponSaveData _weaponSaveData;

    public void Initialize(WeaponSaveSystem.WeaponSaveData weaponSaveData, Sprite sprite)
    {
        _weaponSaveData = weaponSaveData;
        savedWeaponImage.sprite = sprite;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(LoadSavedWeapon);
    }

    private void LoadSavedWeapon()
    {
        WeaponSaveSystem.Instance.LoadWeapon(_weaponSaveData.saveId);
    }
}
