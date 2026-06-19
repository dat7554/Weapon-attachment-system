using UnityEngine;
using UnityEngine.UI;

public class WeaponBuilderUI : MonoBehaviour
{
    private enum Mode
    {
        Browse,
        Modify
    }
    
    [SerializeField] private Button modifyButton;
    [SerializeField] private Button backToBrowseButton;
    [SerializeField] private Button saveButton;

    [SerializeField] private WeaponSelectorUI weaponSelectorUI;
    [SerializeField] private SavedWeaponButtonListUI savedWeaponButtonListUI;
    
    [SerializeField] private Vector3 browseWeaponEulerAngles = new (0f, 40f, 0f);
    
    private Mode _currentMode;
    
    private void Awake()
    {
        modifyButton.onClick.AddListener(() => SetMode(Mode.Modify));
        backToBrowseButton.onClick.AddListener(() => SetMode(Mode.Browse));
    }

    private void Start()
    {
        WeaponAttachmentSystem.Instance.OnWeaponModified += WeaponAttachmentSystem_OnWeaponModified;
        
        SetMode(Mode.Browse);
    }
    
    private void OnDestroy()
    {
        
        WeaponAttachmentSystem.Instance.OnWeaponModified -= WeaponAttachmentSystem_OnWeaponModified;
    }
    
    private void WeaponAttachmentSystem_OnWeaponModified()
    {
        ApplyCurrentModeToWeapon();
    }
    
    private void SetMode(Mode mode)
    {
        _currentMode = mode;

        ApplyModeToPanels();
        ApplyCurrentModeToWeapon();
    }

    private void ApplyModeToPanels()
    {
        bool isBrowseMode = _currentMode == Mode.Browse;
        
        modifyButton.gameObject.SetActive(isBrowseMode);
        weaponSelectorUI.gameObject.SetActive(isBrowseMode);
        savedWeaponButtonListUI.gameObject.SetActive(isBrowseMode);

        backToBrowseButton.gameObject.SetActive(!isBrowseMode);
        saveButton.gameObject.SetActive(!isBrowseMode);
    }

    private void ApplyCurrentModeToWeapon()
    {
        Weapon currentWeapon = WeaponAttachmentSystem.Instance.GetCurrentWeapon;

        switch (_currentMode)
        {
            case Mode.Browse:
                ApplyBrowseModeToWeapon(currentWeapon);
                break;
            case Mode.Modify:
                ApplyModifyModeToWeapon(currentWeapon);
                break;
        }
    }
    
    private void ApplyBrowseModeToWeapon(Weapon weapon)
    {
        weapon.HideAttachmentSlotsUI();
        weapon.DisableMouseRotate();

        WeaponAttachmentSystem.Instance.RotateCurrentWeaponTo(browseWeaponEulerAngles);
    }

    private void ApplyModifyModeToWeapon(Weapon weapon)
    {
        weapon.ShowAttachmentSlotsUI();
        weapon.EnableMouseRotate();

        WeaponAttachmentSystem.Instance.RotateCurrentWeaponTo(Vector3.zero);
    }
}
