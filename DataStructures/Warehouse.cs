using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace DataStructures
{
    public class Warehouse
    {
        private BST<BoxX> boxBST = new BST<BoxX>(); // Main Tree of BoxX
        private BoxX xTempBoxWH; // Temp Box that is used in some functions
        private BoxY yTempBoxWH; // Temp Box that is used in some functions

        public static LinkedList<BoxY> expiryCheckList = new LinkedList<BoxY>(); // Queue of Boxes by Expiry Date (closest expiry is first)
        private LinkedList<BoxY> normalSizeBoxList = new LinkedList<BoxY>(); // Temp list for GiftBoxes found in maxPercentage bounds
        private LinkedList<BoxY> biggerBoxesList = new LinkedList<BoxY>(); // Temp list for GiftBoxes found that exceed the percentage
        private LinkedList<BoxX> boxXList = new LinkedList<BoxX>(); // TempList to run through the tree(s) to show all stock

        internal static int configNumber = MaxBoxes();

        public static int MaxBoxesNumber
        {
            get { return configNumber; }
        }

        public LinkedList<BoxY> NormalSizedBoxes
        {
            get { return normalSizeBoxList; }
        }
        public LinkedList<BoxY> LargerSizeBoxes
        {
            get { return biggerBoxesList; }
        }
        public static LinkedList<BoxY> ExpiryCheckList
        {
            get { return expiryCheckList; }
            set { expiryCheckList = value; }
        }

        private double maxPercentage = 0.50;

        public Warehouse() // CTOR for warehouse, boxes that initialized on startup
        {
            for (int i = 0; i < 10; i++)
            {
                AddBox(5, 6);
                AddBox(3, 12);
                AddBox(10, 7);
                AddBox(6, 9.5);
                AddBox(8, 2);
                AddBox(5, 8);
                AddBox(4, 2);
                AddBox(2, 8);
                AddBox(5.5, 9.6);
                AddBox(15.2, 3);
                AddBox(15.5, 9);
                AddBox(17.2, 6);
                AddBox(25.2, 10);
                AddBox(4, 30);
            }
        }



        #region Add / Remove boxes funcs
        // Function to add box
        public bool AddBox(double x, double y) // bool to check for higher than config
        {
            // Check if BST is empty
            if (boxBST.root == null)
            {
                boxBST.Add(new BoxX(x));
                if (boxBST.root.data.AddBoxY(new BoxY(y)) == true)
                {
                    ExpiryCheckList.AddLast(boxBST.root.data.YTree.root.data); // Only 1 item, so easy add to expiryCheckList
                    return true;
                }
                else
                    return false;
            }

            // Else, create temporary boxes to further check what exists in AddBoxXandY
            xTempBoxWH = new BoxX(x);
            yTempBoxWH = new BoxY(y);

            if (xTempBoxWH.AddBoxXandY(boxBST, yTempBoxWH) == true)
            {
                // add to list
                return true;
            }
            else
                return false;

        }

        // A function to remove Box, creates new temp boxes with parameters passed, checks if X exists,
        // if it exists, procceeds to RemoveBoxY() Func that checks Y exists and removes there.
        public bool RemoveBox(double x, double y)
        {
            yTempBoxWH = new BoxY(y);
            xTempBoxWH = new BoxX(x);

            BoxX XTempBox = boxBST.IfNodeExists(xTempBoxWH); // Get X node if exists by X value
            if (XTempBox != null)
            {
                XTempBox.RemoveBoxY(yTempBoxWH); // Remove Y
                if(XTempBox.YTree.root == null) // If X tree root is null, delete X
                {
                    boxBST.Remove(XTempBox);
                }
                return true;
            }
            else
                return false; // will return if values are incorrect
        }
        #endregion

        #region Purchase Successful / Canceled funcs
        public void PurchaseSuccess(LinkedList<BoxY> list)
        {
            BoxX tempBoxX;
            BoxY tempBoxY;
            foreach (var item in list)
            {
                tempBoxX = new BoxX(item.XParent);

                tempBoxX = boxBST.IfNodeExists(tempBoxX);
                tempBoxY = tempBoxX.YTree.IfNodeExists(item);
                if (tempBoxY.Count == 0) // If count is 0, no more boxes left, Y can be removed
                {
                    tempBoxX.YTree.Remove(item);
                    ExpiryCheckList.Remove(tempBoxY);
                    if (tempBoxX.YTree.root == null) // Checks if X tree is empty
                        boxBST.Remove(tempBoxX);
                }
                else // If still count has some boxes left, just update parameters where needed
                {
                    tempBoxY.OnHoldCount = 0;
                    tempBoxY.LastPurchaseTime = DateTime.Now;
                    ExpiryCheckList.Remove(tempBoxY);
                    ExpiryCheckList.AddLast(tempBoxY);
                }
            }
            list.Clear();
        }

        // A function that returns boxes to warehouse if customer decides not to purchase them.
        // Uses the linked list of boxes added by the "FindGiftBox" func and re-adds them back to the tree.
        public void ReturnToWarehouse(LinkedList<BoxY> list)
        {
            foreach (var item in list)
            {
                item.Count += item.OnHoldCount; // Readd the "Onhold" boxes back to count
                item.OnHoldCount = 0;
            }
            normalSizeBoxList.Clear();
            biggerBoxesList.Clear();
        }
        #endregion

        #region Find Gift Box func

        // A function that searches for gift box/boxes for customer
        public int FindGiftBox(double x, double y, int amount, bool sizeLimit, LinkedList<BoxY> LLBoxY)
        {
            double maxX = x + x * maxPercentage; // Max percentage number for X
            double maxY = y + y * maxPercentage; // Max percentage number for Y

            // Run the function "amount" times the customer asked
            for (int i = 0; i < amount; i++)
            {
                bool giftBoxFound = FindGiftBox(x, y, sizeLimit, maxX, maxY, y, LLBoxY);

                if (giftBoxFound == false && i == 0)// if returns false and i = 0, no boxes available, none found
                    return 0;
                else if (giftBoxFound == false && i <= amount - 1) // Found some boxes but not everything, return the amount left
                    return amount - i;
                else if (giftBoxFound == true) // means that at least one box was found and returned well
                    continue;
            }
            return -1; // If all is good, all boxes were found
        }

        // x and y, are the requested size,
        // sizelimit is to limit the percentage,
        // maxX and maxY are calculated with percentage to not exceed, originalY is the requested Y in order to proceed with new X,
        // And it asks for list to seperate the boxes found within the perecentage, and those who exceed (per user request)
        private bool FindGiftBox(double x, double y, bool sizeLimit, double maxX, double maxY, double originalY, LinkedList<BoxY> list)
        {
            // Recursion / function stop condition, if X tree is empty, or the X / Y values excceed the maximum percentage
            if (boxBST.root == null || sizeLimit == true && (x > maxX || y > maxY))
            {
                return false;
            }

            xTempBoxWH = new BoxX(x);
            yTempBoxWH = new BoxY(y);

            // First check if X exist, if yes, check for Y and get it to the temp boxes above
            xTempBoxWH = boxBST.IfNodeExists(xTempBoxWH);
            if (xTempBoxWH != null)
                yTempBoxWH = xTempBoxWH.YTree.IfNodeExists(yTempBoxWH);

            if (xTempBoxWH != null && yTempBoxWH != null && yTempBoxWH.Count > 0) // If exist and Y box count is not zero add to temp lists and lower count
            {
                yTempBoxWH.Count--;
                if (list.Last != null) // Checks if same box were added last to list, if yes, update the "On hold count" (how many boxes are currently offered)
                {
                    if (list.Last.Value == yTempBoxWH)
                    {
                        list.Last.Value.OnHoldCount++;
                        return true;
                    }
                }
                // If not last, means it's not in the list yet, add to last and update on hold count
                list.AddLast(yTempBoxWH);
                list.Last.Value.OnHoldCount++;
                return true;
            }


            else if (xTempBoxWH != null && yTempBoxWH != null) // if goes here, means that Y count is zero
            {
                yTempBoxWH = xTempBoxWH.YTree.FindNextBiggerNode(yTempBoxWH); // Search for next Y Node

                while (yTempBoxWH != null) // Run through while until Y with count higher than 0 is found, or null
                {
                    if (yTempBoxWH.YValue > maxY && sizeLimit == true) // If Y excceeds max percentage while sizelimit is on (true)
                    {
                        yTempBoxWH = null;
                        break;
                    }

                    if (yTempBoxWH.Count > 0) // If it enters to if statment, Y was found
                    {
                        yTempBoxWH.Count--;

                        if (list.Last != null)
                        {
                            if (list.Last.Value == yTempBoxWH)
                            {
                                list.Last.Value.OnHoldCount++;
                                return true;
                            }
                        }
                        list.AddLast(yTempBoxWH);
                        list.Last.Value.OnHoldCount++;
                        return true;
                    }
                    else // means count is 0, keep searching
                    {
                        yTempBoxWH = xTempBoxWH.YTree.FindNextBiggerNode(yTempBoxWH);
                    }
                }
                // If it reaches here, Y was null (no fitting Y in the X tree), look for next X in recursion
                if (yTempBoxWH == null)
                {
                    xTempBoxWH = boxBST.FindNextBiggerNode(xTempBoxWH); // Get next X and enter the new X in the function
                    if (xTempBoxWH != null)
                    {
                        if (xTempBoxWH.XValue > maxX && sizeLimit == true) // If X has excceeded the limit and limit is on (true), return false to not waste memory on func
                            return false;
                        else
                            return FindGiftBox(xTempBoxWH.XValue, originalY, sizeLimit, maxX, maxY, originalY, list);
                    }
                    else
                        return false;
                }// If returns False, no more X
            }

            else if (xTempBoxWH != null && yTempBoxWH == null) // If X found and Y is null
            {
                yTempBoxWH = new BoxY(y);
                yTempBoxWH = xTempBoxWH.YTree.FindNextBiggerNode(yTempBoxWH); // Get next bigger Y Box

                if (yTempBoxWH == null) // If not found, look for next X
                {
                    xTempBoxWH = boxBST.FindNextBiggerNode(xTempBoxWH);
                     // If X has excceeded the limit and limit is on (true), return false to not waste memory on func
                     // Or if no X found                       
                    if (xTempBoxWH == null || xTempBoxWH.XValue > maxX && sizeLimit == true)
                        return false;
                    else
                        return FindGiftBox(xTempBoxWH.XValue, originalY, sizeLimit, maxX, maxY, originalY, list); // new X found, search again in function with new X
                }
                else if (yTempBoxWH.Count == 0) // If Y found, but Count is 0 (means all boxes are OnHold)
                {
                    while (yTempBoxWH != null)
                    {
                        if (yTempBoxWH.YValue > maxY && sizeLimit == true) // If Y excceeds max percentage while sizelimit is on (true)
                        {
                            yTempBoxWH = null;
                            break;
                        }
                        if (yTempBoxWH.Count > 0) // If all is well, add
                        {
                            yTempBoxWH.Count--;
                            if (list.Last != null)
                            {
                                if (list.Last.Value == yTempBoxWH)
                                {
                                    list.Last.Value.OnHoldCount++;
                                    return true;
                                }
                            }
                            list.AddLast(yTempBoxWH);
                            list.Last.Value.OnHoldCount++;
                            return true;
                        }

                        else // means count is 0
                        {
                            yTempBoxWH = xTempBoxWH.YTree.FindNextBiggerNode(yTempBoxWH);
                        }
                    }
                    // will reach here, if no Y was found (Y was null) look for next X
                    xTempBoxWH = boxBST.FindNextBiggerNode(xTempBoxWH); 
                    if (xTempBoxWH == null || xTempBoxWH.XValue > maxX && sizeLimit == true)
                        return false;
                    else
                        return FindGiftBox(xTempBoxWH.XValue, originalY, sizeLimit, maxX, maxY, originalY, list);
                }
                else if (yTempBoxWH != null && yTempBoxWH.Count > 0) // If everything alright, pass limit validation, and if OK add.
                {

                    if(yTempBoxWH.YValue > maxY && sizeLimit == true) // Check validation for size limit, if excceeds go to next X
                    {
                        xTempBoxWH = boxBST.FindNextBiggerNode(xTempBoxWH); 
                        if (xTempBoxWH == null)
                            return false;
                        else
                            return FindGiftBox(xTempBoxWH.XValue, originalY, sizeLimit, maxX, maxY, originalY, list);
                    }

                    yTempBoxWH.Count--;

                    if (list.Last != null)
                    {
                        if (list.Last.Value == yTempBoxWH)
                        {
                            list.Last.Value.OnHoldCount++;
                            return true;
                        }
                    }

                    list.AddLast(yTempBoxWH);
                    list.Last.Value.OnHoldCount++;
                    return true;
                }
                else
                    return false;
            }

            else if (xTempBoxWH == null) // If no X was found at start of Function
            {
                xTempBoxWH = new BoxX(x);
                xTempBoxWH = boxBST.FindNextBiggerNode(xTempBoxWH); 
                if (xTempBoxWH == null || xTempBoxWH.XValue > maxX && sizeLimit == true) // check limit
                    return false;
                else
                    return FindGiftBox(xTempBoxWH.XValue, originalY, sizeLimit, maxX, maxY, originalY, list);
            }
            return false;
        }

        #endregion


        #region CheckExpiryDate(for Timer.Elapsed), ShowAllBoxes, and MaxBoxes function that loads from .json
        // Function for Timer.Elapsed, each x seconds perform this
        internal static void CheckExpiryDate(Warehouse wh)
        {
            BoxX tempBoxXExp;
            BoxY tempBoxYExp;
            if (expiryCheckList.First == null)
                return;
            else if (DateTime.Now.Subtract(expiryCheckList.First.Value.LastPurchaseTime) >= TimeSpan.FromSeconds(600))
            {
                tempBoxXExp = new BoxX(ExpiryCheckList.First.Value.XParent);
                tempBoxXExp = wh.boxBST.IfNodeExists(tempBoxXExp);
                tempBoxYExp = tempBoxXExp.YTree.IfNodeExists(ExpiryCheckList.First.Value);

                if (tempBoxYExp.OnHoldCount == 0) // Will be higher than 0 if in middle of purchase
                {
                    tempBoxXExp.YTree.Remove(ExpiryCheckList.First.Value);
                    expiryCheckList.RemoveFirst();
                }

                else
                    return;

            }
        }

        // A function that shows all the boxes in warehouse, adds everything to a LinkedList and displays.
        public LinkedList<string> ShowAllBoxes()
        {
            boxXList.Clear();
            boxBST.ScanInOrder(s => boxXList.AddLast(s));
            LinkedList<string> allBoxes = new LinkedList<string>();

            foreach (var item in boxXList)
            {
                item.YTree.ScanInOrder(s => allBoxes.AddLast($"{item.XValue},{s.YValue} Amount: {s.Count}\nLast Purchase Date: {s.LastPurchaseTime}\n"));
            }
            return allBoxes;
        }

        // Gets the MaxBoxes number from .json file in RelativePath folder (.json is located where the exe)
        public static int MaxBoxes()
        {

            var dir = AppDomain.CurrentDomain.BaseDirectory;

            var configPath = dir + "\\config.json";

            //var configPath = @"C:\Config\config.json";

            // Read all text from file
            var serialized = File.ReadAllText(configPath);

            // Deserialize into a `Config` object
            var config = JsonSerializer.Deserialize<ConfigClass>(serialized);

            return config.configNum;
        }

        internal class ConfigClass
        {
            public int configNum { get; set; }

        }

        #endregion
    }

}



