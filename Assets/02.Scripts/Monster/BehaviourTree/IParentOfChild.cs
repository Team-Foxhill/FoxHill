namespace FoxHill.Monster.AI
{
    public interface IParentOfChild : IParent
    {
        Node child { get; set; }
    }
}
