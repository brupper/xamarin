using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Brupper.Logic
{
    public class TreeNode<T>
    {
        private readonly T value;
        private readonly List<TreeNode<T>> children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            this.value = value;
        }

        public TreeNode<T> this[int i] => children[i];

        public TreeNode<T> Parent { get; private set; }

        public T Value => value;

        public ReadOnlyCollection<TreeNode<T>> Children => children.AsReadOnly();

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(params T[] values) => values.Select(AddChild).ToArray();

        public bool RemoveChild(TreeNode<T> node) => children.Remove(node);

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in children)
            {
                child.Traverse(action);
            }
        }

        public IEnumerable<T> Flatten() => new[] { Value }.Concat(children.SelectMany(x => x.Flatten()));
    }
}
