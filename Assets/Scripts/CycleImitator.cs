using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleImitator : MonoBehaviour
{
    public float _stepLength = 1.5f;
    public float _stepHeight = 0.12f;
    private float _stepHeightCurrent;
    public float _hipWidth = 0.1662f;
    public GameObject _originalAxis;
    public GameObject _imitatorAxis;
    public GameObject _pedalLeft;
    public GameObject _pedalRight;
    public GameObject _pedalJointLeft;
    public GameObject _pedalJointRight;
    public GameObject _player;
    public Pedal _cycle;
    public Animator _animator;

    Quaternion _lastRotation;
    Quaternion _lastRotationFixed;
    private float _radius = 0;

    private Vector3 _forward;
    public bool _FreezePlayerY;
    private float _playerHeight;

    private float _movement;

    // Use this for initialization
    void Start()
    {
        _stepHeightCurrent = _stepHeight;
        _playerHeight = _player.transform.position.y;
        _lastRotation = _imitatorAxis.transform.localRotation;
        _lastRotationFixed = _lastRotation;
        _radius = Vector3.Distance(_pedalJointLeft.transform.position, _pedalLeft.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rot = _originalAxis.transform.localRotation.eulerAngles;
        rot.y += 90;
        _imitatorAxis.transform.localRotation = Quaternion.Euler(rot);

        //Movement
        float distancePerDegree = 2 * Mathf.PI * _radius / 360;

        float deltaAngle = Quaternion.Angle(_imitatorAxis.transform.localRotation, Quaternion.Inverse(_lastRotationFixed));
        _lastRotationFixed = _imitatorAxis.transform.localRotation;

        float fixedMovement = distancePerDegree * deltaAngle;

        //Calculate step height for blending walking and standing animation

        if (fixedMovement < 0.001f)
        {
            _stepHeightCurrent = Mathf.Lerp(_stepHeightCurrent, 0, Time.deltaTime);
        }
        else
        {
            _stepHeightCurrent = Mathf.Lerp(_stepHeightCurrent, _stepHeight, Time.deltaTime);
        }
        _stepHeightCurrent = Mathf.Clamp(_stepHeightCurrent, 0, _stepHeight);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rot = _originalAxis.transform.localRotation.eulerAngles;
        rot.y += 90;
        _imitatorAxis.transform.localRotation = Quaternion.Euler(rot);
        
        //Movement
        float distancePerDegree = 2 * Mathf.PI * _radius / 360;

        float deltaAngle = Quaternion.Angle(_imitatorAxis.transform.localRotation, Quaternion.Inverse(_lastRotation));
        _lastRotation = _imitatorAxis.transform.localRotation;

        _movement = distancePerDegree * deltaAngle;

       

        //Movement pattern limits

        _pedalLeft.transform.localPosition = new Vector3(0, -_stepLength, 0);
        if (_pedalLeft.transform.position.y < 0)
        {
            _pedalLeft.transform.position = new Vector3(_pedalLeft.transform.position.x, 0, _pedalLeft.transform.position.z);
        }
        else if (_pedalLeft.transform.position.y > _stepHeightCurrent)
        {
            _pedalLeft.transform.position = new Vector3(_pedalLeft.transform.position.x, _stepHeightCurrent, _pedalLeft.transform.position.z);
        }


        _pedalRight.transform.localPosition = new Vector3(0, -_stepLength, 0);
        if (_pedalRight.transform.position.y < 0)
        {
            _pedalRight.transform.position = new Vector3(_pedalRight.transform.position.x, 0, _pedalRight.transform.position.z);
        }
        else if (_pedalRight.transform.position.y > _stepHeightCurrent)
        {
            _pedalRight.transform.position = new Vector3(_pedalRight.transform.position.x, _stepHeightCurrent, _pedalRight.transform.position.z);
        }


        _pedalJointLeft.transform.localPosition = new Vector3(_hipWidth / 2, 0, 0);
        _pedalJointRight.transform.localPosition = new Vector3(-_hipWidth / 2, 0, 0);


        MovePlayer(_movement);
    }

    private void MovePlayer(float movement)
    {

        float angularVelocity = _cycle.GetAngularVelocity().z;

        if (angularVelocity < 0)
        {
            movement *= -1;
        }


        _player.transform.position += _player.transform.forward * movement;

        if (_FreezePlayerY)
        {
            Vector3 pos = _player.transform.position;
            pos.y = _playerHeight;
            _player.transform.position = pos;
        }

        //TODO:
        //Change avatar walking speed. ~ 0.25 x pedaling speed
        _animator.SetFloat("Speed", movement * 200);
        //_animator.SetFloat("Speed", 2f);
    }
}
