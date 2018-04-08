using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public GameObject hitboxPrefab;
    public Transform[] spawnpoints;
    public string playerNetId;

    void Start()
    {
        if (!isLocalPlayer && !isServer)
        {
            Destroy(this);
            return;
        }

        playerNetId = gameObject.GetComponent<NetworkIdentity>().netId.ToString();
    }

    [Command]
    public void CmdSpawnHitbox(int index, float velocity, float duration)
    {
        var hitbox = (GameObject)Instantiate(
            hitboxPrefab,
            spawnpoints[index].position,
            spawnpoints[index].rotation);

        hitbox.GetComponent<Rigidbody>().velocity = hitbox.transform.forward * velocity;
        hitbox.GetComponent<Hitbox>().spawnerNetId = playerNetId;

        NetworkServer.Spawn(hitbox);

        Destroy(hitbox, duration);
    }
}
