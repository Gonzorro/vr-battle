using Fusion;
using UnityEngine;

public class CameraBillboard : NetworkBehaviour
{
    private Canvas worldCanvas;
    private Transform camTransform;

    private bool isReady;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        worldCanvas = GetComponent<Canvas>();
        camTransform = Camera.main.transform;
        if (worldCanvas && camTransform)
        {
            worldCanvas.worldCamera = camTransform.GetComponent<Camera>();
            isReady = true;
        }
    }

    private void LateUpdate()
    {
        if (!isReady) return;

        Vector3 targetPos = camTransform.position;
        Vector3 lookPos = new(targetPos.x, transform.position.y, targetPos.z);
        transform.LookAt(lookPos);
    }
}
