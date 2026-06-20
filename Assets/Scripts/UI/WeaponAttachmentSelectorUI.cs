using UnityEngine;

public class WeaponAttachmentSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private WeaponAttachmentSelectionButtonUI buttonTemplate;

    private void Awake()
    {
        buttonTemplate.gameObject.SetActive(false);
    }

    public void RefreshButtons(Weapon.PartType partType)
    {
        ClearButtons();

        Weapon currentWeapon = WeaponAttachmentSystem.Instance.GetCurrentWeapon;

        if (!currentWeapon.TryGetAcceptedWeaponPartsFromPartType(partType, out var acceptedParts))
            return;
        
        int currentSelectedIndex = currentWeapon.GetSelectedPartIndex(partType);

        for (int i = 0; i < acceptedParts.Count; i++)
        {
            int selectedIndex = i;
            WeaponPartSO weaponPartSO = acceptedParts[i];
            
            bool isSelected = selectedIndex == currentSelectedIndex;
            
            WeaponAttachmentSelectionButtonUI button = Instantiate(buttonTemplate, contentTransform);
            
            button.Initialize
                (
                    weaponPartSO,
                    isSelected,
                    () =>
                    {
                        int indexToApply = isSelected ? -1 : selectedIndex;

                        WeaponAttachmentSystem.Instance.SetPartByIndex(partType, indexToApply);
                        
                        RefreshButtons(partType);
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
