using UnityEngine;
using UnityEngine.Networking;

public class MouseLook : NetworkBehaviour
{
    public Camera PlayerCamera => _camera;

    [Range(0.1f, 10.0f)]
    [SerializeField] private float sensitivity = 2.0f;
    [Range(-90.0f, .0f)]
    [SerializeField] private float minVert = -45.0f;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float maxVert = 45.0f;

    private float rotationX = .0f;
    private float rotationY = .0f;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        var rb = GetComponentInChildren<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;
    }

    public void Rotation()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, minVert, maxVert);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        _camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}

