using UnityEngine;

namespace FoxHill.Core.Utils
{
    public class FreezeRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}