using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class BoxY : IComparable
    {
        private double Y;
        private int count;
        DateTime lastPurchaseTime = DateTime.Now;

        public DateTime LastPurchaseTime
        {
            get { return lastPurchaseTime; }
            set { lastPurchaseTime = value; }
        }
        public int OnHoldCount { get; set; } // Count while user purchases boxes, puts boxes in list and "On Hold", used in case user returns

        //ctor
        public BoxY(double YValue)
        {
            this.Y = YValue;
        }

        public int CompareTo(object obj)
        {
            BoxY temp = obj as BoxY;
            if (temp == null)
                return default;
            return Y.CompareTo(temp.Y);
        } // Compare by Y double value
        public int Count // Count of how many boxes there are in given Y
        {
            get { return count; }
            set { count = value; }
        }
        public double YValue
        {
            get { return Y; }
            set { Y = value; }
        }
        public double XParent { get; set; } // X parent value, used for re-adding boxes and getting data

    }


    internal class BoxX : IComparable
    {
        double X;
        public BST<BoxY> YTree = new BST<BoxY>();

        //ctor
        public BoxX(double XValue)
        {
            this.X = XValue;
        }

        public double XValue
        {
            get { return X; }
            set { X = value; }
        }

        public int CompareTo(object obj) // Compare by X double value
        {
            BoxX temp = obj as BoxX;
            if (temp == null)
                return default;
            return X.CompareTo(temp.X);
        }       

        // Adding X and Y, checks for existence of X and Y
        public bool AddBoxXandY(BST<BoxX> BSTBoxX, BoxY boxY)
        {
            // Add X parent data to BoxY that is about to be added
            boxY.XParent = this.XValue;
            // Check for X if exists
            BoxX XtempBox = BSTBoxX.IfNodeExists(this);
            // If X doesn't exists, create a new X node + Y node in the X Node
            if (XtempBox == null)
            {
                BSTBoxX.Add(this);

                if (this.AddBoxY(boxY))
                {
                    XtempBox = BSTBoxX.IfNodeExists(XtempBox);
                    Warehouse.expiryCheckList.AddLast(this.YTree.root.data);
                }
                else
                    return false;
            }
            else if(XtempBox != null) // If not null means X exists
            {
                BoxY YtempBox = XtempBox.YTree.IfNodeExists(boxY); // Check for Y if exists

                if (YtempBox == null) // If not, add.
                {
                    boxY.Count++;
                    XtempBox.YTree.Add(boxY);
                    YtempBox = XtempBox.YTree.IfNodeExists(boxY);
                    Warehouse.expiryCheckList.AddLast(YtempBox);
                }
                else if (YtempBox.Count >= Warehouse.MaxBoxesNumber) // Checks if number of boxes is max so it won't be higher than count
                    return false;

                else // means that Y exists, count up.
                {
                    YtempBox.Count++;
                    //Warehouse.expiryCheckList.AddLast(YtempBox);
                }
            }
            return true;
        }

        // Function used only when Y needs to be added, when X exists
        public bool AddBoxY(BoxY yBox)
        {
            if (yBox.Count >= Warehouse.MaxBoxesNumber) // Checks count not exceeds
            {
                return false;
            }
            // else
            yBox.Count++;
            yBox.XParent = this.XValue; ;
            yBox.LastPurchaseTime = DateTime.Now;
            YTree.Add(yBox);
            return true;
        }


        public bool RemoveBoxY(BoxY boxY)
        {
            BoxY tempYBox;
            tempYBox = this.YTree.IfNodeExists(boxY); // Gets the node to be deleted from the YTree of BoxX
            if (tempYBox != null)
            {
                if (tempYBox.Count > 1) // Checks for amount, if larger than 1, just lower count so it won't remove all
                    tempYBox.Count--;
                else if (tempYBox.Count <= 1)
                {
                    Warehouse.ExpiryCheckList.Remove(tempYBox); // Remove from ExpiryCheckList
                    this.YTree.Remove(boxY); // Remove from Tree
                }
                    

                return true;
            }           
            return false;
        }
    }
}
