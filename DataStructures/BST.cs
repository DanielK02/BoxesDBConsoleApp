using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    internal class BST<T> where T : IComparable
    {
        public Node root;

        public void Add(T value)
        {
            AddNode(value);
        }

        private void AddNode(T value) // O(logN) - O(N)
        {
            if (root == null)
            {
                root = new Node(value);
                return;
            }
            Node tmp = root;

            while (true)
            {

                if (value.CompareTo(tmp.data) < 0) //value < tmp.data ->go left
                {
                    if (tmp.left == null)
                    {
                        tmp.left = new Node(value);
                        break;
                    }
                    tmp = tmp.left;
                }
                else // value < tmp.data -> go right
                {
                    if (tmp.right == null)
                    {
                        tmp.right = new Node(value);
                        break;
                    }
                    tmp = tmp.right;
                }
            }
        }


        // Check if Node exists in Tree
        public T IfNodeExists(T value)
        {
            BST<T>.Node checkNull = IfNodeExists(this.root, value);
            if (checkNull != null)
            {
                return checkNull.data;
            }
            return default;
        }
        private Node IfNodeExists(Node node, T key)
        {
            if (node == null)
                return null;

            if (node.data.CompareTo(key) == 0)
                return node;

            // then recur on left subtree /
            Node leftNode = IfNodeExists(node.left, key);

            // node found, no need to look further
            if (leftNode != null)
            {
                if (leftNode.data.CompareTo(key) == 0) return leftNode;
            }


            // node is not found in left,
            // so recur on right subtree /
            Node rightNode = IfNodeExists(node.right, key);

            if (rightNode != null)
            {
                if (rightNode.data.CompareTo(key) == 0) return rightNode;
            }

            return null;
        }


        // Scan BST in order
        public void ScanInOrder(Action<T> itemAction)
        {
            ScanInOrder(root, itemAction);
        }

        private void ScanInOrder(Node tmp, Action<T> itemAction)
        {
            if (tmp == null) return;
            ScanInOrder(tmp.left, itemAction);
            itemAction(tmp.data);
            ScanInOrder(tmp.right, itemAction);
        }

        // Remove function
        public bool Remove(T item)
        {

            root = RemoveItem(root, item, out bool isRemoved);
            return isRemoved;
        }
        private Node RemoveItem(Node root, T item, out bool isRemoved)
        {
            isRemoved = true;  // Set to true, will return false in case of a failure to remove

            if (root == null) // If the tree is empty or recursion condition
            {
                isRemoved = false;
                return root;
            }

            // Look for the value in the BST
            if (root.data.CompareTo(item) > 0)
                root.left = RemoveItem(root.left, item, out isRemoved);
            else if (root.data.CompareTo(item) < 0)
                root.right = RemoveItem(root.right, item, out isRemoved);

            // If value was found procceed to the following
            else
            {
                // If node is leaf or node with one child
                if (root.left == null)
                {
                    return root.right;
                }

                else if (root.right == null)
                {
                    return root.left;
                }

                // If the node to remove has 2 child
                root.data = GetSuccessor(root.right);

                // Delete the successor after moving it to the correct place
                root.right = RemoveItem(root.right, root.data, out isRemoved);
            }
            return root;
        }

        private T GetSuccessor(Node root)
        {
            T successor = root.data;
            while (root.left != null)
            {
                successor = root.left.data;
                root = root.left;
            }
            return successor;
        }


        public T FindNextBiggerNode(T key)
        {
            return FindNextBiggerNode(this.root, key);
        }
        private T FindNextBiggerNode(Node root, T N)
        {
            Node tempNext = default;
            // Start from root and keep looking for larger
            while (root != null && root.right != null)
            {
                // If Node the same as root go to right side
                if (N.CompareTo(root.data) == 0)
                    root = root.right;
                // If root is smaller go to right side
                else if (N.CompareTo(root.data) > 0)
                {
                    root = root.right;
                }
                // If root is greater go to left side and save the current bigger node in case on left side there won't be any
                else if (N.CompareTo(root.data) < 0)
                {
                    tempNext = root;
                    if (root.left != null)
                    {
                        if (N.CompareTo(root.left.data) >= 0)
                        {
                            root = root.left;
                        }
                        else if (N.CompareTo(root.left.data) < 0)
                        {
                            root = root.left;
                        }
                    }

                    else if (N.CompareTo(tempNext.data) < 0)
                        return tempNext.data;
                }
                else
                {
                    tempNext.data = default;
                    break;
                }

            }
            if (root != null) // if reaches here, broke out of while and probably found
            {
                if (root.right == null) //if right is null, can save and exit (I think it caused some null references exc when I was writing, that's why it's here)
                {
                    if (N.CompareTo(root.data) < 0) // last validation
                        return root.data;
                }
            }
            // If none were found or tempNext is the neeeded node
            if (root == null || tempNext == null || root.data.CompareTo(N) < 0)
                return default;

            return tempNext.data;
        }

        internal class Node
        {
            public Node left;
            public Node right;
            public T data;

            public Node(T data)
            {
                this.data = data;
            }
        }

    }
}
