using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public SphereCollider[] hitboxes;
    public PlayerMotor playerMotor;

    [SyncVar(hook = "OnHitboxEnable")]
    public int hitbox = -1;

    [Command]
    public void CmdActivateHitbox(int hib)
    {
        hitbox = hib;
    }

    void OnHitboxEnable(int hitbox)
    {
        if (hitbox == -1) return;
        hitboxes[hitbox].enabled = true;
        StartCoroutine(DisableHitbox(hitbox));
    }
    IEnumerator DisableHitbox(int hib)
    {
        yield return new WaitForSeconds(1.0f);

        if (isServer) hitbox = -1;
        hitboxes[hib].enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform) return;
        other.enabled = false;

        if (playerMotor != null)
        {
            Vector3 otherDirection = other.transform.TransformDirection(Vector3.forward);
            playerMotor.Knockback(otherDirection);
        }
    }
}
