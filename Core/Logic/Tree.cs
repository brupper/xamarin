using System;
using System.Collections.Generic;
using System.Linq;

namespace Brupper.Logic
{
    /// <summary> Internal implementation of <see cref="ITree{T}" /></summary>
    /// <typeparam name="TTreeNodeData">Custom data type to associate with tree node.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{Data,nq} - {Level,nq}")]
    public class Tree<TTreeNodeData>
    {
        public TTreeNodeData Data { get; set;/* TODO: private setter */ } = default!;

        public Tree<TTreeNodeData>? Parent { get; set; /* TODO: private setter */  }

        public ICollection<Tree<TTreeNodeData>> Children { get; set;/* TODO: remove setter */ } = new List<Tree<TTreeNodeData>>(0);

        public bool IsRoot => Parent == null;

        public bool IsLeaf => Children?.Count == 0;

        public int Level => IsRoot ? 0 : (Parent?.Level ?? 0) + 1;

        #region Constructor

        private Tree(TTreeNodeData? data)
        {
            Children = new LinkedList<Tree<TTreeNodeData>>();
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public Tree() { }

        #endregion

        private void LoadChildren(ILookup<TTreeNodeData?, TTreeNodeData> lookup)
        {
            foreach (var data in lookup[Data])
            {
                var child = new Tree<TTreeNodeData>(data) { Parent = this };
                Children.Add(child);
                child.LoadChildren(lookup);
            }
        }

        public static Tree<TTreeNodeData> FromLookup(ILookup<TTreeNodeData?, TTreeNodeData> lookup)
        {
            var rootData = lookup.Count == 1 ? lookup.First().Key : default(TTreeNodeData);
            var root = new Tree<TTreeNodeData>(rootData);
            root.LoadChildren(lookup);
            return root;
        }

        public IEnumerable<Tree<TTreeNodeData>> Flatten()
        {
            yield return this;

            foreach (var node in Children.SelectMany(child => child.Flatten()))
            {
                yield return node;
            }
        }

        public void RecursiveOrder(string orderProperty)
        {
            Children = Children.OrderBy(x => x.Data?.GetPropertyValue(orderProperty))
                .ToList();

            Children.ToList().ForEach(c => c.RecursiveOrder(orderProperty));
        }
    }

    /*
    /// <summary> Generic interface for tree node structure </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITree<T>
    {
        T Data { get; }

        ITree<T> Parent { get; }

        ICollection<ITree<T>> Children { get; }

        bool IsRoot { get; }

        bool IsLeaf { get; }

        int Level { get; }

        IEnumerable<ITree<T>> Flatten();
    }
    */
}
