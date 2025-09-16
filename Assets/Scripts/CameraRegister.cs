using UnityEngine;
using Unity.Cinemachine;

public class CameraRegsiter : MonoBehaviour
{
    private void OnEnable()
    {
        CameraManager.Register(GetComponent<CinemachineCamera>());
    }
    private void OnDisable()
    {
        CameraManager.Unregister(GetComponent<CinemachineCamera>());
    }
}
