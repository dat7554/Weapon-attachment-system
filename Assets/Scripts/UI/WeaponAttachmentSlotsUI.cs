using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAttachmentSlotsUI : MonoBehaviour
{
    public static event Action<Weapon.PartType> OnAttachmentSlotSelected;
    
    [Serializable]
    private class WeaponPartButton
    {
        public Weapon.PartType partType;
        public Button button;
    }

    [SerializeField] private List<WeaponPartButton> weaponPartButtonList;

    private void Awake()
    {
        foreach (var weaponPartButton in weaponPartButtonList)
        {
            weaponPartButton.button.onClick.AddListener
                (
                    //() => WeaponAttachmentSystem.Instance.SetPart(weaponPartButton.partType)
                    () => OnAttachmentSlotSelected?.Invoke(weaponPartButton.partType)
                );
        }
    }
}
