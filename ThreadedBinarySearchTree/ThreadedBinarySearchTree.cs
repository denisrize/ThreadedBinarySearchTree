using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadedBinarySearchTree
{
    internal class ThreadedBinarySearchTree
    {

        private Node root;
        int readerTimeOut;
        int writerTimeOut;


        public ThreadedBinarySearchTree(int rTime=1000,int wTime=1000)
        {
            root = new Node(0);
            //readerTimeOut = rTime;
            //writerTimeOut = wTime;  
        }
        //add num to the tree, if it already exists, do nothing
        public void add(int num) {
            root.myLock.EnterWriteLock();

            Node curr = root;
            Node next = null;

            while (true)
            {
                if (num < curr.value)
                {
                    if (curr.left == null)
                    {
                        curr.left = new Node(num);
                        curr.myLock.ExitWriteLock();
                        break;
                    }
                    next = curr.left;

                }
                else if (num > curr.value)
                {
                    if (curr.right == null)
                    {
                        curr.right = new Node(num);
                        curr.myLock.ExitWriteLock();
                        break;
                    }
                    next = curr.right;
                }
                else
                {
                    curr.myLock.ExitWriteLock();
                    break;
                }
                next.myLock.EnterWriteLock();
                curr.myLock.ExitWriteLock();
                curr = next;
            }


        }

        // remove num from the tree, if it doesn't exists, do nothing
        public void remove(int num) {
            root.myLock.EnterWriteLock();

            Node parnet = root;
            Node curr = null;
            Node next = null;

            if (num < root.value && parnet.left != null)
            {
                curr = parnet.left;
                curr.myLock.EnterWriteLock();

            }
            else if (parnet.right != null)
            {
                curr = parnet.right;
                curr.myLock.EnterWriteLock();

            }

            while (curr != null)
            {
                if(num < curr.value)
                {
                    if(curr.left == null)
                    {
                        curr.myLock.ExitWriteLock();
                        parnet.myLock.ExitWriteLock();
                        break;
                    }
                    next = curr.left;
                }
                else if( num > curr.value)
                {
                    if (curr.right == null)
                    {
                        curr.myLock.ExitWriteLock();
                        parnet.myLock.ExitWriteLock();
                        break;
                    }
                    next = curr.right;
                }
                else // curr.value = num
                {
                    if( curr.left == null && curr.right == null) // no children
                    {
                        if (parnet.left == curr) parnet.left = null;
                        else parnet.right = null;
                        parnet.myLock.ExitWriteLock();
                        break;

                    } 
                    else if(curr.left == null || curr.right == null) // one child
                    {
                        if( parnet.left == curr) parnet.left = curr.left == null? curr.right: curr.left;
                        else parnet.right = curr.right == null? curr.left: curr.right;

                        parnet.myLock.ExitWriteLock();
                        break;
                    }
                    else // two children
                    {
                        Node currParent = curr;
                        Node currMin = curr.right;
                        currMin.myLock.EnterWriteLock();
                        parnet.myLock.ExitWriteLock();

                        while (true)
                        {

                            if( currMin.left == null) // minimum founded
                            {
                                curr.value = currMin.value; // switch
                                if( currParent == curr)
                                {
                                    currParent.right = currMin.right;
                                }
                                else
                                {
                                    currParent.left = currMin.right;
                                    currParent.myLock.ExitWriteLock();
                                }
                                curr.myLock.ExitWriteLock();
                                break;

                            }
                            // if not founed keep traveling
                            currMin.left.myLock.EnterWriteLock();
                            if(currParent != curr) currParent.myLock.ExitWriteLock();

                            currParent = currMin;
                            currMin = currMin.left;


                        }


                    }
            }

                if (next != null)
                {
                    next.myLock.EnterWriteLock();
                    parnet.myLock.ExitWriteLock();
                }

                parnet = curr;
                curr = next;
            }

        }

        // search num in the tree and return true/false accordingly
        public bool search(int num)
        {
            Node curr = null;
            Node next = null;
            if (num < root.value && root.left != null)
            {
                curr = root.left;
                curr.myLock.EnterReadLock();

            }
            else if (root.right != null)
            {
                curr = root.right;
                curr.myLock.EnterReadLock();
            }

            while (curr != null)
            {
                if(curr.value > num)
                {
                    if(curr.left == null)
                    {
                        curr.myLock.ExitReadLock();
                        break;
                    }
                    next = curr.left;
                }
                else if( curr.value < num)
                {
                    if(curr.right == null)
                    {
                        curr.myLock.ExitReadLock();
                        break;
                    }
                    next = curr.right;
                }
                else
                {
                    curr.myLock.ExitReadLock();
                    return true;
                }

                next.myLock.EnterReadLock();
                curr.myLock.ExitReadLock();
                curr = next;
            }

            return false;
        }

        // remove all items from tree
        public void clear() { } 

        // print the values of three from the smallest to largest in comma delimited form. For example; -15,0,1,3,5,23,40,89,201. If the tree is empty do nothing.
        public void print()
        {
            Stack<Node> stack = new Stack<Node>();
            Node curr = root;
            Node next = null;

            curr.myLock.EnterReadLock();

            while( curr != null || stack.Count > 0)
            {
                while( curr != null)
                {
                    stack.Push(curr);
                    next = curr.left;
                    if (next != null) { next.myLock.EnterReadLock(); }
                    curr.myLock.ExitReadLock();                    
                    curr = next;
                }

                curr = stack.Pop();
                curr.myLock.EnterReadLock();
                Console.Write( curr.value + " ");
                next = curr.right;
                if (next != null) { next.myLock.EnterReadLock(); }
                curr.myLock.ExitReadLock();
                curr = next;

            }


        }
        private class Node
        {
            public Node left;
            public Node right;
            public int value;
            public ReaderWriterLockSlim myLock;

            public Node(int val)
            {
                myLock = new ReaderWriterLockSlim();
                value = val;
            }

        }
    }
}
