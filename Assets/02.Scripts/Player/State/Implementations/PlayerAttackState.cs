using UnityEngine;

namespace FoxHill.Player.State.Implementations
{
    public class PlayerAttackState : PlayerStateBase
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        protected override PlayerStateBase _state 
        {
            get => throw new System.NotImplementedException(); 
            set => throw new System.NotImplementedException(); 
        } 

        
    }
}
