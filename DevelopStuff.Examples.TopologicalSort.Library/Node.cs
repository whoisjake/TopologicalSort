using System;
using System.Collections.Generic;
using System.Text;

namespace DevelopStuff.Examples.TopologicalSort.Library
{
    public class Node
    {

        #region Fields

        private int _id;
        private List<Node> _children;
        private Node _parent;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Node"/> class.
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Node"/> class.
        /// </summary>
        public Node(int id)
        {
            this.ID = id;
        }

        #region Properties

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value></value>
        [Parent(typeof(Node), "Children")]
        public Node Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
            }
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value></value>
        public IList<Node> Children
        {
            get
            {
                if (this._children == null)
                {
                    this._children = new List<Node>();
                }

                return this._children;
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value></value>
        public int ID
        {
            get 
            { 
                return this._id; 
            }
            set 
            { 
                this._id = value; 
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.ID.ToString();
        }

        /// <summary>
        /// Adds the child node.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        public void AddChildNode(Node childNode)
        {
            if (!this.Children.Contains(childNode))
            {
                this.Children.Add(childNode);
            }

            childNode.Parent = this;
        }

        /// <summary>
        /// Removes the child node.
        /// </summary>
        /// <param name="childNode">The child node.</param>
        public void RemoveChildNode(Node childNode)
        {
            if (this.Children.Contains(childNode))
            {
                this.Children.Remove(childNode);
            }

            childNode.Parent = null;
        }

        #endregion

    }
}
