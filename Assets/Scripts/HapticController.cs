using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR;

public class HapticController : MonoBehaviour
{

    public static HapticController Instance;

    public bool _useHapShoes;
    public SteamVR_Action_Vibration hapticAction;
    public ShoeManager _shoeManager;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.HardwareTracker);
        //HapticCapabilities capabilities;
        //if (device.TryGetHapticCapabilities(out capabilities))
        //{
        //    if (capabilities.supportsImpulse)
        //    {
        //        uint channel = 0;
        //        float amplitude = 0.5f;
        //        float duration = 1.0f;
        //        device.SendHapticImpulse(channel, amplitude, duration);
        //    }
        //}

    }

    public void LeftFootTouchedGround(int soundFileIndex, float volume)
    {
        if (_useHapShoes)
        {
            _shoeManager.VibrateLeftStrong(soundFileIndex, volume);
        }
        else
        {
            Pulse(0.125f, 150, 100, SteamVR_Input_Sources.LeftFoot);
            Pulse(0.125f, 150, 100, SteamVR_Input_Sources.LeftHand);
            print("LEFT foot DOWN");
        }
    }

    public void RightFootTouchedGround(int soundFileIndex, float volume)
    {
        if (_useHapShoes)
        {
            _shoeManager.VibrateRightStrong(soundFileIndex, volume);
        }
        else
        {
            Pulse(0.125f, 150, 100, SteamVR_Input_Sources.RightFoot);
            Pulse(0.125f, 150, 100, SteamVR_Input_Sources.RightHand);
            print("RIGHT foot DOWN");
        }
    }

    public void LeftFootTakeOff()
    {
        if (_useHapShoes)
        {
           // _shoeManager.VibrateLeftLight();
        }
        else
        {
            Pulse(0.125f, 75, 70f, SteamVR_Input_Sources.LeftFoot);
            Pulse(0.125f, 75, 70f, SteamVR_Input_Sources.LeftHand);
            print("LEFT foot UP");
        }
    }

    public void RightFootTakeOff()
    {
        if (_useHapShoes)
        {
           // _shoeManager.VibrateRightLight();
        }
        else
        {
            Pulse(0.125f, 75, 70f, SteamVR_Input_Sources.RightFoot);
            Pulse(0.125f, 75, 70f, SteamVR_Input_Sources.RightHand);
            print("RIGHT foot UP");
        }
    }

    private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
    {
        if (hapticAction.active)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, source);
        }
    }


}
