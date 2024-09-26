using System.Collections;
using UnityEngine;

namespace FoxHill.Tower
{
    public abstract class AttackTowerControllerBase : TowerControllerBase
    {
        [SerializeField] protected TowerAttackControllerBase _attackController;


        protected override void Awake()
        {
            base.Awake();

            if (_attackController == null)
            {
                _attackController = GetComponentInChildren<TowerAttackControllerBase>();
            }

            if (_stat != null)
            {
                _attackController.Initialize(_stat.AttackRange, _stat.AttackSpeed, _stat.Power);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override IEnumerator PerformTowerFunction()
        {
            _attackController.StartAttack();
            yield return null;
        }

        public override void Pause()
        {
            base.Pause();
            _attackController.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            _attackController.Resume();
        }
    }
}