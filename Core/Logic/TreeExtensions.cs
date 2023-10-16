using System;
using System.Collections.Generic;
using System.Linq;

namespace Brupper.Logic
{
    /// <summary> https://habr.com/en/post/516596 </summary>
    public static partial class TreeExtensions
    {
        #region Dummy Tree Builder withour parent element

        /// <summary> Generates tree of items from item list </summary>
        /// 
        /// <typeparam name="TModel">Type of item in collection</typeparam>
        /// <typeparam name="K">Type of parent</typeparam>
        /// 
        /// <param name="collection">Collection of items</param>
        /// <param name="idSelector">Function extracting item's id</param>
        /// <param name="parentIdSelector">Function extracting item's parent_id</param>
        /// <param name="rootId">Root element id</param>
        /// 
        /// <returns>Tree of items</returns>
        public static IEnumerable<Tree<TModel>> GenerateTree<TModel, K>(this IEnumerable<TModel> collection,
            Func<TModel, K> idSelector,
            Func<TModel, K> parentIdSelector,
            K? rootId = default)
        {
            #region Dummy and old
            //foreach (var c in collection.Where(c => EqualityComparer<K>.Default.Equals(parentIdSelector(c), rootId)))
            //{
            //    yield return new Tree<T>
            //    {
            //        Data = c,
            //        Children = collection.GenerateTree(idSelector, parentIdSelector, idSelector(c)).ToList(),
            //        // Parent = ... TODO: collection.FirstOrDefault(p => EqualityComparer<K>.Default.Equals(parentIdSelector(p), rootId)),
            //    };
            //}
            #endregion

            #region ez meg agyon van optimalizalva...

            var tree = collection
                .Where(c => EqualityComparer<K>.Default.Equals(parentIdSelector(c), rootId))
                .Select(c => new Tree<TModel>
                {
                    Data = c,
                    Children = collection.GenerateTree(idSelector, parentIdSelector, idSelector(c)).ToList(),
                    // Parent = ... TODO: collection.FirstOrDefault(p => EqualityComparer<K>.Default.Equals(parentIdSelector(p), rootId)),
                })
                .ToList();

            var allFlattenedTreeNodes = tree.Flatten(x => x.Children).ToList();
            foreach (var node in allFlattenedTreeNodes)
            {
                node.Parent = allFlattenedTreeNodes
                    .FirstOrDefault(x => x?.Children?.Any(c => EqualityComparer<K>.Default.Equals(idSelector(c.Data), idSelector(node.Data))) ?? false);
            }

            return tree;

            #endregion
        }

        #endregion

        /// <summary> Flatten tree to plain list of nodes </summary>
        public static IEnumerable<TNode> Flatten<TNode>(this IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> childrenSelector)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            return nodes.SelectMany(c => childrenSelector(c).Flatten(childrenSelector)).Concat(nodes);
        }

        /// <summary> Converts given list to tree. </summary>
        /// <typeparam name="T">Custom data type to associate with tree node.</typeparam>
        /// <param name="items">The collection items.</param>
        /// <param name="parentSelector">Expression to select parent.</param>
        public static Tree<T> ToTree<T>(this IList<T> items, Func<T, T, bool> parentSelector)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var lookup = items.ToLookup(item => items.FirstOrDefault(parent => parentSelector(parent, item)), child => child);
            return Tree<T>.FromLookup(lookup);
        }

    }
}
