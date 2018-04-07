using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;   
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    public void SetOffset()
    {
        offset = transform.position - player.transform.position;
    }

    public void ResetCamera()
    {
        transform.position = startPosition;
    }
}
