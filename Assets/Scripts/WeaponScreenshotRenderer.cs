using Unity.Cinemachine;
using UnityEngine;

public class WeaponScreenshotRenderer : MonoBehaviour
{
    [SerializeField] private Camera screenshotCamera;
    [SerializeField] private CinemachineBrain screenshotBrain;
    [SerializeField] private CinemachineCamera screenshotCinemachineCamera;
    
    [SerializeField] private int screenshotWidth = 1017;
    [SerializeField] private int screenshotHeight = 550;
    
    private int _manualFrame;
    
    private void Awake()
    {
        screenshotCamera.enabled = false;

        screenshotBrain.UpdateMethod =
            CinemachineBrain.UpdateMethods.ManualUpdate;

        screenshotBrain.ChannelMask = OutputChannels.Channel01;
        screenshotCinemachineCamera.OutputChannel = OutputChannels.Channel01;
    }
    
    // TODO: current Cinemachine adaption has made this screenshot not working as intended
    public byte[] CaptureScreenshotAsPng()
    {
        RenderTexture renderTexture = new RenderTexture(
            screenshotWidth,
            screenshotHeight,
            24,
            RenderTextureFormat.ARGB32
        );

        RenderTexture previousActiveRenderTexture = RenderTexture.active;
        RenderTexture previousCameraTargetTexture = screenshotCamera.targetTexture;
        Texture2D screenshotTexture = null;

        try
        {
            screenshotCinemachineCamera.Priority = 100;

            // -1 removes damping so the camera immediately reaches its target.
            screenshotBrain.ManualUpdate(++_manualFrame, -1f);

            screenshotCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            screenshotCamera.Render();

            screenshotTexture = new Texture2D(
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

            return screenshotTexture.EncodeToPNG();
        }
        finally
        {
            screenshotCamera.targetTexture = previousCameraTargetTexture;
            RenderTexture.active = previousActiveRenderTexture;

            renderTexture.Release();
            Destroy(renderTexture);
            
            if (screenshotTexture != null)
                Destroy(screenshotTexture);
        }
    }
}
