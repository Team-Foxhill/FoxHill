using UnityEngine;

namespace FoxHill.Map
{
    public class GameObjectDestroyer : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(gameObject);
        }
    }
}