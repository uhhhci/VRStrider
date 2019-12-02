using UnityEngine;
using Valve.VR;
using TMPro;

public class CharacterController : MonoBehaviour
{
    public bool _bikeControlEnabled = true;
    public bool _joystickControlEnabled = false;
    public bool _teleportControlEnabled = false;

    public bool _vibroFeedbackEnabled = true;

    public bool _usePressureTurning = true;
    public bool _useTouchpadTurning = true;

    public bool _useIK;
    public bool _useHandIK;
    public bool _useBlendTree = true;
    public bool _simulated;
    public float _simulatedSpeed = 2;
    public bool _useBikeOffset;
    private float _speedOutput;
    public GameObject _player;
    private GameObject _camera;
    public Animator _animator;
    public IKControl _IKControl;
    public GameObject _axis;
    public float _radius = 0.45f;
    private float _IKBarrierHeight = 0.08f;
    private float _animationProgress = 0;

    public GameObject _pedalLeft;
    public GameObject _pedalRight;
    public PedalHeightAdjuster _adjusterLeft;
    public PedalHeightAdjuster _adjusterRight;

    public float _animationOffset = 0.75f;

    Quaternion _lastRotation;

    public float _speedFactor = 0.66f;
    public GameObject _bike;
    public GameObject _avatar;

    public Transform _realAxis;
    public Transform _realPedal_Main_Left;
    public Transform _realPedal_Reference_Right;
    public Transform _pedalCenteredToAxis;

    public Transform _IKBarrier;

    public float _angle;

    private Vector3 _lastDirection;

    private float[] _angles;
    private int _anglesCounter;
    float _anglesAverage;

    public Transform _cameraVR;
    public Transform _cameraFallback;

    //Must be attached to Player
    public Transform _rotationOriginPoint;

    public Transform _rotationCurrentPoint;
    private bool _rotating = false;
    private Vector3 _lastLocalRotationDirection;
    private Vector3 _initialRotationDirection;
    private Quaternion _initialRotation;

    public Transform _lastDirectionObj;

    public Transform _realPedalCentered;
    public Transform _realAxisCentered;

    public SteamVR_Action_Vector2 m_TouchPosition;
    public SteamVR_Action_Boolean m_Touch;

    public ShoeManager _shoeManager;
    public byte _pressureThreshold = 30;

    private bool _pressureTurning;
    public float _animationState;

    public float _speed;

    private float _axisH;
    private float _axisV;
    private float _mouseX;

    public GameObject _teleportManager;
    public GameObject _textPrompt;
    public GameObject _mesh;

    public GameObject _realBikeTracking;

    // Start is called before the first frame update
    void Start()
    {
        //_lastRotation = _axis.transform.rotation;
        _lastRotation = Quaternion.identity;
        _lastDirection = _avatar.transform.forward;
        _lastDirectionObj.transform.position = _lastDirection;
        _camera = Camera.main.gameObject;
        _angles = new float[30];

        CalculateAxisAngle(false);
    }

    private void Update()
    {
        if (_joystickControlEnabled)
        {
            _axisH = Input.GetAxisRaw("Horizontal");
            _axisV = Input.GetAxisRaw("Vertical");
            _mouseX = Input.GetAxisRaw("Mouse X");
        }
        if (_teleportManager)
        {
            _teleportManager.SetActive(_teleportControlEnabled);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_bikeControlEnabled)
        {

            _IKControl.ikActive = _useIK;
            _IKControl.handIK = _useHandIK;

            CalculateAxisAngle(true);

            Vector2 touchpadAxisLeft = m_TouchPosition[SteamVR_Input_Sources.LeftHand].axis;
            Vector2 touchpadAxisRight = m_TouchPosition[SteamVR_Input_Sources.RightHand].axis;
            float touchpadDeadzone = 0.1f;

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartRotation();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                EndRotation();
            }

            if (_useTouchpadTurning)
            {
                if (m_Touch[SteamVR_Input_Sources.LeftHand].active && touchpadAxisLeft.x < -touchpadDeadzone || m_Touch[SteamVR_Input_Sources.LeftHand].active && touchpadAxisLeft.x > touchpadDeadzone)
                {
                    Turn(touchpadAxisLeft.x);
                }
                if (m_Touch[SteamVR_Input_Sources.RightHand].active && touchpadAxisRight.x < -touchpadDeadzone || m_Touch[SteamVR_Input_Sources.RightHand].active && touchpadAxisRight.x > touchpadDeadzone)
                {
                    Turn(touchpadAxisRight.x);
                }
            }


            if (Input.GetKey(KeyCode.Q))
            {
                Turn(-1);
            }
            if (Input.GetKey(KeyCode.E))
            {
                Turn(1);
            }

            if (_rotating)
            {
                CalculateRotation();
            }

            if (_usePressureTurning)
            {
                DeterminePressureTurning();
            }
        }
        if (_joystickControlEnabled)
        {
            float speed = 0.035f;
            if (Mathf.Abs(_axisV) > 0.25f)
            {
                _player.transform.position += _avatar.transform.forward * _axisV * speed;
            }
            if (Mathf.Abs(_axisH) > 0.25f)
            {
                // _player.transform.position += _avatar.transform.right * _axisH * speed;
            }
            //print(_axisH + " / " + _axisV + " / " + _mouseX);
            Turn(_mouseX);
        }
        if (_teleportControlEnabled)
        {

        }
    }

    public void StartRotation()
    {
        if (_bikeControlEnabled)
        {
            _rotationOriginPoint.position = _rotationCurrentPoint.position;
            _initialRotationDirection = _rotationOriginPoint.position - _camera.transform.position;
            _initialRotation = _player.transform.rotation;
            _rotating = true;
        }
    }

    public void EndRotation()
    {
        _rotating = false;
    }

    private void CalculateRotation()
    {
        Vector3 currentDirectionLocal = _rotationCurrentPoint.position - _camera.transform.position;
        currentDirectionLocal.y = 0;
        Vector3 originDirectionLocal = _rotationOriginPoint.position - _camera.transform.position;
        originDirectionLocal.y = 0;


        float angle = Vector3.SignedAngle(originDirectionLocal, currentDirectionLocal, Vector3.up);

        //TODO: rotate player back to initial rotation
        //_player.transform.rotation = _initialRotation;
        //and then rotate new angle

        Turn(-angle);
        //_player.transform.RotateAround(_camera.transform.position, Vector3.up, -angle);
        //print(angle);
        //StartRotation();

        _rotationOriginPoint.position = _rotationCurrentPoint.position;
    }

    private void MovePlayer(float distance)
    {
        _player.transform.position += _avatar.transform.forward * distance;
    }

    public void Turn(float angle)
    {
        _player.transform.RotateAround(_camera.transform.position, Vector3.up, angle);
    }

    private void CalculateAxisAngle(bool movePlayer)
    {
        if (_simulated)
        {
            _axis.transform.Rotate(new Vector3(_simulatedSpeed, 0, 0));

        }
        else
        {
            //Calculate position of real Axis
            _realAxis.transform.position = Vector3.Lerp(_realPedal_Main_Left.transform.position, _realPedal_Reference_Right.transform.position, 0.5f);
            _realAxis.transform.rotation = _realPedal_Main_Left.transform.rotation;

            Vector3 tmpPos = _realAxis.transform.position;
            Quaternion tmpRot = _realAxis.transform.rotation;

            //_realBikeTracking.transform.position = _realAxis.transform.position;
            //_realBikeTracking.transform.rotation = _realAxis.transform.rotation;

            _realAxis.transform.position = tmpPos;
            _realAxis.transform.rotation = tmpRot;

            _realAxisCentered.position = _realAxis.position;
            _realAxisCentered.rotation = _realAxis.rotation;
            _pedalCenteredToAxis.position = _realPedal_Main_Left.position;
            _pedalCenteredToAxis.rotation = _realPedal_Main_Left.rotation;


            Vector3 realAxisCenteredLocal = _realAxisCentered.localPosition;
            realAxisCenteredLocal.x = 0;
            _realAxisCentered.localPosition = realAxisCenteredLocal;

            Vector3 realPedalCenteredLocal = _pedalCenteredToAxis.localPosition;
            realPedalCenteredLocal.x = 0;
            _pedalCenteredToAxis.localPosition = realPedalCenteredLocal;

            _realAxisCentered.position = _pedalCenteredToAxis.position;
            _realAxisCentered.rotation = _pedalCenteredToAxis.rotation;

            //Vector3 direction = _realPedal.position - _realAxis.position;
            Vector3 direction = _realAxisCentered.localPosition - realAxisCenteredLocal;

            //DEBUG
            //print(direction.magnitude);

            //TODO: reset _axis X rotation and instead of _lastDirection calculate each frame from _axis.transform.forward to current angle

            //float angle = Vector3.SignedAngle(_lastDirection, direction, Vector3.Cross(_realAxis.transform.forward, _realAxis.transform.up));
            float angle = Vector3.SignedAngle(_lastDirection, direction, Vector3.right);
            _lastDirection = direction;

            //DEBUG
            //angle *= 2f;

            _axis.transform.Rotate(new Vector3(angle, 0, 0));

            /*
             
              _axis.transform.localRotation = Quaternion.identity;
            float angle = Vector3.SignedAngle(_avatar.transform.forward, direction, _axis.transform.right);
            _lastDirection = direction;

            _axis.transform.Rotate(new Vector3(angle, 0, 0));
              
             */

        }


        Vector3 posLeft = _pedalLeft.transform.localPosition;
        posLeft.z = _radius;
        _pedalLeft.transform.localPosition = posLeft;

        Vector3 posRight = _pedalRight.transform.localPosition;
        posRight.z = -_radius;
        _pedalRight.transform.localPosition = posRight;


        //Feet Positions
        Quaternion currentRotation = _axis.transform.rotation;

        float signedAngle = Vector3.SignedAngle(_lastRotation * _axis.transform.forward, currentRotation * _axis.transform.forward, _axis.transform.right);

        float _angle = Quaternion.Angle(_lastRotation, currentRotation);
        float sign = 1;
        if (signedAngle < 0)
        {
            sign = -1;
        }
        _angle *= sign;


        _angles[_anglesCounter] = _angle;

        _anglesCounter++;
        if (_anglesCounter >= _angles.Length)
        {
            _anglesCounter = 0;
        }

        _anglesAverage = 0;
        for (int i = 0; i < _angles.Length; i++)
        {
            _anglesAverage += _angles[i];
        }
        _anglesAverage = _anglesAverage / _angles.Length;

        //define radius dependent on speedoutput or distance
        _radius = 0.15f * _anglesAverage;

        //DEBUG
        //_radius = 0.3f;
        _radius = Mathf.Clamp(_radius, 0.15f, 0.5f);

        _IKBarrierHeight = 0.08f * _anglesAverage / 2;
        _IKBarrierHeight = Mathf.Clamp(_IKBarrierHeight, 0.08f, 0.1f);
        Vector3 localBarrierHeight = _IKBarrier.localPosition;
        localBarrierHeight.y = _IKBarrierHeight;
        //_IKBarrier.localPosition = localBarrierHeight;
        _IKBarrier.localScale = new Vector3(_radius * 3, 1, _radius * 3);

        float distance = _speedFactor * 2 * Mathf.PI * _radius * _angle / 360;
        _speedOutput = distance;

        if (movePlayer)
        {
            MovePlayer(distance);
        }


        //animation
        _animationProgress += _angle / 360;

        if (_animationProgress >= 1)
        {
            _animationProgress--;
        }
        else if (_animationProgress < 0)
        {
            _animationProgress++;
        }

        _animationState = _animationOffset + _animationProgress;
        if (_animationState >= 1)
        {
            _animationState--;
        }
        else if (_animationState < 0)
        {
            _animationState++;
        }

        //TODO: find good translation coefficient
        _speed = _anglesAverage;
        _animator.SetFloat("Speed", _speed);

        if (_useBlendTree)
        {
            _animator.Play("Blend Tree", 0, _animationState);

        }
        else
        {
            _animator.Play("Walk", 0, _animationState);
        }


        _lastRotation = currentRotation;


        if (_useBikeOffset)
        {
            //TODO: player slowly rotates towards camera forward so that the head goes to a neutral position
            //TODO: move bike further back with greater speed
            float bikeOffset = -0.1f - _speedOutput * 2;
            bikeOffset = Mathf.Clamp(bikeOffset, -0.3f, 0.1f);
            _bike.transform.localPosition = new Vector3(0, 0, bikeOffset);
        }
    }

    private void DeterminePressureTurning()
    {
        byte[] left = _shoeManager.GetPressureSensorDataLeft();
        byte[] right = _shoeManager.GetPressureSensorDataRight();

        if (!((left == null) || (right == null)))
        {

            float totalPressureLeft = 0;

            for (int i = 0; i < 5; i++)
            {
                totalPressureLeft += left[i];
            }

            float totalPressureRight = 0;

            for (int i = 0; i < 5; i++)
            {
                totalPressureRight += right[i];
            }


            float totalPressureDifference = totalPressureRight - totalPressureLeft;

            float finalAngle = 0;


            //determine body to view direction angle
            Vector3 camForward = _camera.transform.forward;
            camForward.y = 0;
            Vector3 bodyForward = _avatar.transform.forward;
            bodyForward.y = 0;

            float angle = Vector3.SignedAngle(bodyForward, camForward, Vector3.up);

            finalAngle = Mathf.Log(Mathf.Abs(angle));

            if (angle < 0)
            {
                finalAngle *= -1;
            }

            float multiplier = 0.1f;
            finalAngle *= multiplier;

            float deadzone = 10f;


            //if looking to the center reset pressure turning
            if (Mathf.Abs(angle) < deadzone)
            {
                _pressureTurning = false;
            }
            //if looking to the side
            else
            {
                //if looking right and apply pressure on right foot
                if ((finalAngle > 0 && totalPressureDifference > _pressureThreshold) || (finalAngle < 0 && totalPressureDifference < -_pressureThreshold))
                {
                    _pressureTurning = true;
                }

                //if (_pressureTurning && Mathf.Abs(_anglesAverage) > 0.5f)
                if (_pressureTurning)
                {
                    Turn(finalAngle);
                }
            }

        }
    }

    public void ShowTextPrompt(string text)
    {
        _textPrompt.SetActive(true);
        _textPrompt.GetComponentInChildren<TextMeshPro>().SetText(text);
        Invoke("HideTextPrompt", 5f);
    }

    private void HideTextPrompt()
    {
        _textPrompt.SetActive(false);
    }


    public void ShowAvatar(bool show)
    {
        SkinnedMeshRenderer[] _meshes = _mesh.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].enabled = show;
        }
    }

    public void Teleported()
    {
        _player.transform.position += new Vector3(0, -_avatar.transform.localPosition.y, 0);
    }
}
