using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Box
{
    class BoxY : IComparable
    {
        double Y;
        int Count;
        DateTime LastPurchaseTime;

        public int CompareTo(object obj)
        {
            return Y.CompareTo(obj);
        }
    }
    class BoxX : IComparable
    {
        double X;
        BST<BoxY> YTree;

        public int CompareTo(object obj)
        {
            return X.CompareTo(obj);
        }
    }
}
