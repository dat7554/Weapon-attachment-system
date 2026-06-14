using UnityEngine;
using UnityEngine.UI;

public class WeaponAttachmentUI : MonoBehaviour
{
    [SerializeField] private Button scopeButton;
    [SerializeField] private Button handleButton;

    private Weapon _weapon;

    private void Awake()
    {
        _weapon = GetComponentInParent<Weapon>();
        
        scopeButton.onClick.AddListener(SelectScope);
        handleButton.onClick.AddListener(SelectHandle);
    }
    
    private void SelectScope()
    {
        _weapon.SelectAttachment(Weapon.AttachmentType.Scope);
    }

    private void SelectHandle()
    {
        _weapon.SelectAttachment(Weapon.AttachmentType.Handle);
    }
}
