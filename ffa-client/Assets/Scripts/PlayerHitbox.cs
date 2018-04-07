using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public GameObject[] hitboxes;

    Dictionary<string, SphereCollider> colliders = new Dictionary<string, SphereCollider>();

    void Start()
    {
        if (isServer)
        {
            CacheColliders();
        }
    }

    [Server]
    void CacheColliders()
    {
        for (int i = 0; i < hitboxes.Length; i++)
        {
            string name = hitboxes[i].name;
            SphereCollider collider = hitboxes[i].GetComponent<SphereCollider>();

            colliders.Add(name, collider);
        }
    }

    [Command]
    public void CmdToggleHitbox(string hitbox, float duration)
    {
        if (colliders.ContainsKey(hitbox))
        {
            colliders[hitbox].enabled = true;

            StartCoroutine(DisableHitbox(hitbox, duration));
        }
    }

    IEnumerator DisableHitbox(string hitbox, float duration)
    {
        yield return new WaitForSeconds(duration);

        colliders[hitbox].enabled = false;
    }
}
