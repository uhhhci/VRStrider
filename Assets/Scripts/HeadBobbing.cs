using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{

    public CharacterController _characterController;
    public Transform _followTarget;
    public float _distanceX = -0.05f;
    public float _distanceY = 0.02f;
    public float _distanceZ = 0.05f;
    private Vector3 _offset = Vector3.zero;
    private List<Vector3> _positionList;
    public int _delay = 10;

    public Vector3 _manualOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _positionList = new List<Vector3>();
    }

    private void FixedUpdate()
    {
        _positionList.Add(_followTarget.position);
        if (_positionList.Count > _delay)
        {
            _positionList.RemoveAt(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 0-1
        float animationState = _characterController._animationState;

        float sineX = Mathf.Sin(animationState * 2 * Mathf.PI);

        if (animationState <= 0.5f)
        {
            animationState *= 2;
        }
        else if (animationState > 0.5f)
        {
            animationState -= 0.5f;
            animationState *= 2;
        }

        if (_positionList.Count > 0)
        {
            //transform.position = _positionList[0];
        }

        transform.localPosition = Vector3.zero;
        float distanceZ = (_positionList[0] - transform.position).magnitude;

        //transform.localPosition -= _offset;

        float sineY = Mathf.Sin(animationState * 2 * Mathf.PI);

        _offset.y = _distanceY * sineY;
        _offset.x = _distanceX * sineX;
        _offset.z = -distanceZ;

        _offset += _manualOffset;

        Vector3 localPos = transform.localPosition;
        localPos += _offset;
        localPos.z = Mathf.Clamp(localPos.z, -_distanceZ, _distanceZ);

        transform.localPosition = localPos;
    }
}
