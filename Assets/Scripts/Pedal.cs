using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pedal : MonoBehaviour
{
    public bool _simulated;
    [Range(-5, 5)]
    public float _simulatedSpeed = 1;
    public float _speedFactor = 1f;
    public GameObject _player;
    public GameObject _axis;
    public GameObject _tracker;
    public GameObject _joint;
    public GameObject _leftController;
    public bool _limitSpeed = false;
    public float _speedLimit = 500;
    private float _playerHeight;

    private Vector3 _minTrackerPos;
    private Vector3 _maxTrackerPos;
    private Vector3 _axisCenter;

    public Animator _animator;
    public bool _freezeZAxis;

    Quaternion lastRotation;
    float magnitude;
    Vector3 axis;
    Vector3 angularVelocity;
    float deltaAngle;

    private bool _calibrated = false;

    private Vector3 _forward;
    public bool _FreezePlayerY;

    // Use this for initialization
    void Start()
    {
        lastRotation = _axis.transform.localRotation;
        ResetPedalOrigin(false);
        _playerHeight = _player.transform.position.y;

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Menu))
        //{
        //    ResetPedalOrigin(true);
        //}

        //if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Menu))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}

        //if (ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Pad))
        //{
        //    if (ViveInput.GetAxis(HandRole.LeftHand, ControllerAxis.PadX) > 0.6f)
        //    {
        //        Rotate(1);
        //    }
        //    else if (ViveInput.GetAxis(HandRole.LeftHand, ControllerAxis.PadX) < -0.6f)
        //    {
        //        Rotate(-1);
        //    }
        //}

        if (_simulated)
        {
            _axis.transform.Rotate(new Vector3(_simulatedSpeed, 0, 0));
        }
        else
        {

            FindAxisCenter();

            _joint.transform.position = _tracker.transform.position;
            _joint.transform.localPosition = new Vector3(_joint.transform.localPosition.x, _joint.transform.localPosition.y, 0);


            _axis.transform.LookAt(_joint.transform);

            Quaternion rot = _axis.transform.localRotation;
            Vector3 rotV = rot.eulerAngles;
            if (_freezeZAxis)
                rotV.z = 0;

            if (rotV.y < 0 || rotV.y > 260)
            {

                rotV.x = 180 - rotV.x;
            }
            rotV.y = 90;

            rot.eulerAngles = rotV;
            _axis.transform.localRotation = rot;

        }

        /*
        var lookPos = _joint.transform.position - transform.position;
        lookPos.x = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        */
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        //_axis.transform.rotation = rotation;

        Quaternion deltaRotation = _axis.transform.localRotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out magnitude, out axis);


        deltaAngle = Quaternion.Angle(_axis.transform.localRotation, Quaternion.Inverse(lastRotation));

        lastRotation = _axis.transform.localRotation;

        Vector3 newVelocity = (axis * magnitude) / Time.deltaTime;

        if (_limitSpeed)
        {
            if (newVelocity.z > _speedLimit)
            {
                newVelocity.z = _speedLimit;
            }
            else if (newVelocity.z < -_speedLimit)
            {
                newVelocity.z = -_speedLimit;
            }
        }

        if (newVelocity.z > -5 && newVelocity.z < 5)
        {
            newVelocity.z = 0;
        }

        angularVelocity = Vector3.Lerp(angularVelocity, newVelocity, Time.deltaTime);

        MovePlayer(angularVelocity);
    }

    private void FindAxisCenter()
    {
        if (!_calibrated)
        {
            _minTrackerPos = _tracker.transform.position;
            _maxTrackerPos = _tracker.transform.position;
        }

        if (_calibrated)
        {
            Vector3 currentTrackerPosition = _tracker.transform.position;
            if (currentTrackerPosition.x > _maxTrackerPos.x)
            {
                _maxTrackerPos.x = currentTrackerPosition.x;
            }
            if (currentTrackerPosition.x < _minTrackerPos.x)
            {
                _minTrackerPos.x = currentTrackerPosition.x;
            }

            if (currentTrackerPosition.y > _maxTrackerPos.y)
            {
                _maxTrackerPos.y = currentTrackerPosition.y;
            }
            if (currentTrackerPosition.y < _minTrackerPos.y)
            {
                _minTrackerPos.y = currentTrackerPosition.y;
            }

            if (currentTrackerPosition.z > _maxTrackerPos.z)
            {
                _maxTrackerPos.z = currentTrackerPosition.z;
            }
            if (currentTrackerPosition.z < _minTrackerPos.z)
            {
                _minTrackerPos.z = currentTrackerPosition.z;
            }

            //_axisCenter = Vector3.Lerp(_minTrackerPos, _maxTrackerPos, 0.5f);
            _axisCenter = (_minTrackerPos + _maxTrackerPos) / 2;
            transform.position = _axisCenter;
        }
    }

    private void FixedUpdate()
    {
        if (_calibrated)
        {
            // MovePlayer(angularVelocity);


        }
    }

    //Place Vive tracker on left pedal and trigger this function when left pedal is at highest point with trigger ontop
    public void ResetPedalOrigin(bool manual)
    {
        if (manual)
        {
            _calibrated = true;
            _minTrackerPos = _tracker.transform.position;
            _maxTrackerPos = _tracker.transform.position;
        }

        //transform.position = _axisCenter;

        /*
        Quaternion rot = transform.rotation;
        Vector3 rotV = rot.eulerAngles;
        Vector3 trackerV = _tracker.transform.rotation.eulerAngles;
        rotV.y = trackerV.y;
        rot.eulerAngles = rotV;
        transform.rotation = rot;
        */
        //transform.rotation = _tracker.transform.rotation;t
        Vector3 rot = _leftController.transform.rotation.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Euler(rot);

        angularVelocity = Vector3.zero;

        _forward = _axis.transform.forward;
        /*
        _player.transform.rotation = _axis.transform.rotation;
        Quaternion pRot = _player.transform.rotation;
        Vector3 pRotV = pRot.eulerAngles;
        pRotV.x = 0;
        pRotV.z = 0;
        pRot.eulerAngles = pRotV;
        _player.transform.rotation = pRot;
        */

        //SteamVR.instance.hmd.ResetSeatedZeroPose();
    }

    private void MovePlayer(Vector3 movement)
    {
        movement *= _speedFactor;
        float speed = movement.z / 10000f;
        _player.transform.position += _forward * speed;

        if (_FreezePlayerY)
        {
            Vector3 pos = _player.transform.position;
            pos.y = _playerHeight;
            //pos.y = 0;
            _player.transform.position = pos;
        }

        //TODO:
        //Change avatar walking speed. ~ 0.25 x pedaling speed
        _animator.SetFloat("Speed", movement.z / 180);
        //_animator.SetFloat("Speed", 2f);
    }

    public void Rotate(int rotation)
    {
        _forward = Quaternion.AngleAxis(rotation, Vector3.up) * _forward;
        _player.transform.RotateAround(Camera.main.transform.position, new Vector3(0, 1, 0), rotation);
    }

    public Vector3 GetAngularVelocity()
    {
        return angularVelocity;
    }
}
