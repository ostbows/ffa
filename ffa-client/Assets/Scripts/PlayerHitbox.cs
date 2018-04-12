using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public SphereCollider[] hitboxes;
    public PlayerMotor playerMotor;

    void OnTriggerEnter(Collider other)
    {
        if (!isServer || other.transform.root == transform)
        {
            return;
        }

        Vector3 direction = other.transform.TransformDirection(Vector3.forward);
        RpcHitDetected(direction);

        other.enabled = false;
    }

    [ClientRpc]
    void RpcHitDetected(Vector3 direction)
    {
        if (playerMotor != null)
        {
            playerMotor.Knockback(direction);
        }
    }

    void EnableHitbox(int hitbox)
    {
        hitboxes[hitbox].enabled = true;
    }

    void DisableHitbox(int hitbox)
    {
        hitboxes[hitbox].enabled = false;
    }
}
