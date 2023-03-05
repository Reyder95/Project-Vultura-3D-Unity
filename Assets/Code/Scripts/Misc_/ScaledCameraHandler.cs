using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledCameraHandler : MonoBehaviour
{
    public Camera mainCamera;
    public Transform referencePoint;
    public int scaleFactor;
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.rotation = mainCamera.gameObject.transform.rotation;
        this.gameObject.transform.position = referencePoint.position + (mainCamera.gameObject.transform.position / (scaleFactor * 100));
    }
}
