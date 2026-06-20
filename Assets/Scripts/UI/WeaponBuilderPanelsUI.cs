using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBuilderPanelsUI : MonoBehaviour
{
    public event Action OnEnterModifyClicked;
    public event Action OnModifyTabClicked;
    public event Action OnLoadoutTabClicked;
    public event Action OnBackClicked;
    
    [SerializeField] private Button enterModifyButton;
    [SerializeField] private Button modifyTabButton;
    [SerializeField] private Button loadoutTabButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;

    [SerializeField] private WeaponSelectorUI weaponSelectorUI;
    [SerializeField] private SavedWeaponButtonListUI savedWeaponButtonListUI;
    [SerializeField] private WeaponAttachmentSelectorUI weaponAttachmentSelectorUI;
    
    private void Awake()
    {
        enterModifyButton.onClick.AddListener(() => OnEnterModifyClicked?.Invoke());
        modifyTabButton.onClick.AddListener(() => OnModifyTabClicked?.Invoke());
        loadoutTabButton.onClick.AddListener(() => OnLoadoutTabClicked?.Invoke());
        backButton.onClick.AddListener(() => OnBackClicked?.Invoke());
    }
    
    public void ApplyMode(WeaponBuilderUI.Mode mode)
    {
        bool isWeaponSelection = mode == WeaponBuilderUI.Mode.WeaponSelection;
        bool isModify = mode == WeaponBuilderUI.Mode.Modify;
        bool isLoadout = mode == WeaponBuilderUI.Mode.Loadout;
        bool isAttachmentSelection = mode == WeaponBuilderUI.Mode.AttachmentSelection;

        bool showTabs = isModify || isLoadout;

        weaponSelectorUI.gameObject.SetActive(isWeaponSelection);
        enterModifyButton.gameObject.SetActive(isWeaponSelection);

        modifyTabButton.gameObject.SetActive(showTabs);
        loadoutTabButton.gameObject.SetActive(showTabs);

        saveButton.gameObject.SetActive(isModify);
        savedWeaponButtonListUI.gameObject.SetActive(isLoadout);
        weaponAttachmentSelectorUI.gameObject.SetActive(isAttachmentSelection);

        backButton.gameObject.SetActive(!isWeaponSelection);
    }
}
