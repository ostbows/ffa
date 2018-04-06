using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public GameObject[] hitboxes;

    Dictionary<string, BoxCollider> colliders = new Dictionary<string, BoxCollider>();

    public override void OnStartLocalPlayer()
    {
        CmdGetColliders();
    }

    [Command]
    void CmdGetColliders()
    {
        for (int i = 0; i < hitboxes.Length; i++)
        {
            string name = hitboxes[i].name;
            BoxCollider collider = hitboxes[i].GetComponent<BoxCollider>();

            colliders.Add(name, collider);
        }
    }

    [Command]
    void CmdEnableHitbox(string hitbox) // Event
    {
        if (!isServer) return;

        colliders[hitbox].enabled = true;
    }
    [Command]
    void CmdDisableHitbox(string hitbox) // Event
    {
        if (!isServer) return;

        colliders[hitbox].enabled = false;
    }
}
