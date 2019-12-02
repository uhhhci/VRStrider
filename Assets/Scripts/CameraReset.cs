using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReset : MonoBehaviour
{
    public GameObject _camera;
    public GameObject _cameraOrigin;
    public GameObject _player;
    public GameObject _model;
    public GameObject _headbobbing;

    // Start is called before the first frame update
    void Start()
    {
        ResetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCamera();
        }
    }

    public void ResetCamera()
    {
        //vector to target minus vector to current camera, applied to whole player but not model
        _headbobbing.transform.localPosition = Vector3.zero;

        Vector3 offset = _cameraOrigin.transform.position - _camera.transform.position;
        _player.transform.position += offset;
        _model.transform.position -= offset;

        float signedAngle = Vector3.SignedAngle(_camera.transform.forward, _model.transform.forward, Vector3.up);

        _player.transform.RotateAround(_cameraOrigin.transform.position, Vector3.up, signedAngle);
        _model.transform.RotateAround(_cameraOrigin.transform.position, Vector3.up, -signedAngle);
        print("Camera reset");
    }
}
