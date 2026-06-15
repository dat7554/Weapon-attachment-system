using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPartUI : MonoBehaviour
{
    [Serializable]
    private class WeaponPartButton
    {
        public WeaponBody.PartType partType;
        public Button button;
    }

    [SerializeField] private List<WeaponPartButton> weaponPartButtonList;

    private void Awake()
    {
        foreach (var weaponPartButton in weaponPartButtonList)
        {
            weaponPartButton.button.onClick.AddListener
                (
                    () => WeaponAttachmentSystem.Instance.SelectAttachment(weaponPartButton.partType)
                );
        }
    }
}
