using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedalHeightAdjuster : MonoBehaviour
{

    public float _maxHeight = 0.2f;
    private float _minHeight = 0f;
    public GameObject _parent;
    private Vector3 _groundContactPoint;
    private bool _newContactPoint;
    public bool _isLeftFoot;
    private float _lastFire;
    private float _minTime = 0.1f;

    private HapticController _hapticController;

    private int _layerMask;

    public List<AudioSource> _audioSources;
    public CharacterController _characterController;

    private void Start()
    {
        _lastFire = Time.time;
        _hapticController = HapticController.Instance;
        _layerMask = LayerMask.GetMask("Ground_Asphalt", "Ground_Grass", "Ground_Gravel", "Ground_Wood", "Ground_Metal");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = Vector3.zero;
        Vector3 parentPosition = _parent.transform.position;
        Vector3 targetPosition = new Vector3();

        float y = parentPosition.y;

        RaycastHit hit;
        if (Physics.Raycast(parentPosition, Vector3.down, out hit, 1, LayerMask.GetMask("IKBarrier")))
        {
            y = hit.point.y;
            _newContactPoint = false;
        }


        if (y <= _minHeight)
        {
            y = _minHeight;
            if (!_newContactPoint)
            {
                _newContactPoint = true;
                _groundContactPoint = parentPosition;
                _groundContactPoint.y = y;
                FootDown();
            }
        }
        else
        {
            if (_newContactPoint)
            {
                FootUp();
            }
            _newContactPoint = false;
        }
        if (_newContactPoint)
        {
            targetPosition = _groundContactPoint;
        }
        else
        {
            parentPosition.y = y;
            targetPosition = parentPosition;
        }

        transform.position = targetPosition;
    }

    private void FootDown()
    {
        int soundFileIndex = 0;
        RaycastHit hit;
        if (Physics.Raycast(_groundContactPoint, Vector3.down, out hit, 1, _layerMask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Asphalt"))
            {
                soundFileIndex = 0;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Grass"))
            {
                soundFileIndex = 1;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Gravel"))
            {
                soundFileIndex = 2;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Wood"))
            {
                soundFileIndex = 3;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Metal"))
            {
                soundFileIndex = 4;
            }
        }

        if (Time.time > _lastFire + _minTime)
        {
            _lastFire = Time.time;

            float speed = _characterController._speed;
            float volume = Mathf.Abs(speed / 4);
            volume = Mathf.Clamp(volume, 0.2f, 1f);

            if (_characterController._bikeControlEnabled)
            {

                //Debug.Log(soundFileIndex);
                PlayAudio(soundFileIndex, volume);

                if (_characterController._vibroFeedbackEnabled)
                {

                    if (_isLeftFoot)
                    {
                        _hapticController.LeftFootTouchedGround(soundFileIndex, volume);
                    }
                    else
                    {
                        _hapticController.RightFootTouchedGround(soundFileIndex, volume);
                    }
                }
            }
        }
    }

    private void FootUp()
    {
        if (_characterController._vibroFeedbackEnabled)
        {
            if (_isLeftFoot)
            {
                _hapticController.LeftFootTakeOff();
            }
            else
            {
                _hapticController.RightFootTakeOff();
            }
        }
    }

    private void PlayAudio(int soundFileIndex, float volume)
    {
        _audioSources[soundFileIndex].volume = volume;
        _audioSources[soundFileIndex].Play();
    }
}
