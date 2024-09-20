using System;

namespace FoxHill.Core
{
    public interface IPoolable
    {
        event Action<IPoolable> OnRelease;
    }
}
