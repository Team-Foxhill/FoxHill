namespace FoxHill.Monster.AI
{
    public interface IBuilder
    {
        IBuilder Selector();
        IBuilder Parallel();
        IBuilder Sequence();
        IBuilder Execution();
        IBuilder Decorator();
        IBuilder Seek();
        IBuilder FinishCurrentComposite();
        MonsterBehaviourTree build();
    }
}
