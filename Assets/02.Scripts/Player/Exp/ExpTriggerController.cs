using FoxHill.Core.Utils;
using UnityEngine;

namespace FoxHill.Player.Exp
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExpTriggerController : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.layer == LayerRepository.LAYER_EXP)
            {
                if(collision.gameObject.TryGetComponent<Core.Exp.Exp>(out var exp) == true)
                {
                    exp.Obtain(transform);
                }
            }
        }
    }
}