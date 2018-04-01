using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        transform.position = player.transform.position + offset;
    }

    public void SetOffset()
    {
        offset = transform.position - player.transform.position;
    }
}
