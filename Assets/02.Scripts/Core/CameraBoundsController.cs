using UnityEngine;
using Cinemachine;

public class CameraBoundsController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private void LateUpdate()
    {
        if (virtualCamera != null)
        {
            Vector3 pos = virtualCamera.transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            virtualCamera.transform.position = pos;
        }
    }
}