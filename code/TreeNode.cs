using System;
using System.Collections.Generic;

namespace translator
{
    class TreeNode
    {
        public string nodeName;
        public int code;
        public List<TreeNode> children;
        public Token token;

        public TreeNode(string nodeName)
        {
            this.nodeName = nodeName;
            this.children = new List<TreeNode>();
            this.code = -1;
        }

        public TreeNode (string nodeName, int code)
        {
            this.nodeName = nodeName;
            this.children = new List<TreeNode>();
            this.code = code;
        }

        public TreeNode AddChild(string nodeName)
        {
            this.children.Add(new TreeNode(nodeName));
            return this.children[children.Count - 1];
        }

        public TreeNode AddChild(string nodeName, int code)
        {
            this.children.Add(new TreeNode(nodeName, code));
            return this.children[children.Count - 1];
        }
        
        public void PrintTree()
        {
            PrintT(this, 0);
        }

        private void PrintT(TreeNode cur, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write(" ");
            }
            if (cur.code != -1)
            {
                Console.Write(cur.code + " -> ");
            }
            Console.WriteLine(cur.nodeName);
            foreach (TreeNode n in cur.children)
            {
                PrintT(n, depth + 1);
            }
        }
    }
}
