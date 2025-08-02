using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomIn : MonoBehaviour
{

    private Camera camera;
    private float mouseScroll;
    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        mouseScroll = Input.mouseScrollDelta.y * 10 * Time.deltaTime;

        if (camera.orthographicSize >= 1 && camera.orthographicSize <= 15)
        {
            camera.orthographicSize += -mouseScroll; 
        }
    }
}
