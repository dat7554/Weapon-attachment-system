using System.Collections.Generic;
using UnityEngine;

public class SavedWeaponButtonListUI : MonoBehaviour
{
    [SerializeField] private SavedWeaponButtonUI buttonTemplate;

    private void Start()
    {
        buttonTemplate.gameObject.SetActive(false);

        WeaponSaveSystem.Instance.OnWeaponSaved += RefreshButtons;
        
        RefreshButtons();
    }

    private void OnDestroy()
    {
        WeaponSaveSystem.Instance.OnWeaponSaved -= RefreshButtons;
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject childGameObject = transform.GetChild(i).gameObject;
            
            if (childGameObject == buttonTemplate.gameObject)
                continue;
            
            Destroy(childGameObject);
        }
        
        List<WeaponSaveSystem.WeaponSaveData> weaponSaveDataList = WeaponSaveSystem.Instance.GetCurrentWeaponSaveDataList;
        foreach (var weaponSaveData in weaponSaveDataList)
        {
            if (!WeaponSaveSystem.Instance.TryLoadScreenshotSprite(weaponSaveData, out Sprite sprite))
                continue;
            
            SavedWeaponButtonUI savedWeaponButtonUI = Instantiate(buttonTemplate, transform);
            savedWeaponButtonUI.Initialize(weaponSaveData, sprite);
            savedWeaponButtonUI.gameObject.SetActive(true);
        }
    }
}
