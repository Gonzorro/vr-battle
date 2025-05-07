using UnityEngine;

public class NetworkPistolBullet : NetworkBulletBase
{
    protected override void Update() => base.Update();

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;

        int otherLayer = 1 << collision.gameObject.layer;

        if ((environmentLayer | playerLayer & otherLayer) != 0)
            Runner.Despawn(Object);
    }
}
