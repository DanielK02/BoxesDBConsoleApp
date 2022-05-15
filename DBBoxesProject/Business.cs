using DataStructures;
using System;
using System.Timers;

namespace DBBoxesProject
{
    public class Business
    {
        static void Main(string[] args) 
        {
            // Start of program, initialize a warehouse that will be used in all and go to MainMenu
            Warehouse warehouse = new Warehouse();
            
            MainMenu(warehouse);
        }

        static void MainMenu(Warehouse warehouse)
        {
            TimerClass timer = new TimerClass(5000);
            timer.timerWH = warehouse; // Need to send WareHouse data for Elapsed to do it's functionality as intended
            timer.Start();
            bool startUpLoop = true; // Bool for first loop asking user input.

            Console.WriteLine("Welcome to box company, please select user type:");
            Console.WriteLine("1. Buyer");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. Exit");

            while (startUpLoop)
            {
                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("Hello buyer!"); // Goes to Buyer Menu
                        timer.Stop();
                        startUpLoop = false;
                        BuyerMenu(warehouse);
                        break;
                    case "2":
                        Console.WriteLine("Hello business"); // Goes to Warehouse / Business menu
                        startUpLoop = false;
                        WarehouseMenu(warehouse);
                        break;
                    case "3":
                        timer.Stop(); // Exits the program
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Wrong input, retry.");
                        break;
                }
            }
        }

        #region Buyer Menu region
        private static void BuyerMenu(Warehouse wh)
        {
            
            bool correctInput = false;
            bool userInput = false;
            double x;
            double y;
            int amount;

            Console.WriteLine("Welcome to our gift box shop! \nPlease enter the size of the gift box needed.");
            while (correctInput == false)
            {
                Console.WriteLine("\nPlease enter X size (Base width):");
                bool xCorrect = double.TryParse(Console.ReadLine(), out x); // checks correct input if not, starts asking from beginning
                if (xCorrect == false || x <= 0)
                {
                    Console.WriteLine("Wrong input, please input positive numbers.");
                    continue;
                }

                Console.WriteLine("\nPlease enter Y size (Height):");
                bool yCorrect = double.TryParse(Console.ReadLine(), out y); // checks correct input if not, starts asking from beginning
                if (yCorrect == false || y <= 0)
                {
                    Console.WriteLine("Wrong input, please input positive numbers.");
                    continue;
                }

                Console.WriteLine("\nHow many boxes do you need? Please enter the amount:");
                bool amountCorrect = int.TryParse(Console.ReadLine(), out amount); // checks correct input if not, starts asking from beginning
                if (amountCorrect == false || amount <= 0)
                {
                    Console.WriteLine("\nWrong input, please input positive numbers.\n");
                    continue;
                }

                int boxAvailable = wh.FindGiftBox(x, y, amount, true, wh.NormalSizedBoxes); // Go to search for box


                switch (boxAvailable)
                {
                    case -1: // Means that all the amount of boxes had been found within the limits
                        Console.WriteLine();
                        foreach (var item in wh.NormalSizedBoxes)
                        {
                            Console.WriteLine($"{item.XParent},{item.YValue} Amount: {item.OnHoldCount}");
                        }
                        break;
                    case 0:
                        NoBoxesAtAll(wh, x, y, amount); // No Boxes found at all (within limits), offers to find Larger sizes boxes with no limit
                        break;
                    default:        // In case less boxes returned than requested, offers to find larger sized boxes with no limit
                        Console.WriteLine();
                        foreach (var item in wh.NormalSizedBoxes) // ConsoleWrite all boxes
                        {
                            Console.WriteLine($"{item.XParent},{item.YValue} Amount: {item.OnHoldCount}");
                        }
                        SomeBoxes(wh, boxAvailable, x, y); // Go here if not all amount was found
                        userInput = true;
                        break;
                }
                correctInput = true;
            }
            
            while (userInput == false)
            {
                Console.WriteLine("\nWould you like to purchase the boxes?\n1.Yes \n2.No");
                switch (Console.ReadLine())
                {
                    case "1":
                        wh.PurchaseSuccess(wh.NormalSizedBoxes);
                        Console.WriteLine("\nPurchase successful\n");
                        userInput = true;
                        break;
                    case "2":
                        wh.ReturnToWarehouse(wh.NormalSizedBoxes);
                        Console.WriteLine("\nReturned to warehouse\n");
                        userInput = true;
                        break;
                    default:
                        Console.WriteLine("\nWrong input!\n");
                        break;
                }
            }
            MainMenu(wh);
        }

        
        private static void SomeBoxes(Warehouse wh, int amountLeft, double x, double y) // Function for when only some boxes are returned from search
        {
            int availableBoxes = 0;
            bool checkPurchaseInput = true;
            Console.WriteLine("\nWould you like to purchase current boxes amount, or looks for further larger boxes?\n1.Purchase current \n2.Add larger boxes\n3.Cancel Purchase");
            switch (Console.ReadLine())
            {
                case "1":
                    wh.PurchaseSuccess(wh.NormalSizedBoxes);
                    Console.WriteLine("\nPurchase successful\n");
                    return;
                case "2":
                    availableBoxes = wh.FindGiftBox(x, y, amountLeft, false, wh.LargerSizeBoxes);
                    break;
                case "3":
                    Console.WriteLine("\nPurchase canceled\n");
                    return;
                default:
                    Console.WriteLine("\nWrong input\n");
                    SomeBoxes(wh, amountLeft, x, y);
                    break;
            }

            foreach (var item in wh.LargerSizeBoxes)
            {
                Console.WriteLine($"{item.XParent},{item.YValue} Amount: {item.OnHoldCount}");
            }

            if (availableBoxes == -1 || availableBoxes != 0)
            {
                Console.WriteLine("\nWould you like to purchase all the boxes? \n1.Yes\n2.No");
                while(checkPurchaseInput == true)
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            wh.PurchaseSuccess(wh.NormalSizedBoxes);
                            wh.PurchaseSuccess(wh.LargerSizeBoxes);
                            Console.WriteLine("\nPurchase successful");
                            return;
                        case "2":
                            Console.WriteLine("\nPurchase canceled");                           
                            return;
                        default:
                            Console.WriteLine("\nWrong input");
                            break;
                    }
                    checkPurchaseInput = false;
                }               
            }
            else
            {
                Console.WriteLine("No additional boxes found, purchase the normal sized boxes? \n1.Yes\n2.No");
                while (checkPurchaseInput == true)
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            wh.PurchaseSuccess(wh.NormalSizedBoxes);
                            Console.WriteLine("\nPurchase successful");
                            return;
                        case "2":
                            Console.WriteLine("\nPurchase canceled");
                            return;
                        default:
                            Console.WriteLine("\nWrong input");
                            break;
                    }
                    checkPurchaseInput = false;
                }
              
            }
            return;
        }

        private static void NoBoxesAtAll(Warehouse wh, double x, double y, int amount) // Function when no boxes at all return from search, asks the user to search without limit
        {
            Console.WriteLine("\nNo boxes found for the size requested, would you like bigger boxes than requested?\n1.Yes \n2.No");
            switch (Console.ReadLine())
            {
                case "1":
                    wh.FindGiftBox(x, y, amount, false, wh.LargerSizeBoxes);
                    foreach (var item in wh.LargerSizeBoxes)
                    {
                        Console.WriteLine($"{item.XParent},{item.YValue} Amount: {item.OnHoldCount}");
                    }
                    break;
                case "2":
                    BuyerMenu(wh);
                    break;
                default:
                    BuyerMenu(wh);
                    break;
            }
        }

        #endregion


        #region Warehouse user menu + funcs
        public static void WarehouseMenu(Warehouse wh)
        {
            double xVal = 0;
            double yVal = 0;
            int boxesAmount = 0;

            Console.WriteLine("\nWelcome to the warehouse, what action would you like to take?");
            Console.WriteLine("1.Add Box \n2.Remove Box \n3.Show all stock\n4.Show all stock by last purchase date\n5.Show boxes that weren't purchased T time\n6.Main Menu");
            var warehouseInput = Console.ReadLine();

            switch (warehouseInput)
            {
                case "1":
                    AddMenu(wh, xVal, yVal); // Adds single box by X Y values
                    break;
                case "2":
                    RemoveMenu(wh, xVal, yVal, boxesAmount); // Removes amount of boxes by X Y values
                    break;
                case "3":
                    ShowBoxes(wh);
                    WarehouseMenu(wh);
                    break;
                case "4":
                    ShowExpiry();
                    WarehouseMenu(wh);
                    break;
                case "5":
                    ShowUnsoldBoxesByTime(wh);                
                    break;
                case "6":
                    MainMenu(wh);                   
                    break;
                default:
                    Console.WriteLine("Wrong input.");
                    WarehouseMenu(wh);
                    break;
            }
        }

        #region Add / Remove for warehouse
        private static void AddMenu(Warehouse wh, double xVal, double yVal) 
        {
            bool isMax;

            Console.WriteLine("Please enter X and Y value to Add \nPlease enter X value:");
            bool xValFlag = double.TryParse(Console.ReadLine(), out xVal);
            Console.WriteLine("Please enter Y value:");
            bool yValFlag = double.TryParse(Console.ReadLine(), out yVal);

            if (xVal <= 0 || yVal <= 0 || xValFlag == false || yValFlag == false)
            {
                Console.WriteLine("Wrong input!");
                AddMenu(wh, xVal, yVal);
            }
            else
            {
                isMax = wh.AddBox(xVal, yVal);
                if(isMax == false)
                    Console.WriteLine("Max boxes count, returned to supplier.");
                WarehouseMenu(wh);
            }

        }

        private static void RemoveMenu(Warehouse wh, double xVal, double yVal, int boxesAmount)
        {
            Console.WriteLine("Please enter X and Y value to remove \nPlease enter X value:");
            bool xValFlag = double.TryParse(Console.ReadLine(), out xVal);
            Console.WriteLine("Please enter Y value:");
            bool yValFlag = double.TryParse(Console.ReadLine(), out yVal);
            Console.WriteLine("Please enter amount to remove:");
            bool boxesAmountFlag = int.TryParse(Console.ReadLine(), out boxesAmount);

            if (xValFlag == false || yValFlag == false || boxesAmountFlag == false || xVal <= 0 || yVal <= 0 || boxesAmount <= 0)
            {
                Console.WriteLine("\nOne of the inputs were wrong, please input only positive numbers.\n");
                RemoveMenu(wh, xVal, yVal, boxesAmount);
            }


            for (int i = 0; i < boxesAmount; i++)
            {
                wh.RemoveBox(xVal, yVal);
            }

            WarehouseMenu(wh);

        }
        #endregion

        #region Show different data for warehouse funcs
        private static void ShowExpiry()
        {
            foreach (var item in Warehouse.ExpiryCheckList)
            {
                Console.WriteLine($"{item.XParent},{item.YValue} Amount: {item.Count} \nLast Purchase Date: {item.LastPurchaseTime}\n");
            }
        }

        private static void ShowBoxes(Warehouse wh)
        {
            foreach (var item in wh.ShowAllBoxes())
            {
                Console.WriteLine(item);
            }
        }

        private static void ShowUnsoldBoxesByTime(Warehouse wh)
        {

            int timeByUser = 0;
            bool userInput;
            Console.WriteLine("Please enter the time in seconds to see boxes that weren't sold more than T time:");
            userInput = int.TryParse(Console.ReadLine(), out timeByUser);
            if (userInput == true && timeByUser > 0)
            {
                foreach (var item in Warehouse.ExpiryCheckList)
                {
                    if (TimeSpan.FromSeconds(timeByUser) <= DateTime.Now.Subtract(item.LastPurchaseTime))
                    {
                        Console.WriteLine($"{item.XParent},{item.YValue} - Last Purchased:{DateTime.Now.Subtract(item.LastPurchaseTime).Seconds} Seconds Ago\n");
                    }
                    else
                        break;
                }
            }
            else
            {
                Console.WriteLine("Wrong input, please retry!");
                ShowUnsoldBoxesByTime(wh);
            }

            WarehouseMenu(wh);
        }
        #endregion
        #endregion
    }
}
