using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DevelopStuff.Examples.TopologicalSort.Library
{
    /// <summary>
    /// A topological sorter that unwraps an object graph for relationship management.
    /// </summary>
    public class TopologicalSorter<T>
    {

        #region Internal Classes

        /// <summary>
        /// Private internal class to help the sorter.
        /// </summary>
        private class GraphNode<N>
        {
            #region Fields

            private List<GraphNode<N>> _children;
            private List<GraphNode<N>> _parents;

            private N _model;

            #endregion

            /// <summary>
            /// Initializes a new instance of the Node class.
            /// </summary>
            public GraphNode(N model)
            {
                this.Model = model;
            }

            #region Properties

            /// <summary>
            /// Gets or sets the model it represents.
            /// </summary>
            public N Model
            {
                get
                {
                    return this._model;
                }
                set
                {
                    this._model = value;
                }
            }

            /// <summary>
            /// Gets the parent nodes.
            /// </summary>
            /// <value></value>
            public IList<GraphNode<N>> Parents
            {
                get
                {
                    if (this._parents == null)
                    {
                        this._parents = new List<GraphNode<N>>();
                    }

                    return this._parents;
                }
            }

            /// <summary>
            /// Gets the child nodes.
            /// </summary>
            /// <value></value>
            public IList<GraphNode<N>> Children
            {
                get
                {
                    if (this._children == null)
                    {
                        this._children = new List<GraphNode<N>>();
                    }

                    return this._children;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Adds the child node.
            /// </summary>
            /// <param name="childNode">The child node.</param>
            public void AddChildNode(GraphNode<N> childNode)
            {
                if (!this.Children.Contains(childNode))
                {
                    this.Children.Add(childNode);
                }

                if (!childNode.Parents.Contains(this))
                {
                    childNode.AddParentNode(this);
                }
            }

            /// <summary>
            /// Removes the child node.
            /// </summary>
            /// <param name="childNode">The child node.</param>
            public void RemoveChildNode(GraphNode<N> childNode)
            {
                if (this.Children.Contains(childNode))
                {
                    this.Children.Remove(childNode);
                }

                if (childNode.Parents.Contains(this))
                {
                    childNode.RemoveParentNode(this);
                }
            }

            /// <summary>
            /// Adds the parent node.
            /// </summary>
            /// <param name="parentNode">The parent node.</param>
            public void AddParentNode(GraphNode<N> parentNode)
            {
                if (!this.Parents.Contains(parentNode))
                {
                    this.Parents.Add(parentNode);
                }

                if (!parentNode.Children.Contains(this))
                {
                    parentNode.AddChildNode(this);
                }
            }

            /// <summary>
            /// Removes the parent node.
            /// </summary>
            /// <param name="parentNode">The parent node.</param>
            public void RemoveParentNode(GraphNode<N> parentNode)
            {
                if (this.Parents.Contains(parentNode))
                {
                    this.Parents.Remove(parentNode);
                }

                if (parentNode.Children.Contains(this))
                {
                    parentNode.RemoveChildNode(this);
                }
            }
            #endregion
        }

        #endregion

        /// <summary>
        /// Sorts the <see cref="BaseModel"/>s in a topological fashion.
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public IList<T> Sort(IList<T> models)
        {
            List<T> sorted = new List<T>();

            List<GraphNode<T>> nodes = this.GenerateNodes(models);

            // Queue up the nodes that don't have any incoming edges.
            Queue<GraphNode<T>> rootNodes = this.GetRootNodes(nodes);

            while (nodes.Count > 0)
            {
                // if you have nodes, but no nodes that don't have any incoming edges, then you've got a circular reference.
                if (rootNodes.Count == 0)
                {
                    throw new ApplicationException("The graph contains a circular reference.");
                }

                // grab the first start node.
                GraphNode<T> n = rootNodes.Dequeue();

                // add it your output.
                sorted.Add(n.Model);

                // for each of it's dependancies, 
                // remove the incoming edge,
                // queue the node,
                // then remove the original node from the graph.
                for (int i = (n.Children.Count - 1); i >= 0; i--)
                {
                    GraphNode<T> childNode = n.Children[i];
                    n.RemoveChildNode(childNode);
                    if (childNode.Parents.Count == 0)
                    {
                        rootNodes.Enqueue(childNode);
                    }
                }

                nodes.Remove(n);
            }

            return sorted;
        }

        /// <summary>
        /// Generates a node collection from models.
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<TopologicalSorter<T>.GraphNode<T>> GenerateNodes(IList<T> models)
        {
            List<TopologicalSorter<T>.GraphNode<T>> nodes = new List<TopologicalSorter<T>.GraphNode<T>>();
            Hashtable preMadeNodes = new Hashtable();

            foreach (T model in models)
            {
                TopologicalSorter<T>.GraphNode<T> node = new TopologicalSorter<T>.GraphNode<T>(model);
                preMadeNodes.Add(model, node);
                nodes.Add(node);
            }

            foreach (T model in models)
            {
                TopologicalSorter<T>.GraphNode<T> modelNode = preMadeNodes[model] as TopologicalSorter<T>.GraphNode<T>;
                List<T> parents = GetParentModels(model, models);

                foreach (T parent in parents)
                {
                    TopologicalSorter<T>.GraphNode<T> parentNode = preMadeNodes[parent] as TopologicalSorter<T>.GraphNode<T>;
                    if (parentNode != null)
                    {
                        parentNode.AddChildNode(modelNode);
                    }
                }
            }

            return nodes;
        }

        /// <summary>
        /// Gets a list of parent models given a child model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<T> GetParentModels(T model, IList<T> models)
        {
            List<T> parents = new List<T>();

            Type modelType = model.GetType();
            Dictionary<string, ParentAttribute> parentAttributes = ParentAttribute.ForClass(modelType);

            foreach (string propertyName in parentAttributes.Keys)
            {
                PropertyInfo parentProperty = modelType.GetProperty(propertyName);

                T parentValue = (T)parentProperty.GetValue(model, null);

                if (parentValue != null)
                {
                    if (models.Contains(parentValue))
                        parents.Add(parentValue);
                }
            }

            return parents;
        }

        /// <summary>
        /// Gets the root nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns></returns>
        private Queue<GraphNode<T>> GetRootNodes(IList<GraphNode<T>> nodes)
        {
            Queue<GraphNode<T>> rootNodes = new Queue<GraphNode<T>>();
            foreach (GraphNode<T> node in nodes)
            {
                if (node.Parents.Count == 0)
                {
                    rootNodes.Enqueue(node);
                }
            }

            return rootNodes;
        }
    }
}
