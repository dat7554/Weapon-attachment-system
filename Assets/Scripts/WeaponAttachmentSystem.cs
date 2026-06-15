using System;
using UnityEngine;

public class WeaponAttachmentSystem : MonoBehaviour
{
    public static WeaponAttachmentSystem Instance;
    
    [SerializeField] private WeaponBody weaponBody;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void SelectAttachment(WeaponBody.PartType partType)
    {
        switch (partType)
        {
            case WeaponBody.PartType.Scope:
                weaponBody.SetPart(WeaponBody.PartType.Scope);
                break;
            case WeaponBody.PartType.Handle:
                weaponBody.SetPart(WeaponBody.PartType.Handle);
                break;
        }
    }
    
    public int GetNextIndex(int currentIndex, int count)
    {
        if (count == 0) return -1;
        
        if (currentIndex < 0)
        {
            return 0;
        }
        
        if (currentIndex == count - 1)
        {
            return -1;
        }
        
        return currentIndex + 1;
    }
}
