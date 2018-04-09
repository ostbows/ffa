using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public SphereCollider[] hitboxes;
    public PlayerMotor playerMotor;

    [Command]
    public void CmdActivateHitbox(int hib)
    {
        hitboxes[hib].enabled = true;
        StartCoroutine(DisableHitbox(hib));
    }
    IEnumerator DisableHitbox(int hib)
    {
        yield return new WaitForSeconds(0.5f);

        hitboxes[hib].enabled = false;
    }

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
}
