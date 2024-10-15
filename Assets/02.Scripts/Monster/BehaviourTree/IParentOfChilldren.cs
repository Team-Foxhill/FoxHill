using System.Collections.Generic;

namespace FoxHill.Monster.AI
{
    public interface IParentOfChilldren : IParent
    {
        List<Node> children { get; set; }
    }
}
