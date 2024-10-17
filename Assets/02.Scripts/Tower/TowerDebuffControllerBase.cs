using FoxHill.Core.Debuff;
using FoxHill.Core.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Tower
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class TowerDebuffControllerBase : MonoBehaviour
    {
        protected CircleCollider2D _debuffTrigger;
        protected int _targetLayer;
        protected DebuffType _debuffType;
        public void Initialize(float debuffRange, DebuffType debuffType)
        {
            _debuffTrigger = GetComponent<CircleCollider2D>();
            _targetLayer = LayerRepository.LAYER_PATH_FOLLOW_MONSTER;

            _debuffTrigger.radius = debuffRange;
            _debuffType = debuffType;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.layer ==  _targetLayer)
            {
                if(collision.TryGetComponent<IDebuffable>(out var debuffable) == true)
                {
                    debuffable.ApplyDebuff(_debuffType);
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == _targetLayer)
            {
                if (collision.TryGetComponent<IDebuffable>(out var debuffable) == true)
                {
                    debuffable.RemoveDebuff(_debuffType);
                }
            }
        }
    }
}