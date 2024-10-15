using System.Collections;

namespace FoxHill.Monster.FSM
{
    public interface IState
    {
        State monsterState { get; }
        IEnumerator OnEnter(params object[] parameters);
        State OnUpdate();
        IEnumerator OnExit();
    }
}
