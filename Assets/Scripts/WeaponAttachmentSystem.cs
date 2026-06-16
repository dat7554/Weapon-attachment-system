using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachmentSystem : MonoBehaviour
{
    public static WeaponAttachmentSystem Instance;
    
    [SerializeField] private List<Weapon> weaponBodyList;
    [SerializeField] private Weapon currentWeapon;

    private Coroutine _rotateWeaponToOriginalCoroutine;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
    }

    public List<Weapon> GetWeaponBodyList => weaponBodyList;
    public Weapon GetCurrentWeapon => currentWeapon;

    public void SelectWeapon(Weapon weapon)
    {
        if (weapon.GetDisplayName() == currentWeapon.GetDisplayName())
        {
            if (_rotateWeaponToOriginalCoroutine == null)
            {
                _rotateWeaponToOriginalCoroutine = StartCoroutine(RotateWeaponToOriginalRoutine());
            }
            return;
        }
        
        Weapon spawnedWeapon = Instantiate(weapon, currentWeapon.transform.parent);
        spawnedWeapon.transform.localPosition = currentWeapon.transform.localPosition;
        spawnedWeapon.transform.localRotation = Quaternion.identity;
        
        Destroy(currentWeapon.gameObject);
        currentWeapon = spawnedWeapon;
    }
    
    public void SetPart(Weapon.PartType partType)
    {
        switch (partType)
        {
            case Weapon.PartType.Scope:
                currentWeapon.SetPart(Weapon.PartType.Scope);
                break;
            case Weapon.PartType.Handle:
                currentWeapon.SetPart(Weapon.PartType.Handle);
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

    private IEnumerator RotateWeaponToOriginalRoutine()
    {
        float rotateSpeed = 5f;
        while (Quaternion.Angle(currentWeapon.transform.localRotation, Quaternion.identity) > 1f)
        {
            if (Input.GetMouseButton(0))
            {
                _rotateWeaponToOriginalCoroutine = null;
                yield break;
            }
            
            currentWeapon.transform.localRotation = Quaternion.Slerp
                (
                    currentWeapon.transform.localRotation, 
                    Quaternion.identity, 
                    Time.deltaTime * rotateSpeed
                );
            
            yield return null;
        }

        currentWeapon.transform.localRotation = Quaternion.identity;

        _rotateWeaponToOriginalCoroutine = null;
    }
}
