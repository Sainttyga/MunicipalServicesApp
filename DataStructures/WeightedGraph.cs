#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.DataStructures
{
    public sealed class GraphEdge<T>
    {
        public GraphEdge(T from, T to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public T From { get; }
        public T To { get; }
        public double Weight { get; }

        public override string ToString() => $"{From} - {To} ({Weight:0.#})";
    }

    /// <summary>
    /// Undirected weighted graph stored as adjacency lists (memory O(V + E)).
    /// Provides breadth-first and depth-first traversal, connected components,
    /// Dijkstra shortest paths and Prim / Kruskal minimum spanning trees.
    /// </summary>
    public class WeightedGraph<T>
    {
        private readonly Dictionary<T, List<GraphEdge<T>>> adjacency = new Dictionary<T, List<GraphEdge<T>>>();
        private readonly List<T> order = new List<T>();

        public int VertexCount => order.Count;
        public int EdgeCount { get; private set; }
        public IReadOnlyList<T> Vertices => order;

        public bool AddVertex(T vertex)
        {
            if (adjacency.ContainsKey(vertex)) return false;
            adjacency[vertex] = new List<GraphEdge<T>>();
            order.Add(vertex);
            return true;
        }

        public bool ContainsVertex(T vertex) => adjacency.ContainsKey(vertex);

        /// <summary>Adds an undirected edge (stored once in each endpoint's list).</summary>
        public void AddEdge(T a, T b, double weight)
        {
            if (weight < 0) throw new ArgumentOutOfRangeException(nameof(weight), "Weights must be non-negative.");
            AddVertex(a);
            AddVertex(b);
            adjacency[a].Add(new GraphEdge<T>(a, b, weight));
            adjacency[b].Add(new GraphEdge<T>(b, a, weight));
            EdgeCount++;
        }

        public IReadOnlyList<GraphEdge<T>> Neighbours(T vertex)
        {
            return adjacency.TryGetValue(vertex, out var list) ? list : (IReadOnlyList<GraphEdge<T>>)new List<GraphEdge<T>>();
        }

        public int Degree(T vertex) => Neighbours(vertex).Count;

        /// <summary>Every undirected edge exactly once.</summary>
        public List<GraphEdge<T>> Edges()
        {
            var index = new Dictionary<T, int>();
            for (int i = 0; i < order.Count; i++) index[order[i]] = i;
            var edges = new List<GraphEdge<T>>();
            foreach (var v in order)
                foreach (var e in adjacency[v])
                    if (index[e.From] < index[e.To]) edges.Add(e);
            return edges;
        }

        /// <summary>Breadth-first search: visits vertices in order of hop distance, using a queue. O(V + E).</summary>
        public List<KeyValuePair<T, int>> BreadthFirst(T start)
        {
            var result = new List<KeyValuePair<T, int>>();
            if (!adjacency.ContainsKey(start)) return result;
            var depth = new Dictionary<T, int> { [start] = 0 };
            var queue = new Queue<T>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                result.Add(new KeyValuePair<T, int>(v, depth[v]));
                foreach (var e in adjacency[v])
                {
                    if (depth.ContainsKey(e.To)) continue;
                    depth[e.To] = depth[v] + 1;
                    queue.Enqueue(e.To);
                }
            }
            return result;
        }

        /// <summary>Depth-first search: follows one branch as far as possible, using a stack. O(V + E).</summary>
        public List<T> DepthFirst(T start)
        {
            var result = new List<T>();
            if (!adjacency.ContainsKey(start)) return result;
            var visited = new HashSet<T>();
            var stack = new Stack<T>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var v = stack.Pop();
                if (!visited.Add(v)) continue;
                result.Add(v);
                var neighbours = adjacency[v];
                for (int i = neighbours.Count - 1; i >= 0; i--)
                    if (!visited.Contains(neighbours[i].To)) stack.Push(neighbours[i].To);
            }
            return result;
        }

        /// <summary>Groups of vertices that can reach each other (found with repeated DFS).</summary>
        public List<List<T>> ConnectedComponents()
        {
            var seen = new HashSet<T>();
            var components = new List<List<T>>();
            foreach (var v in order)
            {
                if (seen.Contains(v)) continue;
                var component = DepthFirst(v);
                foreach (var c in component) seen.Add(c);
                components.Add(component);
            }
            return components;
        }

        /// <summary>Dijkstra with a min-heap priority queue: O((V + E) log V).</summary>
        public Dictionary<T, double> ShortestDistances(T source, out Dictionary<T, T> previous)
        {
            var dist = new Dictionary<T, double>();
            previous = new Dictionary<T, T>();
            if (!adjacency.ContainsKey(source)) return dist;
            foreach (var v in order) dist[v] = double.PositiveInfinity;
            dist[source] = 0;
            var heap = new MinHeap<KeyValuePair<T, double>>((a, b) => a.Value.CompareTo(b.Value));
            heap.Push(new KeyValuePair<T, double>(source, 0));
            var done = new HashSet<T>();
            while (!heap.IsEmpty)
            {
                var current = heap.Pop();
                if (!done.Add(current.Key)) continue;
                foreach (var e in adjacency[current.Key])
                {
                    double candidate = current.Value + e.Weight;
                    if (candidate < dist[e.To])
                    {
                        dist[e.To] = candidate;
                        previous[e.To] = current.Key;
                        heap.Push(new KeyValuePair<T, double>(e.To, candidate));
                    }
                }
            }
            return dist;
        }

        public Dictionary<T, double> ShortestDistances(T source) => ShortestDistances(source, out _);

        /// <summary>The vertices on the shortest path from source to target (empty if unreachable).</summary>
        public List<T> ShortestPath(T source, T target, out double distance)
        {
            var dist = ShortestDistances(source, out var previous);
            var path = new List<T>();
            if (!dist.TryGetValue(target, out distance) || double.IsInfinity(distance)) return path;
            var comparer = EqualityComparer<T>.Default;
            for (T v = target; ; v = previous[v])
            {
                path.Add(v);
                if (comparer.Equals(v, source)) break;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Prim's minimum spanning tree grown from <paramref name="start"/> with a min-heap of candidate edges:
        /// O(E log V). Only covers the component that contains the start vertex.
        /// </summary>
        public List<GraphEdge<T>> PrimMinimumSpanningTree(T start)
        {
            var tree = new List<GraphEdge<T>>();
            if (!adjacency.ContainsKey(start)) return tree;
            var inTree = new HashSet<T> { start };
            var heap = new MinHeap<GraphEdge<T>>(adjacency[start], (a, b) => a.Weight.CompareTo(b.Weight));
            while (!heap.IsEmpty && inTree.Count < order.Count)
            {
                var edge = heap.Pop();
                if (inTree.Contains(edge.To)) continue;
                inTree.Add(edge.To);
                tree.Add(edge);
                foreach (var next in adjacency[edge.To])
                    if (!inTree.Contains(next.To)) heap.Push(next);
            }
            return tree;
        }

        /// <summary>Kruskal's minimum spanning forest: sort edges, join components with union-find. O(E log E).</summary>
        public List<GraphEdge<T>> KruskalMinimumSpanningTree()
        {
            var sets = new UnionFind<T>(order);
            var tree = new List<GraphEdge<T>>();
            foreach (var e in Edges().OrderBy(e => e.Weight))
            {
                if (sets.Union(e.From, e.To)) tree.Add(e);
                if (tree.Count == order.Count - 1) break;
            }
            return tree;
        }

        public static double TotalWeight(IEnumerable<GraphEdge<T>> edges) => edges.Sum(e => e.Weight);
    }

    /// <summary>Disjoint-set forest with path compression and union by rank.</summary>
    public class UnionFind<T>
    {
        private readonly Dictionary<T, T> parent = new Dictionary<T, T>();
        private readonly Dictionary<T, int> rank = new Dictionary<T, int>();

        public UnionFind(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                parent[item] = item;
                rank[item] = 0;
            }
        }

        public T Find(T item)
        {
            var rootItem = item;
            while (!EqualityComparer<T>.Default.Equals(parent[rootItem], rootItem)) rootItem = parent[rootItem];
            while (!EqualityComparer<T>.Default.Equals(parent[item], rootItem))
            {
                var next = parent[item];
                parent[item] = rootItem;
                item = next;
            }
            return rootItem;
        }

        /// <summary>Joins the sets holding a and b; false when they were already in the same set.</summary>
        public bool Union(T a, T b)
        {
            T ra = Find(a), rb = Find(b);
            if (EqualityComparer<T>.Default.Equals(ra, rb)) return false;
            if (rank[ra] < rank[rb]) { var t = ra; ra = rb; rb = t; }
            parent[rb] = ra;
            if (rank[ra] == rank[rb]) rank[ra]++;
            return true;
        }
    }
}
