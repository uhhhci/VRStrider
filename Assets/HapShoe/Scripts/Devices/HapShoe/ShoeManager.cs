
/**
 *  Haptics Framework
 *
 *  UHH HCI 
 *  Author: Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeManager : MonoBehaviour
{
    [SerializeField]
    public DeviceServer deviceServer;

    public bool useHeatMap = false;
    public int hMFactor = 50;

    Vector3 RightFootPoint1 = new Vector3(0.9f, -2.6f, 0.0f);
    Vector3 RightFootPoint2 = new Vector3(2.1f, -2.6f, 0.0f);
    Vector3 RightFootPoint3 = new Vector3(2.4f, -0.4f, 0.0f);
    Vector3 RightFootPoint4 = new Vector3(1.0f, 1.5f, 0.0f);
    Vector3 RightFootPoint5 = new Vector3(2.8f, 1.4f, 0.0f);
    Vector3 LeftFootPoint1 = new Vector3(0.9f - 3.15f, -2.6f, 0.0f);
    Vector3 LeftFootPoint2 = new Vector3(2.1f - 3.15f, -2.6f, 0.0f);
    Vector3 LeftFootPoint3 = new Vector3(2.4f - 5.0f, -0.4f, 0.0f);
    Vector3 LeftFootPoint4 = new Vector3(1.0f - 3.82f, 1.4f, 0.0f);
    Vector3 LeftFootPoint5 = new Vector3(2.8f - 3.82f, 1.5f, 0.0f);

    [Range(0, 255)] public byte UmbralPressurePoint1 = 200;
    [Range(0, 255)] public byte UmbralPressurePoint2 = 200;
    [Range(0, 255)] public byte UmbralPressurePoint3 = 200;
    [Range(0, 255)] public byte UmbralPressurePoint4 = 200;
    [Range(0, 255)] public byte UmbralPressurePoint5 = 200;

    private HapShoe hapShoeRight = null;
    private HapShoe hapShoeLeft = null;

    private float _lastVolume;
    private int _index;
    private int _volume;


    void Update()
    {

        if (hapShoeRight == null && deviceServer.connectedDevicesByName.ContainsKey("HapShoeRight"))
        {
            hapShoeRight = (HapShoe)deviceServer.connectedDevicesByName["HapShoeRight"];
            hapShoeRight.SetWaitFlag(false);
        }

        if (hapShoeRight != null)
        {


            //if (hapShoeRight.pressure[0] > UmbralPressurePoint1) hapShoeRight.Play(3);
            //if (hapShoeRight.pressure[1] > UmbralPressurePoint2) hapShoeRight.Play(3);
            //if (hapShoeRight.pressure[2] > UmbralPressurePoint3) hapShoeRight.Play(2);
            //if (hapShoeRight.pressure[3] > UmbralPressurePoint4) hapShoeRight.Play(4);
            //if (hapShoeRight.pressure[4] > UmbralPressurePoint5) hapShoeRight.Play(4);
        }

        if (hapShoeLeft == null && deviceServer.connectedDevicesByName.ContainsKey("HapShoeLeft"))
        {
            hapShoeLeft = (HapShoe)deviceServer.connectedDevicesByName["HapShoeLeft"];
            hapShoeLeft.SetWaitFlag(false);
        }

        if (hapShoeLeft != null)
        {

            //if (hapShoeLeft.pressure[0] > UmbralPressurePoint1) hapShoeLeft.Play(1);
            //if (hapShoeLeft.pressure[1] > UmbralPressurePoint2) hapShoeLeft.Play(2);
            //if (hapShoeLeft.pressure[2] > UmbralPressurePoint3) hapShoeLeft.Play(3);
            //if (hapShoeLeft.pressure[3] > UmbralPressurePoint4) hapShoeLeft.Play(4);
            //if (hapShoeLeft.pressure[4] > UmbralPressurePoint5) hapShoeLeft.Play(5);
        }
    }

    public void VibrateRightStrong(int soundFileIndex, float volume)
    {
        if (hapShoeRight != null)
        {
            _volume = (int)(volume * 100);

            _index = soundFileIndex;
            Invoke("PlayRight", 0.05f);
        }
    }


    public void VibrateLeftStrong(int soundFileIndex, float volume)
    {
        if (hapShoeLeft != null)
        {
            _volume = (int)(volume * 100);

            _index = soundFileIndex;
            Invoke("PlayLeft", 0.05f);
        }
    }

    private void PlayLeft()
    {
        hapShoeLeft.Play(_index, _volume);
    }
    private void PlayRight()
    {
        hapShoeRight.Play(_index, _volume);
    }


    public byte[] GetPressureSensorDataRight()
    {
        if (hapShoeRight)
        {
            byte[] result = new byte[5];

            for (int i = 0; i < 5; i++)
            {
                result[i] = hapShoeRight.pressure[i];
            }

            return result;
        }
        else
        {
            return null;
        }
    }

    public byte[] GetPressureSensorDataLeft()
    {
        if (hapShoeLeft)
        {
            byte[] result = new byte[5];

            for (int i = 0; i < 5; i++)
            {
                result[i] = hapShoeLeft.pressure[i];
            }

            return result;
        }
        else
        {
            return null;
        }
    }
}