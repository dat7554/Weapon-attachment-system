using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    public event Action<Weapon> OnWeaponSelected;
    
    [SerializeField] private Transform contentTransform;
    [SerializeField] private WeaponSelectionButtonUI buttonTemplate;
    
    private void Awake()
    {
        buttonTemplate.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        ClearButtons();
        
        List<Weapon> weaponList = WeaponAttachmentSystem.Instance.GetWeaponList;
        
        for (int i = 0; i < weaponList.Count; i++)
        {
            Weapon weapon = weaponList[i];
            
            WeaponSelectionButtonUI button = Instantiate(buttonTemplate, contentTransform);
            
            bool isSelected = WeaponAttachmentSystem.Instance.GetCurrentWeapon.GetDisplayName() == weapon.GetDisplayName();
            
            button.Initialize
            (
                weapon,
                isSelected,
                () =>
                {
                    OnWeaponSelected?.Invoke(weapon);
                    RefreshButtons();
                }
            );
            
            button.gameObject.SetActive(true);
        }
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
}
