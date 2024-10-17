using FoxHill.Core.Debuff;
using System.Collections;
using UnityEngine;

namespace FoxHill.Tower
{
    public abstract class DebuffTowerControllerBase : TowerControllerBase
    {
        [SerializeField] protected TowerDebuffControllerBase _debuffController;
        [SerializeField] protected DebuffType _debuffType;

        protected override void Awake()
        {
            base.Awake();

            _debuffController ??= GetComponentInChildren<TowerDebuffControllerBase>();

            if (_stat != null)
            {
                _debuffController.Initialize(_stat.AttackRange, _debuffType);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override IEnumerator PerformTowerFunction()
        {
            yield return null;
        }
    }
}