using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling
{
    namespace FunctionTree
    {
        public abstract class FunctionTreeNode
        {
            List<FunctionTreeNode> children = new List<FunctionTreeNode>();
            public List<FunctionTreeNode> Children
            {
                get { return children; }
                set { children = value; }
            }

            public int NumChildren()
            {
                int number = 1; // this node
                foreach (FunctionTreeNode n in children)
                {
                    number += n.NumChildren();
                }
                return number;
            }
            abstract public double Evaluate();
        }


        public delegate double NodeFunction(double[] inputs);

        [Serializable]
        public class BranchNode : FunctionTreeNode
        {
            NodeFunction function;

            public BranchNode(NodeFunction Function)
            {
                function = Function;
            }

            override public double Evaluate()
            {
                double[] inputs = new double[base.Children.Count];
                for (int i = 0; i < base.Children.Count; i++)
                {
                    inputs[i] = base.Children[i].Evaluate();
                }
                return function(inputs);
            }
        }

        [Serializable]
        public class LeafNode : FunctionTreeNode
        {
            double value;

            public LeafNode(double Value)
            {
                value = Value;
            }

            override public double Evaluate()
            {
                return value;
            }
        }
    }
}
