using UnityEngine;
using Unity.Cinemachine;

public class SwitchCam : MonoBehaviour
{
    public CinemachineCamera cam1;
    public CinemachineCamera cam2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            CameraManager.SwitchCamera(cam1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraManager.SwitchCamera(cam2);
        }
    }



}
