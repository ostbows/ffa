using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public GameObject[] hitboxes;

    Dictionary<string, BoxCollider> colliders;

    public override void OnStartLocalPlayer()
    {
        colliders = new Dictionary<string, BoxCollider>();

        for (int i = 0; i < hitboxes.Length; i++)
        {
            string name = hitboxes[i].name;
            BoxCollider collider = hitboxes[i].GetComponent<BoxCollider>();

            colliders.Add(name, collider);
        }
    }

    void UnarmedAtkForwardStart() // Event
    {
        if (!isLocalPlayer) return;

        colliders["LowerLegRHitbox"].enabled = true;
    }
    void UnarmedAtkForwardEnd() // Event
    {
        if (!isLocalPlayer) return;

        colliders["LowerLegRHitbox"].enabled = false;
    }
}
