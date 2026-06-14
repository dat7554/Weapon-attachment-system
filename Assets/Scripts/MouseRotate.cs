using UnityEngine;

public class MouseRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 0.2f;
    
    private Vector3 _lastMousePosition;
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = _lastMousePosition - Input.mousePosition;
            
            mouseDelta.x = Mathf.Clamp(mouseDelta.x, -200, 200);
            mouseDelta.y = Mathf.Clamp(mouseDelta.y, -200, 200);

            transform.localEulerAngles += new Vector3(mouseDelta.y, mouseDelta.x) * rotateSpeed;
            float eulerAnglesX = transform.eulerAngles.x;
            if (eulerAnglesX > 180)
                eulerAnglesX -= 360;
            float rotationX = Mathf.Clamp(eulerAnglesX, -7, 7);

            transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
        
        _lastMousePosition = Input.mousePosition;
    }
}
