using UnityEngine;

public class PlayerHitboxOnCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform.root)
        {
            return;
        }

        PlayerMotor playerMotor = other.GetComponent<PlayerMotor>();

        if (playerMotor != null)
        {
            Vector3 attackerDirection = transform.root.TransformDirection(Vector3.forward);
            playerMotor.Move(attackerDirection * 2.0f);
        }
    }
}
