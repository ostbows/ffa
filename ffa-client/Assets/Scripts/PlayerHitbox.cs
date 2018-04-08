using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHitbox : NetworkBehaviour
{
    public GameObject hitboxPrefab;
    public Transform[] spawnpoints;
    public NetworkIdentity networkIdentity;

    [SerializeField] string playerNetId;

    void Start()
    {
        if (!isLocalPlayer && !isServer)
        {
            Destroy(this);
            return;
        }

        playerNetId = networkIdentity.netId.ToString();
    }

    [Command]
    public void CmdSpawnHitbox(int index, float velocity, float delay, float duration)
    {
        StartCoroutine(SpawnHitbox(index, velocity, delay, duration));
    }

    [Server]
    IEnumerator SpawnHitbox(int index, float velocity, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

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
