using System.Collections;
using UnityEngine;

namespace FoxHill.Core.Exp
{
    public class Exp : MonoBehaviour
    {
        public float Amount;


        public void Obtain(Transform playerTransform)
        {
            StartCoroutine(C_Obtain(playerTransform));
        }

        private IEnumerator C_Obtain(Transform playerTransform)
        {
            float moveSpeed = 6f;
            Vector3 direction = playerTransform.position - transform.position;

            while (direction.magnitude > 0.05f)
            {
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                direction = playerTransform.position - transform.position;
                moveSpeed += 0.03f;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}