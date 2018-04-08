using UnityEngine;
using UnityEngine.Networking;

public class Hitbox : NetworkBehaviour
{
    [SyncVar]
    public string spawnerNetId;

    void OnTriggerEnter(Collider collision)
    {
        var target = collision.gameObject;
        var targetNetId = target.GetComponent<NetworkIdentity>().netId.ToString();

        if (targetNetId == spawnerNetId) return;

        var targetPlayerMotor = target.GetComponent<PlayerMotor>();

        if (targetPlayerMotor != null)
        {
            targetPlayerMotor.Knockback(transform.TransformDirection(Vector3.forward));
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
