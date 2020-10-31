using UnityEngine;
#region using UnityEngine.XR;
#if UNITY_5 || UNITY_2017
#elif UNITY_2018 || UNITY_2019
using UnityEngine.XR;
#endif
#endregion
using UnityEngine.VR;

[DisallowMultipleComponent]
public class VRHelper : MonoBehaviour
{
    static public bool isVRScene = false;

    #region VRDeviceType / XRSettings.supportedDevices
#if UNITY_5 || UNITY_2017
    static private VRDeviceType _vrDeviceType = VRDeviceType.None;
#elif UNITY_2018 || UNITY_2019

#endif
    #endregion

    // 
    void Start()
    {

#if UNITY_5 || UNITY_2017
        SetVRDeviceType(VRDeviceType.Split);
#elif UNITY_2018 || UNITY_2019

#endif
        isVRScene = true;

        //XRSettings.supportedDevices

    }

    // 
    void OnDisable()
    {

#if UNITY_5 || UNITY_2017
        SetVRDeviceType(VRDeviceType.None);
#elif UNITY_2018 || UNITY_2019

#endif
        isVRScene = false;
    }

#if UNITY_5 || UNITY_2017
    // 
    static public void SetVRDeviceType(VRDeviceType vrDeviceType)
    {
        if (_vrDeviceType == vrDeviceType) { return; }

        _vrDeviceType = vrDeviceType;
        VRSettings.loadedDevice = _vrDeviceType;
        bool vr = (_vrDeviceType != VRDeviceType.None);
        VRSettings.showDeviceView = vr;
        VRSettings.enabled = vr;
    }
#elif UNITY_2018 || UNITY_2019
    static public void SetVRDeviceType(string deviceNama)
    {
        XRSettings.LoadDeviceByName(deviceNama);
        bool vr = true;
        XRSettings.showDeviceView = vr;
        XRSettings.enabled = vr;
    }
#endif

}
