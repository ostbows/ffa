using UnityEngine;

public class PlayerHitboxOnCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Todo: Compare network ID
        if (other.gameObject.name == transform.root.name)
        {
            return;
        }
        // Todo: Destroy on server
        Destroy(other.gameObject);
    }
}
