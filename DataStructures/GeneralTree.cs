#nullable disable
using System;
using System.Collections.Generic;
using System.Text;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Basic (general) tree: every node may have any number of children.
    /// Used to group service requests as Root > Category > Status > Request.
    /// </summary>
    public class GeneralTreeNode<T>
    {
        private readonly List<GeneralTreeNode<T>> children = new List<GeneralTreeNode<T>>();

        public GeneralTreeNode(T value)
        {
            Value = value;
        }

        public T Value { get; }
        public GeneralTreeNode<T> Parent { get; private set; }
        public IReadOnlyList<GeneralTreeNode<T>> Children => children;
        public bool IsLeaf => children.Count == 0;

        public GeneralTreeNode<T> AddChild(T value)
        {
            var child = new GeneralTreeNode<T>(value) { Parent = this };
            children.Add(child);
            return child;
        }

        /// <summary>Returns the direct child matching <paramref name="match"/>, adding it when missing.</summary>
        public GeneralTreeNode<T> GetOrAddChild(T value, Func<T, T, bool> match)
        {
            foreach (var child in children)
                if (match(child.Value, value)) return child;
            return AddChild(value);
        }

        /// <summary>Number of nodes in this subtree (including this node).</summary>
        public int Count()
        {
            int total = 1;
            foreach (var child in children) total += child.Count();
            return total;
        }

        /// <summary>Number of levels in this subtree (a single node has height 1).</summary>
        public int Height()
        {
            int tallest = 0;
            foreach (var child in children) tallest = Math.Max(tallest, child.Height());
            return tallest + 1;
        }

        public int Depth()
        {
            int depth = 0;
            for (var n = Parent; n != null; n = n.Parent) depth++;
            return depth;
        }

        /// <summary>Depth-first pre-order walk (parent before children).</summary>
        public IEnumerable<GeneralTreeNode<T>> PreOrder()
        {
            var stack = new Stack<GeneralTreeNode<T>>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                yield return node;
                for (int i = node.children.Count - 1; i >= 0; i--) stack.Push(node.children[i]);
            }
        }

        /// <summary>Breadth-first walk (level by level).</summary>
        public IEnumerable<GeneralTreeNode<T>> LevelOrder()
        {
            var queue = new Queue<GeneralTreeNode<T>>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node;
                foreach (var child in node.children) queue.Enqueue(child);
            }
        }

        public List<GeneralTreeNode<T>> Leaves()
        {
            var leaves = new List<GeneralTreeNode<T>>();
            foreach (var node in PreOrder())
                if (node.IsLeaf) leaves.Add(node);
            return leaves;
        }

        /// <summary>Draws the tree as indented text with box-drawing connectors.</summary>
        public string Render(Func<T, string> label)
        {
            var sb = new StringBuilder();
            sb.AppendLine(label(Value));
            for (int i = 0; i < children.Count; i++)
                children[i].Render(sb, "", i == children.Count - 1, label);
            return sb.ToString();
        }

        private void Render(StringBuilder sb, string indent, bool last, Func<T, string> label)
        {
            sb.Append(indent).Append(last ? "└── " : "├── ").AppendLine(label(Value));
            string childIndent = indent + (last ? "    " : "│   ");
            for (int i = 0; i < children.Count; i++)
                children[i].Render(sb, childIndent, i == children.Count - 1, label);
        }
    }
}
