using UnityEngine;

public class WeaponBuilderUI : MonoBehaviour
{
    public enum Mode
    {
        WeaponSelection,
        Modify,
        Loadout,
        AttachmentSelection
    }
    
    [SerializeField] private WeaponBuilderPanelsUI panelsUI;
    [SerializeField] private WeaponBuilderWeaponViewUI weaponViewUI;

    [SerializeField] private WeaponSelectorUI weaponSelectorUI;
    [SerializeField] private SavedWeaponButtonListUI savedWeaponButtonListUI;
    [SerializeField] private WeaponAttachmentSelectorUI weaponAttachmentSelectorUI;
    
    private Mode _currentMode;
    private Weapon.PartType _selectedPartType;

    private void Start()
    {
        panelsUI.OnEnterModifyClicked += HandleEnterModifyClicked;
        panelsUI.OnModifyTabClicked += HandleModifyTabClicked;
        panelsUI.OnLoadoutTabClicked += HandleLoadoutTabClicked;
        panelsUI.OnBackClicked += HandleBackButtonClicked;
        
        weaponSelectorUI.OnWeaponSelected += WeaponSelectorUI_OnWeaponSelected;
        WeaponAttachmentSystem.Instance.OnWeaponChanged += WeaponAttachmentSystem_OnWeaponChanged;
        WeaponAttachmentSlotsUI.OnAttachmentSlotSelected += WeaponAttachmentSlotsUI_OnAttachmentSlotSelected;
        
        SetMode(Mode.WeaponSelection);
    }

    private void OnDestroy()
    {
        panelsUI.OnEnterModifyClicked -= HandleEnterModifyClicked;
        panelsUI.OnModifyTabClicked -= HandleModifyTabClicked;
        panelsUI.OnLoadoutTabClicked -= HandleLoadoutTabClicked;
        panelsUI.OnBackClicked -= HandleBackButtonClicked;
        
        weaponSelectorUI.OnWeaponSelected -= WeaponSelectorUI_OnWeaponSelected;
        WeaponAttachmentSystem.Instance.OnWeaponChanged -= WeaponAttachmentSystem_OnWeaponChanged;
        WeaponAttachmentSlotsUI.OnAttachmentSlotSelected -= WeaponAttachmentSlotsUI_OnAttachmentSlotSelected;
    }
    
    private void HandleEnterModifyClicked() => SetMode(Mode.Modify);

    private void HandleModifyTabClicked() => SetMode(Mode.Modify);

    private void HandleLoadoutTabClicked() => SetMode(Mode.Loadout);
    
    private void HandleBackButtonClicked()
    {
        switch (_currentMode)
        {
            case Mode.AttachmentSelection:
                SetMode(Mode.Modify);
                return;
            
            case Mode.Modify:
            case Mode.Loadout:
                SetMode(Mode.WeaponSelection);
                break;
        }
    }

    private void WeaponSelectorUI_OnWeaponSelected(Weapon weapon) => WeaponAttachmentSystem.Instance.SelectWeapon(weapon);
    
    private void WeaponAttachmentSystem_OnWeaponChanged() => ApplyCurrentModeToWeapon();
    
    private void WeaponAttachmentSlotsUI_OnAttachmentSlotSelected(Weapon.PartType partType)
    {
        _selectedPartType = partType;
        weaponAttachmentSelectorUI.RefreshButtons(partType);
        SetMode(Mode.AttachmentSelection);
    }
    
    private void SetMode(Mode mode)
    {
        _currentMode = mode;
        
        RefreshModeData();
        panelsUI.ApplyMode(_currentMode);
        ApplyCurrentModeToWeapon();
    }
    
    private void RefreshModeData()
    {
        if (_currentMode != Mode.Loadout)
            return;

        string weaponDisplayName = WeaponAttachmentSystem.Instance.GetCurrentWeapon.GetDisplayName();
        savedWeaponButtonListUI.RefreshButtons(weaponDisplayName);
    }
    
    private void ApplyCurrentModeToWeapon()
    {
        Weapon currentWeapon = WeaponAttachmentSystem.Instance.GetCurrentWeapon;
        weaponViewUI.ApplyMode(_currentMode, currentWeapon, _selectedPartType);
    }
}
