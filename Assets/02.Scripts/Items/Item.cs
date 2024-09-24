using System.Collections;
using UnityEngine;

namespace FoxHill.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Item : MonoBehaviour
    {
        public ItemData Info;
        public Sprite Image => _icon.sprite;

        private SpriteRenderer _icon;

        private void Awake()
        {
            _icon = GetComponent<SpriteRenderer>();
        }

        public void Obtain(Transform playerTransform)
        {
            StartCoroutine(C_Obtain(playerTransform));
        }

        private IEnumerator C_Obtain(Transform playerTransform)
        {
            float moveSpeed = 4f;
            Vector3 direction = playerTransform.position - transform.position;

            while (direction.magnitude > 0.05f)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                direction = playerTransform.position - transform.position;
                moveSpeed += 0.01f;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}