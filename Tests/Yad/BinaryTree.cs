#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

#endregion

namespace Tests.Yad
{
    public class BinaryTreeTests
    {
        [Fact]
        public void Bt1()
        {
            var rand = new Random(21);
            var values = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                values.Add(rand.Next(1, 100));
            }

            var ll = new LinkedList<int>();
            var node = ll.AddFirst(5);
        }
    }

    //class BinaryTreeNode<TNODE> : IComparable<TNODE> where TNODE : IComparable<TNODE>
    //{
    //    public TNODE Value { get; private set; }
    //    public BinaryTreeNode<TNODE> Left { get; set; }
    //    public BinaryTreeNode<TNODE> Right { get; set; }

    //    public BinaryTreeNode(TNODE value)
    //    {
    //        Value = value;
    //    }
    //    public int CompareTo(TNODE other)
    //    {
    //        return Value.CompareTo(other);
    //    }

    //    public int ComareNode(BinaryTreeNode<TNODE> other)
    //    {
    //        return Value.CompareTo(other.Value);
    //    }
    //}
}