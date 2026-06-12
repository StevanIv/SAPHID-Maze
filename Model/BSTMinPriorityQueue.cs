namespace Model
{

    internal sealed class BSTNode
    {
        public readonly int Priority;
        public readonly int[] Cell;
        public readonly int[] Wall;
        public BSTNode? Left;
        public BSTNode? Right;

        public BSTNode(int priority, int[] cell, int[] wall)
        {
            Priority = priority;
            Cell = cell;
            Wall = wall;
        }
    }


    internal sealed class BSTMinPriorityQueue
    {
        private BSTNode? _root;
        public int Count { get; private set; }

        public void Enqueue(int priority, int[] cell, int[] wall)
        {
            _root = Insert(_root, new BSTNode(priority, cell, wall));
            Count++;
        }

        public (int Priority, int[] Cell, int[] Wall) DequeueMin()
        {
            if (_root == null)
                throw new InvalidOperationException("Priority queue is empty.");

            var min = FindMin(_root);
            _root = DeleteMin(_root);
            Count--;
            return (min.Priority, min.Cell, min.Wall);
        }

        private static BSTNode Insert(BSTNode? node, BSTNode newNode)
        {
            if (node == null) return newNode;

            if (newNode.Priority <= node.Priority)
                node.Left = Insert(node.Left, newNode);
            else
                node.Right = Insert(node.Right, newNode);

            return node;
        }


        private static BSTNode FindMin(BSTNode node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }


        private static BSTNode? DeleteMin(BSTNode node)
        {
            if (node.Left == null) return node.Right;
            node.Left = DeleteMin(node.Left);
            return node;
        }
    }
}
