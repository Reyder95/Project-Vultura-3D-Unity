using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    // How fast we want to rotate the camera
    private float mouseSensitivity = 3.0f;

    // The current rotation around the object
    private float _rotationY = 90;
    private float _rotationX = 30;

    // The target's transform. The object we are looking at. This is not the player, which allows us to adjust the camera's position relative to the player.
    [SerializeField]
    private Transform _target;

    // The player itself, allowing us to make the target and player move together.
    [SerializeField]
    private Transform _player;

    // The distance the camera is from the target, we use this for zoom.
    private float _distanceFromTarget = 50.0f;

    // The current rotation of the camera in relation to the target.
    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;     // Is intended for smoothing. Will use for zooming in and out --TODO--

    //private float _smoothTime = 3.0f;   // --TODO-- Use for zooming

    // Sets camera position behind ship. Lets you switch from ship to ship and it will place the camera accordingly
    void SetCameraPosition()
    {
        Vector3 _currentRotation = new Vector3(_rotationX, _rotationY);
        transform.localEulerAngles = _currentRotation;
        transform.position = _target.position - transform.right * _distanceFromTarget;
    }

    // Reinitialize the camera behind the new player ship
    public void ReinitializeCamera(GameObject newPlayer)
    {
        _player = newPlayer.transform;
        _target = newPlayer.transform.GetChild(0);

        SetCameraPosition();
    }

    // Makes camera follow the ship
    void FixedUpdate()
    {
        // Makes target follow the player
        _target.position = _player.position;
    }

    // Update is called once per frame
    void Update()
    {
        // When player holds down right click
        if (Input.GetMouseButton(1))
        {
            // Set cursor to visible and lock the cursor (this moves cursor to center upon release)
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // Set mouse X and Y values based on mouse movement and mouse sensitivity.
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Adjust rotation
            _rotationY += mouseX;
            _rotationX -= mouseY;

            // Clamp rotation
            _rotationX = Mathf.Clamp(_rotationX, -80, 80);

            // Set the current rotation based on newly set X and Y values
            Vector3 _currentRotation = new Vector3(_rotationX, _rotationY);
            transform.localEulerAngles = _currentRotation;
        }
        else
        {
            // If mouse is not being held down, re-show the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Set distance based on zoom level. This handles zooming
        _distanceFromTarget -= Input.GetAxis("Mouse ScrollWheel") * 10.0f;
        _distanceFromTarget = Mathf.Clamp(_distanceFromTarget, 30.0f, 1000.0f);
        
        // Set new camera transform.
        transform.position = _target.position - transform.forward * _distanceFromTarget;
    }
}
