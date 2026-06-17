using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WeaponSaveSystem : MonoBehaviour
{
    public static WeaponSaveSystem Instance;
    
    public event Action OnWeaponSaved;
    
    [Serializable]
    public class WeaponSaveData
    {
        public string saveId;
        public string weaponDisplayName;
        public string weaponScreenshotFileName;
        public string savedTime;
        public Weapon.WeaponAttachmentSlotSaveData[] attachments;
    }
    
    [SerializeField] private Camera weaponScreenshotCamera;
    [SerializeField] private Transform cloneContainerTransform;
    [SerializeField] private int screenshotWidth = 1017;
    [SerializeField] private int screenshotHeight = 550;

    private string _savesFolderPath;
    private string _weaponScreenshotsFolderPath;

    private Coroutine _refreshWeaponCloneCoroutine;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        
        weaponScreenshotCamera.enabled = false;
        
        _savesFolderPath = Path.Combine(Application.persistentDataPath, "Saves");
        _weaponScreenshotsFolderPath = Path.Combine(Application.persistentDataPath, "WeaponScreenshots");
    }
    
    private void Start()
    {
        WeaponAttachmentSystem.Instance.OnWeaponModified += WeaponAttachmentSystem_OnWeaponModified;
    }

    private void OnDestroy()
    {
        WeaponAttachmentSystem.Instance.OnWeaponModified -= WeaponAttachmentSystem_OnWeaponModified;
    }

    public void Save()
    {
        Weapon currentWeapon = WeaponAttachmentSystem.Instance.GetCurrentWeapon;
        
        string currentTime = $"{DateTime.Now:yyyyMMdd_HHmmss}";
        string saveId = CreateSaveId(currentWeapon, currentTime);
        
        EnsureSaveFoldersExist();
        
        byte[] screenshotBytes = CaptureScreenshotAsPng();
        
        string weaponScreenshotFileName = $"{saveId}.png";
        string jsonFileName = $"{saveId}.json";
        
        string screenshotFilePath = GetScreenshotFilePath(weaponScreenshotFileName);
        string jsonFilePath = GetJsonFilePath(jsonFileName);

        WeaponSaveData weaponSaveData = new WeaponSaveData
        {
            saveId = saveId,
            weaponDisplayName = currentWeapon.GetDisplayName(),
            weaponScreenshotFileName = weaponScreenshotFileName,
            savedTime = currentTime,
            attachments = currentWeapon.GetAttachmentSaveDataList().ToArray()
        };
        
        SaveScreenshotFile(screenshotFilePath, screenshotBytes);
        SaveJsonFile(jsonFilePath, weaponSaveData);
        
        OnWeaponSaved?.Invoke();
    }

    public List<WeaponSaveData> Load()
    {
        List<WeaponSaveData> weaponSaveDataList = new List<WeaponSaveData>();
        
        if (!CanLoadSaves())
            return weaponSaveDataList;
        
        foreach (string jsonFilePath in GetJsonFilePaths())
        {
            WeaponSaveData weaponSaveData = LoadSaveDataFromJson(jsonFilePath);
            weaponSaveDataList.Add(weaponSaveData);
        }

        return weaponSaveDataList;
    }
    
    public bool TryLoadScreenshotSprite(WeaponSaveData weaponSaveData, out Sprite sprite)
    {
        sprite = null;
        
        string screenshotPath = Path.Combine(
            _weaponScreenshotsFolderPath,
            weaponSaveData.weaponScreenshotFileName
        );
        
        if (!File.Exists(screenshotPath))
        {
            Debug.LogWarning("Missing screenshot: " + screenshotPath);
            return false;
        }
            
        byte[] imageBytes = File.ReadAllBytes(screenshotPath);
        Texture2D screenshotTexture = new Texture2D(2, 2);
        
        if (!screenshotTexture.LoadImage(imageBytes))
        {
            Debug.LogWarning("Failed to load screenshot image: " + screenshotPath);
            Destroy(screenshotTexture);
            return false;
        }
        
        sprite = CreateSpriteFromTexture(screenshotTexture);
        return true;
    }

    private string CreateSaveId(Weapon weapon, string currentTime)
    {
        return $"{SanitizeFileName(weapon.GetDisplayName())}_{currentTime}";
    }

    private void EnsureSaveFoldersExist()
    {
        if (!Directory.Exists(_weaponScreenshotsFolderPath))
            Directory.CreateDirectory(_weaponScreenshotsFolderPath);
        
        if (!Directory.Exists(_savesFolderPath))
            Directory.CreateDirectory(_savesFolderPath);
    }

    private byte[] CaptureScreenshotAsPng()
    {
        RenderTexture renderTexture = new RenderTexture(
            screenshotWidth,
            screenshotHeight,
            24,
            RenderTextureFormat.ARGB32
        );

        RenderTexture previousActiveRenderTexture = RenderTexture.active;
        RenderTexture previousCameraTargetTexture = weaponScreenshotCamera.targetTexture;

        weaponScreenshotCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        weaponScreenshotCamera.Render();

        Texture2D screenshotTexture = new Texture2D(
            screenshotWidth,
            screenshotHeight,
            TextureFormat.RGBA32,
            false
        );

        screenshotTexture.ReadPixels(
            new Rect(0, 0, screenshotWidth, screenshotHeight),
            0,
            0
        );

        screenshotTexture.Apply();

        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
        
        weaponScreenshotCamera.targetTexture = previousCameraTargetTexture;
        RenderTexture.active = previousActiveRenderTexture;

        renderTexture.Release();
        Destroy(renderTexture);
        Destroy(screenshotTexture);

        return screenshotBytes;
    }

    private string GetScreenshotFilePath(string screenshotFileName) => Path.Combine(_weaponScreenshotsFolderPath, screenshotFileName);

    private string GetJsonFilePath(string jsonFileName) => Path.Combine(_savesFolderPath, jsonFileName);
    
    private void SaveScreenshotFile(string screenshotFilePath, byte[] screenshotBytes)
    {
        File.WriteAllBytes(screenshotFilePath, screenshotBytes);
        Debug.Log("Saved weapon screenshot to: " + screenshotFilePath);
    }

    private void SaveJsonFile(string jsonFilePath, WeaponSaveData weaponSaveData)
    {
        string json = JsonUtility.ToJson(weaponSaveData, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Saved weapon json to: " + jsonFilePath);
    }

    private string SanitizeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }

    private bool CanLoadSaves()
    {
        if (!Directory.Exists(_savesFolderPath))
        {
            Debug.Log("'Saves' folder does not exist yet.");
            return false;
        }
        
        if (!Directory.Exists(_weaponScreenshotsFolderPath))
        {
            Debug.Log("'Weapon Screenshots' folder does not exist yet.");
            return false;
        }
        
        return true;
    }

    private string[] GetJsonFilePaths()
    {
        // Lastest json files first
        string[] jsonFilePaths = Directory
            .GetFiles(_savesFolderPath, "*.json")
            .OrderByDescending(File.GetLastWriteTime)
            .ToArray();
        
        return jsonFilePaths;
    }

    private WeaponSaveData LoadSaveDataFromJson(string jsonFilePath)
    {
        Debug.Log("Found save file: " + jsonFilePath);
            
        string json = File.ReadAllText(jsonFilePath);
        WeaponSaveData weaponSaveData = JsonUtility.FromJson<WeaponSaveData>(json);

        return weaponSaveData;
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
    
    private void WeaponAttachmentSystem_OnWeaponModified()
    {
        if (_refreshWeaponCloneCoroutine != null)
            StopCoroutine(_refreshWeaponCloneCoroutine);
        
        _refreshWeaponCloneCoroutine = StartCoroutine(RefreshWeaponCloneRoutine());
    }

    private IEnumerator RefreshWeaponCloneRoutine()
    {
        for (int i = 0; i < cloneContainerTransform.childCount; i++)
        {
            Destroy(cloneContainerTransform.GetChild(i).gameObject);
        }
        
        yield return new WaitForEndOfFrame();
        
        Weapon spawnedClonedWeapon = Instantiate(WeaponAttachmentSystem.Instance.GetCurrentWeapon, cloneContainerTransform);
        spawnedClonedWeapon.transform.localPosition = Vector3.zero;
        spawnedClonedWeapon.transform.localRotation = Quaternion.identity;
        spawnedClonedWeapon.DisableMouseRotate();
    }
}
