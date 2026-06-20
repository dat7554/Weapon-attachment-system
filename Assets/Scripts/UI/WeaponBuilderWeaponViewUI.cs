using UnityEngine;

public class WeaponBuilderWeaponViewUI : MonoBehaviour
{
    [SerializeField] private WeaponAttachmentCameraController attachmentCameraController;
    [SerializeField] private Vector3 selectionWeaponEulerAngles = new (0f, 40f, 0f);
    
    public void ApplyMode(WeaponBuilderUI.Mode mode, Weapon weapon, Weapon.PartType selectedPartType)
    {
        switch (mode)
        {
            case WeaponBuilderUI.Mode.WeaponSelection:
            case WeaponBuilderUI.Mode.Loadout:
                PreviewWeapon(weapon);
                break;
            
            case WeaponBuilderUI.Mode.Modify:
                ApplyModifyModeToWeapon(weapon);
                break;
            
            case WeaponBuilderUI.Mode.AttachmentSelection:
                ApplyAttachmentSelectionModeToWeapon(weapon, selectedPartType);
                break;
        }
    }
    
    private void PreviewWeapon(Weapon weapon)
    {
        weapon.HideAttachmentSlotsUI();
        weapon.DisableMouseRotate();
        
        attachmentCameraController.ShowDefaultCamera();
        WeaponAttachmentSystem.Instance.RotateCurrentWeaponTo(selectionWeaponEulerAngles);
    }

    private void ApplyModifyModeToWeapon(Weapon weapon)
    {
        weapon.ShowAttachmentSlotsUI();
        //weapon.EnableMouseRotate();
        
        attachmentCameraController.ShowDefaultCamera();
        WeaponAttachmentSystem.Instance.RotateCurrentWeaponTo(Vector3.zero);
    }

    private void ApplyAttachmentSelectionModeToWeapon(Weapon weapon, Weapon.PartType selectedPartType)
    {
        weapon.HideAttachmentSlotsUI();
        
        attachmentCameraController.ShowAttachmentCamera(selectedPartType);
        WeaponAttachmentSystem.Instance.RotateCurrentWeaponTo(Vector3.zero);
    }
}
