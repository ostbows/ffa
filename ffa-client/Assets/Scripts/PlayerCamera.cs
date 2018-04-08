using UnityEngine;
using UnityEngine.Networking;

public class PlayerCamera : NetworkBehaviour
{
    CameraMotor cameraMotor;

    void Start()
    {
        if (!isLocalPlayer) Destroy(this);
    }

    public override void OnStartLocalPlayer()
    {
        cameraMotor = Camera.main.GetComponent<CameraMotor>();

        if (cameraMotor != null)
        {
            cameraMotor.player = gameObject;
            cameraMotor.SetOffset();
        }
    }

    void OnDestroy()
    {
        if (cameraMotor != null)
        {
            cameraMotor.player = null;
            cameraMotor.ResetCamera();
        }
    }
}
