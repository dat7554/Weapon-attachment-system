using UnityEngine;
using UnityEngine.UI;

public class SavedWeaponButtonUI : MonoBehaviour
{
    [SerializeField] private Image savedWeaponImage;

    public void SetSprite(Sprite sprite)
    {
        savedWeaponImage.sprite = sprite;
    }
}
