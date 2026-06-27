using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedWeaponSelectorUI : MonoBehaviour
{
    public static event Action<WeaponSaveSystem.WeaponSaveData> OnSavedWeaponSelected;
    
    [SerializeField] private Transform contentTransform;
    [SerializeField] private SavedWeaponButtonUI buttonTemplate;
    
    private ScrollRect _scrollRect;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        
        buttonTemplate.gameObject.SetActive(false);
    }
    
    private void Start()
    {
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
            
            SavedWeaponButtonUI savedWeaponButtonUI = Instantiate(buttonTemplate, contentTransform);
            
            savedWeaponButtonUI.Initialize
                (
                    weaponSaveData, 
                    sprite,
                    selectedSaveData => OnSavedWeaponSelected?.Invoke(selectedSaveData)
                );
            
            savedWeaponButtonUI.gameObject.SetActive(true);
        }
        
        ResetScrollPosition();
    }

    private void ClearButtons()
    {
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            GameObject childGameObject = contentTransform.GetChild(i).gameObject;
            
            if (childGameObject == buttonTemplate.gameObject)
                continue;
            
            Destroy(childGameObject);
        }
    }
    
    private void ResetScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1f;
    }
}
