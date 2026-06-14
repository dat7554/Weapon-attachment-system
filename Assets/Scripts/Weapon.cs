using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public enum AttachmentType
    {
        Scope,
        Handle
    }
    
    [SerializeField] private Transform scopeAttachPoint;
    [SerializeField] private Transform handleAttachPoint;
    
    [SerializeField] private List<GameObject> scopePrefabs;
    [SerializeField] private List<GameObject> handlePrefabs;

    private int _currentScopeIndex = -1;
    private GameObject _currentScope;
    
    private int _currentHandleIndex = -1;
    private GameObject _currentHandle;

    public void SelectAttachment(AttachmentType attachmentType)
    {
        switch (attachmentType)
        {
            case AttachmentType.Scope:
                _currentScopeIndex = GetNextIndex(_currentScopeIndex, scopePrefabs.Count);
        
                if (_currentScope) Destroy(_currentScope);
        
                if (_currentScopeIndex == -1) return;
                _currentScope = Instantiate(scopePrefabs[_currentScopeIndex], scopeAttachPoint);
                
                break;
            case AttachmentType.Handle:
                _currentHandleIndex = GetNextIndex(_currentHandleIndex, handlePrefabs.Count);
        
                if (_currentHandle) Destroy(_currentHandle);
        
                if (_currentHandleIndex == -1) return;
                _currentHandle = Instantiate(handlePrefabs[_currentHandleIndex], handleAttachPoint);
                
                break;
        }
    }

    private int GetNextIndex(int currentIndex, int count)
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