using UnityEngine;

namespace FoxHill.Pet
{
    public class Pet : MonoBehaviour
    {
        [SerializeField] private Transform _followTarget;
        private float _range = 3f;
        private float _speed = 1f;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if(_followTarget == null)
            {
                return;
            }

            Vector3 direction = (_followTarget.position - transform.position).normalized;
            _renderer.flipX = (direction.x < 0f);

            if ((transform.position - _followTarget.position).magnitude > _range)
            {
                transform.Translate(direction * _speed * Time.deltaTime);
            }
        }
    }
}
