using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.DataStructures;
using Xunit;

namespace MunicipalServicesApp.Tests
{
    public class BinaryTreeTests
    {
        private static IEnumerable<BinaryTreeBase<int, string>> AllTrees()
        {
            yield return new BinarySearchTree<int, string>();
            yield return new AvlTree<int, string>();
            yield return new RedBlackTree<int, string>();
        }

        [Fact]
        public void EveryTreeStoresFindsAndOrdersRandomKeys()
        {
            var rng = new Random(7);
            var keys = Enumerable.Range(0, 500).Select(_ => rng.Next(10000)).Distinct().ToList();
            foreach (var tree in AllTrees())
            {
                foreach (var k in keys) tree.Insert(k, "v" + k);
                Assert.Equal(keys.Count, tree.Count);
                Assert.True(tree.IsOrdered(), tree.Name);
                Assert.Equal(keys.OrderBy(k => k), tree.InOrder().Select(kv => kv.Key));
                foreach (var k in keys)
                {
                    Assert.True(tree.TryGetValue(k, out var v));
                    Assert.Equal("v" + k, v);
                }
                Assert.False(tree.Contains(-1));
            }
        }

        [Fact]
        public void DuplicateKeyReplacesValueWithoutGrowing()
        {
            foreach (var tree in AllTrees())
            {
                tree.Insert(5, "a");
                tree.Insert(5, "b");
                Assert.Equal(1, tree.Count);
                tree.TryGetValue(5, out var v);
                Assert.Equal("b", v);
            }
        }

        [Fact]
        public void TraversalsFollowTheirDefinitions()
        {
            var bst = new BinarySearchTree<int, string>();
            foreach (var k in new[] { 50, 30, 70, 20, 40, 60, 80 }) bst.Insert(k, "");
            Assert.Equal(new[] { 20, 30, 40, 50, 60, 70, 80 }, bst.InOrder().Select(kv => kv.Key));
            Assert.Equal(new[] { 50, 30, 20, 40, 70, 60, 80 }, bst.PreOrder().Select(kv => kv.Key));
            Assert.Equal(new[] { 20, 40, 30, 60, 80, 70, 50 }, bst.PostOrder().Select(kv => kv.Key));
            Assert.Equal(new[] { 50, 30, 70, 20, 40, 60, 80 }, bst.LevelOrder().Select(kv => kv.Key));
            Assert.Equal(3, bst.Height());
            Assert.Equal(20, bst.MinKey());
            Assert.Equal(80, bst.MaxKey());
        }

        [Fact]
        public void SortedInsertsDegradeTheBstButNotTheBalancedTrees()
        {
            var bst = new BinarySearchTree<int, string>();
            var avl = new AvlTree<int, string>();
            var rb = new RedBlackTree<int, string>();
            for (int i = 1; i <= 1023; i++) { bst.Insert(i, ""); avl.Insert(i, ""); rb.Insert(i, ""); }
            Assert.Equal(1023, bst.Height());
            Assert.Equal(10, avl.Height());
            Assert.True(rb.Height() <= 2 * Math.Log(1024, 2));
            Assert.True(avl.IsBalanced());
            Assert.True(rb.IsValid());
            avl.TryGetValue(1023, out _, out int avlComparisons);
            bst.TryGetValue(1023, out _, out int bstComparisons);
            Assert.True(avlComparisons <= 10);
            Assert.Equal(1023, bstComparisons);
        }

        [Fact]
        public void AvlStaysBalancedForAllFourRotationCases()
        {
            foreach (var order in new[] { new[] { 3, 2, 1 }, new[] { 1, 2, 3 }, new[] { 3, 1, 2 }, new[] { 1, 3, 2 } })
            {
                var avl = new AvlTree<int, string>();
                foreach (var k in order) avl.Insert(k, "");
                Assert.True(avl.IsBalanced());
                Assert.Equal(2, avl.Root.Key);
                Assert.Equal(2, avl.Height());
                Assert.True(avl.Rotations >= 1);
            }
        }

        [Fact]
        public void RedBlackInvariantsHoldOnRandomData()
        {
            var rng = new Random(11);
            var rb = new RedBlackTree<int, string>();
            for (int i = 0; i < 2000; i++)
            {
                rb.Insert(rng.Next(100000), "");
                if (i % 250 == 0) Assert.True(rb.IsValid());
            }
            Assert.True(rb.IsValid());
            Assert.False(rb.Root.IsRed);
        }

        [Fact]
        public void BstRemoveHandlesLeafOneChildAndTwoChildren()
        {
            var bst = new BinarySearchTree<int, string>();
            foreach (var k in new[] { 50, 30, 70, 20, 40, 60, 80, 65 }) bst.Insert(k, "");
            Assert.True(bst.Remove(20));   // leaf
            Assert.True(bst.Remove(60));   // one child
            Assert.True(bst.Remove(50));   // two children (root)
            Assert.False(bst.Remove(999));
            Assert.Equal(new[] { 30, 40, 65, 70, 80 }, bst.InOrder().Select(kv => kv.Key));
            Assert.True(bst.IsOrdered());
        }

        [Fact]
        public void RangeQueryMatchesLinearScanAndPrunes()
        {
            var keys = Enumerable.Range(1, 1000).ToList();
            var avl = new AvlTree<int, int>();
            var rb = new RedBlackTree<int, int>();
            foreach (var k in keys) { avl.Insert(k, k); rb.Insert(k, k); }
            var expected = keys.Where(k => k >= 400 && k <= 409).ToList();
            Assert.Equal(expected, avl.Range(400, 409, out int avlVisited));
            Assert.Equal(expected, rb.Range(400, 409, out int rbVisited));
            Assert.True(avlVisited < 50, "AVL visited " + avlVisited);
            Assert.True(rbVisited < 50, "RB visited " + rbVisited);
            Assert.Empty(avl.Range(2000, 3000, out _));
            Assert.Equal(keys, avl.Range(int.MinValue, int.MaxValue, out _));
        }

        [Fact]
        public void RenderDrawsEveryNode()
        {
            var avl = new AvlTree<int, string>();
            for (int i = 1; i <= 7; i++) avl.Insert(i, "");
            string text = avl.Render(n => n.Key.ToString());
            for (int i = 1; i <= 7; i++) Assert.Contains(i.ToString(), text);
            Assert.StartsWith("4", text);
        }
    }

    public class HeapTests
    {
        [Fact]
        public void PopsInAscendingOrder()
        {
            var rng = new Random(3);
            var values = Enumerable.Range(0, 300).Select(_ => rng.Next(1000)).ToList();
            var heap = new MinHeap<int>((a, b) => a.CompareTo(b));
            foreach (var v in values) { heap.Push(v); Assert.True(heap.IsValid()); }
            Assert.Equal(values.Min(), heap.Peek());
            Assert.Equal(values.OrderBy(v => v), heap.ToSortedList());
            Assert.Equal(values.Count, heap.Count);   // ToSortedList must not consume the heap
            var popped = new List<int>();
            while (heap.TryPop(out int v)) popped.Add(v);
            Assert.Equal(values.OrderBy(v => v), popped);
        }

        [Fact]
        public void HeapifyBuildsAValidHeap()
        {
            var heap = new MinHeap<int>(new[] { 9, 4, 7, 1, 8, 2, 6, 3, 5 }, (a, b) => a.CompareTo(b));
            Assert.True(heap.IsValid());
            Assert.Equal(1, heap.Pop());
            Assert.Equal(2, heap.Pop());
        }

        [Fact]
        public void EmptyHeapThrows()
        {
            var heap = new MinHeap<int>((a, b) => a.CompareTo(b));
            Assert.Throws<InvalidOperationException>(() => heap.Peek());
            Assert.Throws<InvalidOperationException>(() => heap.Pop());
            Assert.False(heap.TryPop(out _));
        }
    }

    public class GraphTests
    {
        private static WeightedGraph<string> Sample()
        {
            var g = new WeightedGraph<string>();
            g.AddEdge("A", "B", 4);
            g.AddEdge("A", "C", 1);
            g.AddEdge("C", "B", 2);
            g.AddEdge("B", "D", 5);
            g.AddEdge("C", "D", 8);
            g.AddEdge("D", "E", 3);
            g.AddVertex("Z");
            return g;
        }

        [Fact]
        public void BreadthFirstReportsHopDistance()
        {
            var bfs = Sample().BreadthFirst("A").ToDictionary(kv => kv.Key, kv => kv.Value);
            Assert.Equal(0, bfs["A"]);
            Assert.Equal(1, bfs["B"]);
            Assert.Equal(1, bfs["C"]);
            Assert.Equal(2, bfs["D"]);
            Assert.Equal(3, bfs["E"]);
            Assert.False(bfs.ContainsKey("Z"));
        }

        [Fact]
        public void DepthFirstVisitsTheWholeComponentOnce()
        {
            var dfs = Sample().DepthFirst("A");
            Assert.Equal("A", dfs[0]);
            Assert.Equal(5, dfs.Count);
            Assert.Equal(dfs.Count, dfs.Distinct().Count());
            Assert.Equal(2, Sample().ConnectedComponents().Count);
        }

        [Fact]
        public void DijkstraFindsShortestPath()
        {
            var g = Sample();
            var path = g.ShortestPath("A", "E", out double distance);
            Assert.Equal(new[] { "A", "C", "B", "D", "E" }, path);
            Assert.Equal(11, distance);
            Assert.Empty(g.ShortestPath("A", "Z", out _));
        }

        [Fact]
        public void PrimAndKruskalAgreeOnRandomGraphs()
        {
            var rng = new Random(5);
            for (int round = 0; round < 20; round++)
            {
                var g = new WeightedGraph<int>();
                int n = 12;
                for (int i = 1; i < n; i++) g.AddEdge(i, rng.Next(i), rng.Next(1, 50));   // spanning tree keeps it connected
                for (int extra = 0; extra < 20; extra++)
                {
                    int a = rng.Next(n), b = rng.Next(n);
                    if (a != b) g.AddEdge(a, b, rng.Next(1, 50));
                }
                var prim = g.PrimMinimumSpanningTree(0);
                var kruskal = g.KruskalMinimumSpanningTree();
                Assert.Equal(n - 1, prim.Count);
                Assert.Equal(n - 1, kruskal.Count);
                Assert.Equal(WeightedGraph<int>.TotalWeight(kruskal), WeightedGraph<int>.TotalWeight(prim));
            }
        }

        [Fact]
        public void DijkstraMatchesFloydWarshall()
        {
            var rng = new Random(9);
            int n = 10;
            var g = new WeightedGraph<int>();
            for (int i = 0; i < n; i++) g.AddVertex(i);
            var d = new double[n, n];
            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) d[i, j] = i == j ? 0 : double.PositiveInfinity;
            for (int e = 0; e < 25; e++)
            {
                int a = rng.Next(n), b = rng.Next(n);
                if (a == b) continue;
                int w = rng.Next(1, 30);
                g.AddEdge(a, b, w);
                d[a, b] = Math.Min(d[a, b], w);
                d[b, a] = d[a, b];
            }
            for (int k = 0; k < n; k++) for (int i = 0; i < n; i++) for (int j = 0; j < n; j++)
                if (d[i, k] + d[k, j] < d[i, j]) d[i, j] = d[i, k] + d[k, j];
            for (int s = 0; s < n; s++)
            {
                var dist = g.ShortestDistances(s);
                for (int t = 0; t < n; t++) Assert.Equal(d[s, t], dist[t]);
            }
        }

        [Fact]
        public void NegativeWeightsAreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WeightedGraph<string>().AddEdge("A", "B", -1));
        }
    }

    public class GeneralTreeTests
    {
        [Fact]
        public void CountsHeightAndWalks()
        {
            var root = new GeneralTreeNode<string>("Root");
            var roads = root.AddChild("Roads");
            root.AddChild("Water").AddChild("W1");
            roads.AddChild("R1");
            roads.AddChild("R2");
            Assert.Equal(6, root.Count());
            Assert.Equal(3, root.Height());
            Assert.Equal(new[] { "Root", "Roads", "R1", "R2", "Water", "W1" }, root.PreOrder().Select(n => n.Value));
            Assert.Equal(new[] { "Root", "Roads", "Water", "R1", "R2", "W1" }, root.LevelOrder().Select(n => n.Value));
            Assert.Equal(3, root.Leaves().Count);
            Assert.Same(roads, root.GetOrAddChild("Roads", (a, b) => a == b));
            Assert.Equal(2, roads.Children[0].Depth());
            Assert.Contains("└── Water", root.Render(v => v));
        }
    }
}
