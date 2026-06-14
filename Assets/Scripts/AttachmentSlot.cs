using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    [SerializeField] private Weapon.AttachmentType attachmentType;
    
    public Weapon.AttachmentType GetAttachmentType() => attachmentType;
}
