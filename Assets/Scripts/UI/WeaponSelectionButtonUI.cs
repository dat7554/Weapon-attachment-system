using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    
    public Button GetButton() => button;
    
    public void SetButtonText(string text) => buttonText.text = text;
}
