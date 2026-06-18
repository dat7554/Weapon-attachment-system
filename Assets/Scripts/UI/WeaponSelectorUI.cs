using UnityEngine;

public class WeaponSelectorUI : MonoBehaviour
{
    [SerializeField] private WeaponSelectionButtonUI selectionButtonTemplate;
    
    private void Start()
    {
        selectionButtonTemplate.gameObject.SetActive(false);
        
        foreach (var weapon in WeaponAttachmentSystem.Instance.GetWeaponList)
        {
            WeaponSelectionButtonUI weaponSelectionSelectionButtonUI = Instantiate(selectionButtonTemplate, transform);
            
            weaponSelectionSelectionButtonUI.gameObject.SetActive(true);
            weaponSelectionSelectionButtonUI.SetButtonText(weapon.GetDisplayName());
            weaponSelectionSelectionButtonUI.GetButton().onClick.AddListener
            (
                () => WeaponAttachmentSystem.Instance.SelectWeapon(weapon)
            );
        }
    }
}
