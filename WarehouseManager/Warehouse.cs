using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataStructures;


namespace WarehouseManager
{
    public class Warehouse
    {
        BST<BoxX> BoxBST = new BST<BoxX>();
        BoxX XtempBox;
        BoxY YtempBox;
        LinkedList<BoxX> BoxXList = new LinkedList<BoxX>();
        LinkedList<BoxY> BoxYList = new LinkedList<BoxY>();
        int configNumber = MaxBoxes(); //MaxBoxes(config);

        double maxPercentage = 0.25;

        private static int MaxBoxes()
        {

            var configPath = @"C:\Users\Daniel Krigel\Desktop\Config\config.json";

            // Read all text from file
            var serialized = File.ReadAllText(configPath);

            // Deserialize into a `Config` object
            var config = JsonSerializer.Deserialize<Config>(serialized);

            return config.configNum;
        }
        internal class Config
        {
            public int configNum { get; set; }

        }

        public Warehouse()
        {
            AddBox2(5, 6);
            AddBox2(3, 12);
            AddBox2(10, 7);
            AddBox2(5, 8);
            AddBox2(6, 9.5);
            AddBox2(8, 2);
            AddBox2(10, 7);
            AddBox2(3, 12);
            AddBox2(4, 2);
            AddBox2(2, 8);
            
        }

        public void ShowBoxes()
        {
            BoxXList.Clear();
            BoxBST.ScanInOrder(s => BoxXList.AddLast(s));

            foreach (var item in BoxXList)
            {
                item.YTree.ScanInOrder(s => Console.Write($"{item.XValue},{s.YValue} Amount: {s.Count}\n"));
            }
        }
        public void FindGiftBox(double x, double y)
        {
            if (BoxBST.root == null)
                Console.WriteLine("No boxes in warehouse!");

            XtempBox = new BoxX(x);
            YtempBox = new BoxY(y);

            BoxXList.Clear();
            BoxBST.ScanInOrder(s => BoxXList.AddLast(s));


            foreach (BoxX itemX in BoxXList)
            {
                if (itemX.XValue == x)
                {
                    itemX.YTree.ScanInOrder(yBox => BoxYList.AddLast(yBox));

                    foreach (BoxY itemY in BoxYList)
                    {
                        if (y == itemY.YValue)
                        {
                            Console.WriteLine($"Box Found {itemX.XValue},{itemY.YValue}");
                            itemY.Count--;
                            return;
                        }
                        else if (itemY.YValue > y && maxPercentageBox(itemY.YValue, y) == true)
                        {
                            Console.WriteLine($"Box Found {itemX.XValue},{itemY.YValue}");
                            itemY.Count--;
                            return;
                        }
                    }
                    // Else look for next Y node in size
                    // If not, go to next X

                }
                else if (itemX.XValue > x && maxPercentageBox(itemX.XValue, x) == true) // Should be first bigger BoxX
                {
                    itemX.YTree.ScanInOrder(yBox => BoxYList.AddLast(yBox));

                    foreach (BoxY itemY in BoxYList)
                    {
                        if (y == itemY.YValue)
                        {
                            Console.WriteLine($"Box Found {itemX.XValue},{itemY.YValue}");
                            itemY.Count--;
                            return;
                        }
                        else if (itemY.YValue > y && maxPercentageBox(itemY.YValue, y) == true)
                        {
                            Console.WriteLine($"Box Found {itemX.XValue},{itemY.YValue}");
                            itemY.Count--;
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("No box found.");
        }

        // First version
        public void AddBox(double x, double y)
        {

            // Check if BST is empty
            if (BoxBST.root == null)
            {
                BoxBST.Add(new BoxX(x));
                BoxBST.root.data.AddBoxY(new BoxY(y));
                return;
            }

            XtempBox = new BoxX(x);
            YtempBox = new BoxY(y);

            // Check if X exists
            BST<BoxX>.Node tempXNode = BoxBST.IfNodeExists(XtempBox); // O(log(n)) - O(n)
            // If not, create new X node and Y node inside the X tree
            if (tempXNode == null)
            {
                BoxBST.Add(new BoxX(x));
                // Get the Node from the func
                tempXNode = BoxBST.IfNodeExists(XtempBox); // O(log(n))
                tempXNode.data.AddBoxY(YtempBox);
                return;
            }
            // If exists, check for Y exists
            else if (tempXNode != null)
            {
                BST<BoxY>.Node tempYNode = tempXNode.data.IfYBoxExists(YtempBox); // O(log(n)) - O(n)
                // If Y not exists add Y
                if (tempYNode == null)
                    tempXNode.data.AddBoxY(YtempBox);
                else // If Y exists, up the count by 1
                {
                    tempYNode.data.Count++;
                }
            }
            return;
        }

        // Second version
        public void AddBox2(double x, double y)
        {
            // Check if BST is empty
            if (BoxBST.root == null)
            {
                BoxBST.Add(new BoxX(x));
                BoxBST.root.data.AddBoxY(new BoxY(y));
                return;
            }

            // Else, create temporary boxes to further check what exists in AddBoxXandY
            XtempBox = new BoxX(x);
            YtempBox = new BoxY(y);

            XtempBox.AddBoxXandY(BoxBST, YtempBox);

        }
        public bool maxPercentageBox(double boxItem, double requstedSize)
        {
            if(boxItem < requstedSize + requstedSize * maxPercentage)
            {
                return true;
            }
            return false;
        }

    }


}
// First check if X exists
// If X doesn't exist, create new node + add Y
// If X exists check if Y exists
// If Y doesn't exist create new node
// If exists Counts up




