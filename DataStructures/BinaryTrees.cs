#nullable disable
using System;
using System.Collections.Generic;
using System.Text;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>Node shared by the binary search tree, AVL tree and red-black tree.</summary>
    public sealed class BinaryTreeNode<TKey, TValue>
    {
        public BinaryTreeNode(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Height = 1;
            IsRed = true;
        }

        public TKey Key { get; internal set; }
        public TValue Value { get; internal set; }
        public BinaryTreeNode<TKey, TValue> Left { get; internal set; }
        public BinaryTreeNode<TKey, TValue> Right { get; internal set; }
        /// <summary>Cached subtree height (used by the AVL tree).</summary>
        public int Height { get; internal set; }
        /// <summary>Node colour (used by the red-black tree).</summary>
        public bool IsRed { get; internal set; }
    }

    /// <summary>
    /// Binary tree operations shared by every binary search tree variant:
    /// lookup with comparison counting, the four traversals, range queries and drawing.
    /// </summary>
    public abstract class BinaryTreeBase<TKey, TValue> where TKey : IComparable<TKey>
    {
        protected BinaryTreeNode<TKey, TValue> root;

        public BinaryTreeNode<TKey, TValue> Root => root;
        public int Count { get; protected set; }
        /// <summary>Number of rotations performed while inserting (0 for the plain BST).</summary>
        public int Rotations { get; protected set; }

        public abstract string Name { get; }

        /// <summary>Inserts a key, or replaces the value when the key already exists.</summary>
        public abstract void Insert(TKey key, TValue value);

        public void Clear()
        {
            root = null;
            Count = 0;
            Rotations = 0;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value, out _);
        }

        /// <summary>Iterative search that also reports how many key comparisons were needed: O(height).</summary>
        public bool TryGetValue(TKey key, out TValue value, out int comparisons)
        {
            comparisons = 0;
            var node = root;
            while (node != null)
            {
                comparisons++;
                int c = key.CompareTo(node.Key);
                if (c == 0) { value = node.Value; return true; }
                node = c < 0 ? node.Left : node.Right;
            }
            value = default(TValue);
            return false;
        }

        public bool Contains(TKey key) => TryGetValue(key, out _);

        /// <summary>Number of levels (empty tree = 0, single node = 1).</summary>
        public int Height() => HeightOf(root);

        private static int HeightOf(BinaryTreeNode<TKey, TValue> n)
        {
            if (n == null) return 0;
            return 1 + Math.Max(HeightOf(n.Left), HeightOf(n.Right));
        }

        public TKey MinKey()
        {
            if (root == null) throw new InvalidOperationException("The tree is empty.");
            var n = root;
            while (n.Left != null) n = n.Left;
            return n.Key;
        }

        public TKey MaxKey()
        {
            if (root == null) throw new InvalidOperationException("The tree is empty.");
            var n = root;
            while (n.Right != null) n = n.Right;
            return n.Key;
        }

        /// <summary>Left, node, right: visits keys in ascending order.</summary>
        public List<KeyValuePair<TKey, TValue>> InOrder()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(Count);
            var stack = new Stack<BinaryTreeNode<TKey, TValue>>();
            var n = root;
            while (n != null || stack.Count > 0)
            {
                while (n != null) { stack.Push(n); n = n.Left; }
                n = stack.Pop();
                result.Add(new KeyValuePair<TKey, TValue>(n.Key, n.Value));
                n = n.Right;
            }
            return result;
        }

        /// <summary>Node, left, right: the order needed to copy the tree's shape.</summary>
        public List<KeyValuePair<TKey, TValue>> PreOrder()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(Count);
            if (root == null) return result;
            var stack = new Stack<BinaryTreeNode<TKey, TValue>>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var n = stack.Pop();
                result.Add(new KeyValuePair<TKey, TValue>(n.Key, n.Value));
                if (n.Right != null) stack.Push(n.Right);
                if (n.Left != null) stack.Push(n.Left);
            }
            return result;
        }

        /// <summary>Left, right, node: children before parents (the order used to delete a tree).</summary>
        public List<KeyValuePair<TKey, TValue>> PostOrder()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(Count);
            if (root == null) return result;
            var stack = new Stack<BinaryTreeNode<TKey, TValue>>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var n = stack.Pop();
                result.Add(new KeyValuePair<TKey, TValue>(n.Key, n.Value));
                if (n.Left != null) stack.Push(n.Left);
                if (n.Right != null) stack.Push(n.Right);
            }
            result.Reverse();
            return result;
        }

        /// <summary>Breadth-first, level by level, using a queue.</summary>
        public List<KeyValuePair<TKey, TValue>> LevelOrder()
        {
            var result = new List<KeyValuePair<TKey, TValue>>(Count);
            if (root == null) return result;
            var queue = new Queue<BinaryTreeNode<TKey, TValue>>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                result.Add(new KeyValuePair<TKey, TValue>(n.Key, n.Value));
                if (n.Left != null) queue.Enqueue(n.Left);
                if (n.Right != null) queue.Enqueue(n.Right);
            }
            return result;
        }

        /// <summary>
        /// All values with low &lt;= key &lt;= high, in key order. Subtrees that cannot hold a key in the
        /// range are skipped, so a balanced tree visits O(log n + k) nodes instead of all n.
        /// </summary>
        public List<TValue> Range(TKey low, TKey high, out int nodesVisited)
        {
            var result = new List<TValue>();
            int visited = 0;
            var stack = new Stack<BinaryTreeNode<TKey, TValue>>();
            var n = root;
            while (n != null || stack.Count > 0)
            {
                while (n != null)
                {
                    visited++;
                    stack.Push(n);
                    n = low.CompareTo(n.Key) < 0 ? n.Left : null;
                }
                n = stack.Pop();
                bool aboveLow = low.CompareTo(n.Key) <= 0;
                bool belowHigh = high.CompareTo(n.Key) >= 0;
                if (aboveLow && belowHigh) result.Add(n.Value);
                n = high.CompareTo(n.Key) > 0 ? n.Right : null;
            }
            nodesVisited = visited;
            return result;
        }

        public List<TValue> Range(TKey low, TKey high) => Range(low, high, out _);

        /// <summary>True when every node's key is greater than its left subtree and smaller than its right.</summary>
        public bool IsOrdered()
        {
            var keys = InOrder();
            for (int i = 1; i < keys.Count; i++)
                if (keys[i - 1].Key.CompareTo(keys[i].Key) >= 0) return false;
            return true;
        }

        /// <summary>Draws the tree top-down; the right child is listed first, then the left child.</summary>
        public string Render(Func<BinaryTreeNode<TKey, TValue>, string> label, int maxNodes = 200)
        {
            if (root == null) return "(empty tree)";
            var sb = new StringBuilder();
            int printed = 0;
            sb.AppendLine(label(root));
            RenderChildren(root, "", sb, label, ref printed, maxNodes);
            if (printed >= maxNodes) sb.AppendLine("... (output truncated)");
            return sb.ToString();
        }

        private static void RenderChildren(BinaryTreeNode<TKey, TValue> n, string indent, StringBuilder sb,
            Func<BinaryTreeNode<TKey, TValue>, string> label, ref int printed, int maxNodes)
        {
            if (n.Left == null && n.Right == null) return;
            RenderChild(n.Right, "R", indent, n.Left == null, sb, label, ref printed, maxNodes);
            RenderChild(n.Left, "L", indent, true, sb, label, ref printed, maxNodes);
        }

        private static void RenderChild(BinaryTreeNode<TKey, TValue> child, string side, string indent, bool last,
            StringBuilder sb, Func<BinaryTreeNode<TKey, TValue>, string> label, ref int printed, int maxNodes)
        {
            if (child == null || printed >= maxNodes) return;
            printed++;
            sb.Append(indent).Append(last ? "└─" : "├─").Append(side).Append(": ").AppendLine(label(child));
            RenderChildren(child, indent + (last ? "     " : "│    "), sb, label, ref printed, maxNodes);
        }
    }

    /// <summary>
    /// Plain (unbalanced) binary search tree. Average O(log n), but keys that arrive in sorted order
    /// turn it into a chain with O(n) height, which is what the AVL and red-black trees prevent.
    /// </summary>
    public class BinarySearchTree<TKey, TValue> : BinaryTreeBase<TKey, TValue> where TKey : IComparable<TKey>
    {
        public override string Name => "Binary Search Tree (unbalanced)";

        public override void Insert(TKey key, TValue value)
        {
            if (root == null)
            {
                root = new BinaryTreeNode<TKey, TValue>(key, value);
                Count++;
                return;
            }
            var n = root;
            while (true)
            {
                int c = key.CompareTo(n.Key);
                if (c == 0) { n.Value = value; return; }
                if (c < 0)
                {
                    if (n.Left == null) { n.Left = new BinaryTreeNode<TKey, TValue>(key, value); Count++; return; }
                    n = n.Left;
                }
                else
                {
                    if (n.Right == null) { n.Right = new BinaryTreeNode<TKey, TValue>(key, value); Count++; return; }
                    n = n.Right;
                }
            }
        }

        /// <summary>Removes a key (Hibbard deletion: a node with two children takes its in-order successor).</summary>
        public bool Remove(TKey key)
        {
            BinaryTreeNode<TKey, TValue> parent = null, n = root;
            while (n != null)
            {
                int c = key.CompareTo(n.Key);
                if (c == 0) break;
                parent = n;
                n = c < 0 ? n.Left : n.Right;
            }
            if (n == null) return false;

            if (n.Left != null && n.Right != null)
            {
                var succParent = n;
                var succ = n.Right;
                while (succ.Left != null) { succParent = succ; succ = succ.Left; }
                n.Key = succ.Key;
                n.Value = succ.Value;
                parent = succParent;
                n = succ;
            }

            var child = n.Left ?? n.Right;
            if (parent == null) root = child;
            else if (parent.Left == n) parent.Left = child;
            else parent.Right = child;
            Count--;
            return true;
        }
    }

    /// <summary>
    /// AVL tree: a BST where the heights of the two subtrees of every node differ by at most one.
    /// After each insert the four cases (LL, RR, LR, RL) are fixed with single or double rotations,
    /// so the height is always below 1.44 log2(n + 2) and every lookup is O(log n).
    /// </summary>
    public class AvlTree<TKey, TValue> : BinaryTreeBase<TKey, TValue> where TKey : IComparable<TKey>
    {
        public override string Name => "AVL Tree";

        public override void Insert(TKey key, TValue value)
        {
            root = Insert(root, key, value);
        }

        private BinaryTreeNode<TKey, TValue> Insert(BinaryTreeNode<TKey, TValue> n, TKey key, TValue value)
        {
            if (n == null)
            {
                Count++;
                return new BinaryTreeNode<TKey, TValue>(key, value);
            }
            int c = key.CompareTo(n.Key);
            if (c == 0) { n.Value = value; return n; }
            if (c < 0) n.Left = Insert(n.Left, key, value);
            else n.Right = Insert(n.Right, key, value);
            return Rebalance(n);
        }

        private BinaryTreeNode<TKey, TValue> Rebalance(BinaryTreeNode<TKey, TValue> n)
        {
            Update(n);
            int balance = BalanceFactor(n);
            if (balance > 1)
            {
                if (BalanceFactor(n.Left) < 0) n.Left = RotateLeft(n.Left);   // Left-Right case
                return RotateRight(n);                                         // Left-Left case
            }
            if (balance < -1)
            {
                if (BalanceFactor(n.Right) > 0) n.Right = RotateRight(n.Right); // Right-Left case
                return RotateLeft(n);                                           // Right-Right case
            }
            return n;
        }

        public static int H(BinaryTreeNode<TKey, TValue> n) => n == null ? 0 : n.Height;
        public static int BalanceFactor(BinaryTreeNode<TKey, TValue> n) => n == null ? 0 : H(n.Left) - H(n.Right);

        private static void Update(BinaryTreeNode<TKey, TValue> n)
        {
            n.Height = 1 + Math.Max(H(n.Left), H(n.Right));
        }

        private BinaryTreeNode<TKey, TValue> RotateRight(BinaryTreeNode<TKey, TValue> y)
        {
            var x = y.Left;
            y.Left = x.Right;
            x.Right = y;
            Update(y);
            Update(x);
            Rotations++;
            return x;
        }

        private BinaryTreeNode<TKey, TValue> RotateLeft(BinaryTreeNode<TKey, TValue> x)
        {
            var y = x.Right;
            x.Right = y.Left;
            y.Left = x;
            Update(x);
            Update(y);
            Rotations++;
            return y;
        }

        /// <summary>True when every node has a balance factor of -1, 0 or +1 and its cached height is correct.</summary>
        public bool IsBalanced() => CheckBalanced(root) >= 0;

        private static int CheckBalanced(BinaryTreeNode<TKey, TValue> n)
        {
            if (n == null) return 0;
            int l = CheckBalanced(n.Left), r = CheckBalanced(n.Right);
            if (l < 0 || r < 0 || Math.Abs(l - r) > 1) return -1;
            int h = 1 + Math.Max(l, r);
            return h == n.Height ? h : -1;
        }
    }

    /// <summary>
    /// Red-black tree (left-leaning variant). Rules: the root is black, a red node never has a red child,
    /// red links lean left, and every path from the root to an empty link has the same number of black nodes.
    /// This keeps the height at most 2 log2(n + 1) while needing fewer rotations than an AVL tree on inserts.
    /// </summary>
    public class RedBlackTree<TKey, TValue> : BinaryTreeBase<TKey, TValue> where TKey : IComparable<TKey>
    {
        public override string Name => "Red-Black Tree";

        public override void Insert(TKey key, TValue value)
        {
            root = Insert(root, key, value);
            root.IsRed = false;
        }

        private BinaryTreeNode<TKey, TValue> Insert(BinaryTreeNode<TKey, TValue> h, TKey key, TValue value)
        {
            if (h == null)
            {
                Count++;
                return new BinaryTreeNode<TKey, TValue>(key, value) { IsRed = true };
            }
            int c = key.CompareTo(h.Key);
            if (c < 0) h.Left = Insert(h.Left, key, value);
            else if (c > 0) h.Right = Insert(h.Right, key, value);
            else h.Value = value;

            if (IsRed(h.Right) && !IsRed(h.Left)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColours(h);
            return h;
        }

        private static bool IsRed(BinaryTreeNode<TKey, TValue> n) => n != null && n.IsRed;

        private BinaryTreeNode<TKey, TValue> RotateLeft(BinaryTreeNode<TKey, TValue> h)
        {
            var x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            Rotations++;
            return x;
        }

        private BinaryTreeNode<TKey, TValue> RotateRight(BinaryTreeNode<TKey, TValue> h)
        {
            var x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            x.IsRed = h.IsRed;
            h.IsRed = true;
            Rotations++;
            return x;
        }

        private static void FlipColours(BinaryTreeNode<TKey, TValue> h)
        {
            h.IsRed = true;
            h.Left.IsRed = false;
            h.Right.IsRed = false;
        }

        /// <summary>Number of black nodes on any path from the root to an empty link.</summary>
        public int BlackHeight()
        {
            int count = 0;
            for (var n = root; n != null; n = n.Left)
                if (!n.IsRed) count++;
            return count;
        }

        /// <summary>Checks every red-black rule plus BST ordering.</summary>
        public bool IsValid()
        {
            if (root == null) return true;
            if (root.IsRed || !IsOrdered()) return false;
            return CheckNode(root, BlackHeight(), 0);
        }

        private static bool CheckNode(BinaryTreeNode<TKey, TValue> n, int expectedBlack, int black)
        {
            if (n == null) return black == expectedBlack;
            if (IsRed(n.Right)) return false;                      // red links must lean left
            if (n.IsRed && IsRed(n.Left)) return false;            // no two reds in a row
            if (!n.IsRed) black++;
            return CheckNode(n.Left, expectedBlack, black) && CheckNode(n.Right, expectedBlack, black);
        }
    }
}
