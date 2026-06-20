using System;
using System.Collections.Generic;
using UnityEngine;

public class SavedWeaponButtonListUI : MonoBehaviour
{
    public static event Action<WeaponSaveSystem.WeaponSaveData> OnSavedWeaponSelected;
    
    [SerializeField] private SavedWeaponButtonUI buttonTemplate;

    private void Start()
    {
        buttonTemplate.gameObject.SetActive(false);

        WeaponSaveSystem.Instance.OnWeaponSaved += RefreshButtons;
    }

    private void OnDestroy()
    {
        WeaponSaveSystem.Instance.OnWeaponSaved -= RefreshButtons;
    }

    public void RefreshButtons(string weaponDisplayName)
    {
        ClearButtons();
        
        List<WeaponSaveSystem.WeaponSaveData> weaponSaveDataList = WeaponSaveSystem.Instance.GetSaveDataListForWeapon(weaponDisplayName);
        foreach (var weaponSaveData in weaponSaveDataList)
        {
            if (!WeaponSaveSystem.Instance.TryLoadScreenshotSprite(weaponSaveData, out Sprite sprite))
                continue;
            
            SavedWeaponButtonUI savedWeaponButtonUI = Instantiate(buttonTemplate, transform);
            
            savedWeaponButtonUI.Initialize
                (
                    weaponSaveData, 
                    sprite,
                    selectedSaveData => OnSavedWeaponSelected?.Invoke(selectedSaveData)
                );
            
            savedWeaponButtonUI.gameObject.SetActive(true);
        }
    }

    private void ClearButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childGameObject = transform.GetChild(i).gameObject;
            
            if (childGameObject == buttonTemplate.gameObject)
                continue;
            
            Destroy(childGameObject);
        }
    }
}
