using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachmentSystem : MonoBehaviour
{
    public static WeaponAttachmentSystem Instance;

    public event Action OnWeaponModified;
    
    [SerializeField] private List<Weapon> weaponBodyList;
    [SerializeField] private Weapon currentWeapon;

    private Coroutine _resetWeaponRotationCoroutine;
    private bool _hasInitializedCloneWeapon;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
    }

    // TODO: Fix that this not working to instantiate clone weapon
    private void Update()
    {
        if (!_hasInitializedCloneWeapon)
        {
            SelectWeapon(currentWeapon);
            _hasInitializedCloneWeapon = true;
        }
    }

    public List<Weapon> GetWeaponBodyList => weaponBodyList;
    public Weapon GetCurrentWeapon => currentWeapon;

    public void SelectWeapon(Weapon weapon)
    {
        if (weapon.GetDisplayName() == currentWeapon.GetDisplayName())
        {
            if (_resetWeaponRotationCoroutine == null)
            {
                _resetWeaponRotationCoroutine = StartCoroutine(ResetWeaponRotationRoutine());
            }
            return;
        }
        
        Weapon spawnedWeapon = Instantiate(weapon, currentWeapon.transform.parent);
        spawnedWeapon.transform.localPosition = currentWeapon.transform.localPosition;
        spawnedWeapon.transform.localRotation = Quaternion.identity;
        
        Destroy(currentWeapon.gameObject);
        currentWeapon = spawnedWeapon;
        
        OnWeaponModified?.Invoke();
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
        
        OnWeaponModified?.Invoke();
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

    private IEnumerator ResetWeaponRotationRoutine()
    {
        float rotateSpeed = 5f;
        while (Quaternion.Angle(currentWeapon.transform.localRotation, Quaternion.identity) > 1f)
        {
            if (Input.GetMouseButton(0))
            {
                _resetWeaponRotationCoroutine = null;
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

        _resetWeaponRotationCoroutine = null;
    }
}
