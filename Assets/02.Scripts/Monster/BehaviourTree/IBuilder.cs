using System;

namespace FoxHill.Monster.AI
{
    public interface IBuilder
    {
        IBuilder Selector();
        IBuilder Parallel(int successCount);
        IBuilder Sequence();
        IBuilder Execution(Func<Result> execute);
        IBuilder Decorator(Func<bool> condition);
        IBuilder Seek(float radius, float angle, int targetmask, float maxDistance, float stoppingDistance, float moveSpeed);
        IBuilder FinishCurrentComposite();
        IBuilder Build();

        MonsterBehaviourTree GetResult();
    }
}
