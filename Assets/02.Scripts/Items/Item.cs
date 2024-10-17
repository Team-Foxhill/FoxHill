using FoxHill.Core.Pause;
using System.Collections;
using UnityEngine;

namespace FoxHill.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Item : MonoBehaviour, IPausable
    {
        public ItemData Info;
        public Sprite Image => _icon.sprite;

        private const float MAX_LIFETIME = 60f;

        private SpriteRenderer _icon;

        private bool _isPaused = false;

        private void Awake()
        {
            _icon = GetComponent<SpriteRenderer>();

            PauseManager.Register(this);
        }

        private void Start()
        {
            StartCoroutine(C_CheckLifeTime());
        }

        private IEnumerator C_CheckLifeTime()
        {
            float lifeTime = 0f;

            while (lifeTime < MAX_LIFETIME)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                lifeTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }

        public void Obtain(Transform playerTransform)
        {
            StartCoroutine(C_Obtain(playerTransform));
        }

        private IEnumerator C_Obtain(Transform playerTransform)
        {
            float moveSpeed = 5f;
            Vector3 direction = playerTransform.position - transform.position;

            while (direction.magnitude > 0.05f)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                direction = playerTransform.position - transform.position;
                moveSpeed += 0.02f;
                yield return null;
            }

            Destroy(gameObject);
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}