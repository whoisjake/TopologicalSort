using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using DevelopStuff.Examples.TopologicalSort.Library;

namespace DevelopStuff.Examples.TopologicalSort.Tests
{
    /// <summary>
    ///This is a test class for DevelopStuff.Examples.TopologicalSort.Library.Node and is intended
    ///to contain all DevelopStuff.Examples.TopologicalSort.Library.Node Unit Tests
    ///</summary>
    [TestClass()]
    public class NodeTest
    {
        /// <summary>
        /// Tests the topological sorter.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ApplicationException))]
        public void TestTopologicalSorterWithCircularGraph()
        {
            List<Node> nodes = new List<Node>();

            Node seven = new Node(7);
            nodes.Add(seven);

            Node five = new Node(5);
            nodes.Add(five);

            five.Parent = seven;
            seven.Parent = five;

            TopologicalSorter<Node> sorter = new TopologicalSorter<Node>();

            IList<Node> sorted = sorter.Sort(nodes);
        }

        /// <summary>
        /// Tests the topological sorter.
        /// </summary>
        [TestMethod()]
        public void TestTopologicalSorter()
        {
            List<Node> nodes = new List<Node>();

            Node seven = new Node(7);
            nodes.Add(seven);

            Node five = new Node(5);
            nodes.Add(five);

            Node three = new Node(3);
            nodes.Add(three);

            Node eleven = new Node(11);
            nodes.Add(eleven);

            Node eight = new Node(8);
            nodes.Add(eight);

            Node two = new Node(2);
            nodes.Add(two);

            Node nine = new Node(9);
            nodes.Add(nine);

            Node ten = new Node(10);
            nodes.Add(ten);

            three.Parent = two;
            eleven.Parent = seven;
            seven.Parent = eight;
            five.Parent = nine;
            ten.Parent = eleven;
            nine.Parent = eleven;

            TopologicalSorter<Node> sorter = new TopologicalSorter<Node>();

            IList<Node> sorted = sorter.Sort(nodes);

            StringBuilder sb = new StringBuilder();

            foreach (Node node in sorted)
            {
                sb.Append(node.ID + " ");
            }

            Assert.IsTrue(sorted.IndexOf(two) < sorted.IndexOf(three), "The order: " + sb.ToString());
            Assert.IsTrue(sorted.IndexOf(eight) < sorted.IndexOf(seven), "The order: " + sb.ToString());
            Assert.IsTrue(sorted.IndexOf(seven) < sorted.IndexOf(eleven), "The order: " + sb.ToString());
            Assert.IsTrue(sorted.IndexOf(eleven) < sorted.IndexOf(nine), "The order: " + sb.ToString());
            Assert.IsTrue(sorted.IndexOf(eleven) < sorted.IndexOf(ten), "The order: " + sb.ToString());
            Assert.IsTrue(sorted.IndexOf(nine) < sorted.IndexOf(five), "The order: " + sb.ToString());

            Random rand = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 10000; i++)
            {
                List<Node> newList = this.RandomizeList(sorted, rand);

                sorted = sorter.Sort(newList);

                sb = new StringBuilder();

                foreach (Node node in sorted)
                {
                    sb.Append(node.ID + " ");
                }

                Assert.IsTrue(sorted.IndexOf(two) < sorted.IndexOf(three), "The order: " + sb.ToString());
                Assert.IsTrue(sorted.IndexOf(eight) < sorted.IndexOf(seven), "The order: " + sb.ToString());
                Assert.IsTrue(sorted.IndexOf(seven) < sorted.IndexOf(eleven), "The order: " + sb.ToString());
                Assert.IsTrue(sorted.IndexOf(eleven) < sorted.IndexOf(nine), "The order: " + sb.ToString());
                Assert.IsTrue(sorted.IndexOf(eleven) < sorted.IndexOf(ten), "The order: " + sb.ToString());
                Assert.IsTrue(sorted.IndexOf(nine) < sorted.IndexOf(five), "The order: " + sb.ToString());
            }

        }
        
        /// <summary>
        /// Randomizes the list.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private List<Node> RandomizeList(IList<Node> nodes, Random random)
        {
            List<Node> newList = new List<Node>();

            while (nodes.Count > 0)
            {
                Node toRemoveNode = null;

                while (nodes.Count > 0)
                {
                    int rand = random.Next(0, nodes.Count - 1);
                    toRemoveNode = nodes[rand];
                    nodes.Remove(toRemoveNode);
                    newList.Add(toRemoveNode);
                }
            }

            return newList;
        }

    }


}
