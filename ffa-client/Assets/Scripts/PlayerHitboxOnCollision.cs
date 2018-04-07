using UnityEngine;

public class PlayerHitboxOnCollision : MonoBehaviour
{
    Transform player;
    SphereCollider hitbox;

    void Awake()
    {
        player = transform.root;
        hitbox = this.GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform != player)
        {
            PlayerMotor targetPlayerMotor = other.GetComponent<PlayerMotor>();

            if (targetPlayerMotor != null)
            {
                hitbox.enabled = false;

                Vector3 playerDirection = transform.root.TransformDirection(Vector3.forward);
                targetPlayerMotor.Move(playerDirection);
            }
        }
    }
}
