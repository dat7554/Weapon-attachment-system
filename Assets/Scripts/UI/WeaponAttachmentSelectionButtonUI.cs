using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAttachmentSelectionButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Outline outline;
    
    public void Initialize(WeaponPartSO weaponPartSO, bool isSelected, UnityEngine.Events.UnityAction onClick)
    {
        buttonText.text = weaponPartSO.displayName;
        outline.enabled = isSelected;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }
}
