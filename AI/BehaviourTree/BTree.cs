namespace ReiBrary.AI.BehaviourTree
{
    public class BTree
    {
        public Node Root;

        public NodeState Tick() => Root.Tick();
    }
}
