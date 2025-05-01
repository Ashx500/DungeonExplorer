using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using DungeonExplorer;

namespace DungeonExplorer
{
    public class GameCommands
    {
        public void ClearTerminal()
        {
            Console.Clear();
        }
        /* A method for choices within the game with 2 options, all possible logic is met including 
         * case sensitivity
         * Whitespace
         * empty strings
         */
        public bool Choice(string message, string choice1, string choice2)
        {
            Console.Write(message);
            while (true)
            {
                string choices = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(choices))
                {
                    Console.WriteLine("Input cannot be Empty.");
                    continue;
                }

                if (choices.Equals(choice1, StringComparison.OrdinalIgnoreCase)) return true;
                if (choices.Equals(choice2, StringComparison.OrdinalIgnoreCase)) return false;

                Console.WriteLine($"Invalid choice, Please select {choice1} or {choice2}");
            }
        }

        public bool IsValidInt(string input)
        {
            return int.TryParse(input, out _);
        }

        public int GetValidInteger(string prompt, string numberOfChoicesLow, string numberOfChoicesHigh)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out result))
                {
                    return result;
                }
                Console.WriteLine($"Invalid input, please enter {numberOfChoicesLow}-{numberOfChoicesHigh}");
            }
        }

        //Endgame method utilising the "Choice" method for a simple but effective way to end the game
        public bool Endgame()
        {
            bool gameStatus = Choice("\n\n(Y/N) Would you like to replay?", "Y", "N");

            if (!gameStatus)
            {
                Console.Write("Thanks for playing, I hope you enjoyed!");
                Environment.Exit(0);
            }

            return gameStatus;
        }
    }
    public class Intro
    {
        private GameCommands _GameCommands;
        public string Username { get; private set; }
        public Intro()
        {
            _GameCommands = new GameCommands();
        }
        public void Print(string Info)
        {
            Console.Write(Info);
        }



        // A simple method that utilizes Console.Write, for the letter by letter typing effect with a fractional delay depending on length of the text
        public void PrintLetterByLetter(string info, int delay)
        {
            foreach (char c in info)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
        }
        /* Creating a Display intro method that will utilize methods such as PrintLetterByLetter, we are also using thread.sleep for short delay's to allow
         * the user adequate time to read along with the story, and we will also be using _clear.Terminal For a higher quality reading experience,*/
        public void Displayintro()
        {
            PrintLetterByLetter("Hello.", 100);
            Thread.Sleep(1000);

            Print(".");
            Thread.Sleep(1000);

            PrintLetterByLetter(". *You whisper quietly*", 50);
            Thread.Sleep(1000);

            _GameCommands.ClearTerminal();
            PrintLetterByLetter("Where am i, what is my name? *you ask yourself*", 50);
            Thread.Sleep(1000);

            _GameCommands.ClearTerminal();
            PrintLetterByLetter("It.. Its.. Its... : ", 200);
            Username = Console.ReadLine();

            _GameCommands.ClearTerminal();
            PrintLetterByLetter($"Ah yes.. thats right, {Username}", 50);
            Thread.Sleep(1000);

            _GameCommands.ClearTerminal();
            Thread.Sleep(2000);
            PrintLetterByLetter("You seem to have woken up in damp decrepit dungeon, a solitary touch struggles to drown out the overwhelming darkness of the room.", 50);
            Thread.Sleep(2000);

            _GameCommands.ClearTerminal();
            PrintLetterByLetter("*You look down and are met with a pair of heavy handcuffs, however they're no longer attatched to your wrists*", 50);
            Thread.Sleep(2000);

            _GameCommands.ClearTerminal();
            PrintLetterByLetter("*You turn around and find yourself facing a wooden door.*", 50);
            Thread.Sleep(2000);

            _GameCommands.ClearTerminal();
            PrintLetterByLetter("You have a choice to make:\n\n", 50);
            PrintLetterByLetter("(1) Explore the Dungeon...\n", 50);
            PrintLetterByLetter("(2) Exit through the wooden door\n\n", 50);
        }
        public void displayOutro()
        {
            Console.Clear();
            PrintLetterByLetter("You continue your search trying to find new enterances wherever possible, however you conclude that you have searched every crevice of this dark mysterious place.", 50);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("You navigate your way back through all of the rooms you have searched and studied until you reach the room where you awoke.", 50);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("You peer up from the shakles still left of the cold floor ready to exit through the wooden door.", 50);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("However...", 300);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("To your horror...", 300);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("This door, your escape, no longer exists...", 200);
            Thread.Sleep(2000);

            Console.Clear();
            PrintLetterByLetter("Thank you for playing ", 100);
            PrintLetterByLetter("Dungeon Explorer.", 300);
        }

        // Game choice utilizing Choice method
        public bool YouHaveAChoice()
        {
            return _GameCommands.Choice("Make your choice: ", "1", "2");    
        }
        public bool HandleYouHaveAChoice()
        {
            bool choice = YouHaveAChoice();

            if (choice)
            {
                _GameCommands.ClearTerminal();
                return true;
            }
            else
            {
                _GameCommands.ClearTerminal();
                PrintLetterByLetter("You choose to try out the wooden door to find a cobbled stair case leading to the surface, you swifly make your exit.\n", 50);
                Thread.Sleep(500);
                PrintLetterByLetter("did you win or lose? that is up to your own philosophy of the term 'winning'.\n\n", 50);
                PrintLetterByLetter("The End.\n\n", 300);
                PrintLetterByLetter("Your trait: 'The Reluctant Adventurer'", 50);
                _GameCommands.Endgame();
                return false;
            }
        }
    }

    internal class Game
    {
        private Player player;
        private Room currentRoom;
        private Intro intro;
        private GameCommands _GameCommands;
        private static Random rand;
        private bool roomSearched = false;
        private bool roomStudied = false;
        private int xpTotal = 0;
        static volatile bool stopWheel = false;

        public Game()
        {
            _GameCommands = new GameCommands();
            intro = new Intro();
            currentRoom = new Room();
            rand = new Random();
        }

        // Recieving a random room intro, using PrintLetterByLetter method for a consistant game flow
        public void NewAreaIntro()
        {
            Debug.Assert(currentRoom != null, "currentRoom Should not be null");
            intro.PrintLetterByLetter(currentRoom.GetRoomIntro(), 50);
            Thread.Sleep(2000);
            Console.Clear();
        }

        public static List<int> ItemBoxGameEvenOddCalculation(int even)
        {
            List<int> evenNumbers = new List<int>();
            List<int> oddNumbers = new List<int>();

            int odd = 5 - even;

            while (evenNumbers.Count < even)
            {
                int num = rand.Next(1, 10);

                if (num % 2 == 0 && !evenNumbers.Contains(num))
                {
                    evenNumbers.Add(num);
                }
            }
            while (oddNumbers.Count < odd)
            {
                int num = rand.Next(1, 10);

                if (num % 2 != 0 && !oddNumbers.Contains(num))
                {
                    oddNumbers.Add(num);
                }
            }
            List<int> combinedNumbers = new List<int>();
            combinedNumbers.AddRange(evenNumbers);
            combinedNumbers.AddRange(oddNumbers);

            return combinedNumbers;
        }

        public static List<int> ItemBoxGameRandomIntegers(int RandomIntegerAmount)
        {
            List<int> randomNumbers = new List<int>();
            for (int i = 0; i < RandomIntegerAmount; i++)
            {
                randomNumbers.Add(rand.Next(1, 10)); 
            }
            return randomNumbers;
        }

        static void spinWheelResult(ref bool stopSlot)
        {
            List<int> randomNumbers2 = new List<int> { 0, 0, 0, 0, 0 };
            List<int> randomNumbers3 = new List<int> { 0, 0, 0, 0, 0 };

            while (!stopSlot)
            {
                List<int> randomNumbers1 = ItemBoxGameRandomIntegers(5);

                Console.Clear();
                Console.WriteLine("\nPress the spacebar to stop the wheel and claim your item!\n");
                Console.Write($"=======> Test your luck! <========|| ===> stars: ??? <=== ||\n");
                Console.Write($"                                  ||   -{randomNumbers1[0]}-{randomNumbers1[1]}-{randomNumbers1[2]}-{randomNumbers1[3]}-{randomNumbers1[4]}-        ||  1 Even Number = 1 star item             \n");
                Console.Write($"odd: [1,3,5,7,9] = loss           ||>>  {randomNumbers2[0]} {randomNumbers2[1]} {randomNumbers2[2]} {randomNumbers2[3]} {randomNumbers2[4]}  <<     ||  2 Even Numbers = 2 star item            \n");
                Console.Write($"Even: [2,4,6,8] = star            ||   -{randomNumbers3[0]}-{randomNumbers3[1]}-{randomNumbers3[2]}-{randomNumbers3[3]}-{randomNumbers3[4]}-        ||  3 Even Numbers = 3 star item            \n");
                Console.WriteLine($"                                                          ||  4 Even Numbers = 4 star item            ");
                Console.WriteLine($"                                                          ||  5 Even Numbers = 5 star item            ");
                Thread.Sleep(200);

                randomNumbers3 = randomNumbers2;
                randomNumbers2 = randomNumbers1;
            }
        }

        static void ItemBoxDisplay(List<int> PlayerNumbers, string item)
        {
            int boxWidth = 32;
            string DynamicItem = item.Length > boxWidth
                ? item.Substring(0, boxWidth)
                : item.PadLeft((boxWidth + item.Length) / 2).PadRight(boxWidth);

            int stars = PlayerNumbers.Count(num => num % 2 == 0);

            Console.Clear();
            Console.Write($"=======> Test your luck! <========||===> stars: {stars} <===||\n");
            Console.Write($"odd: [1,3,5,7,9] = loss           ||   - - - - - -    ||\n");
            Console.Write($"Even: [2,4,6,8] = star            ||>>  {PlayerNumbers[0]} {PlayerNumbers[1]} {PlayerNumbers[2]} {PlayerNumbers[3]} {PlayerNumbers[4]}  << ||\n");
            Console.Write($"+--------------------------------+||   - - - - - -    ||\n");
            Console.Write($"|         Item Recieved:         |||   Even: {stars}        ||    \n");
            Console.Write($"|                                |||   Odd: {5 - stars}         ||\n");
            Console.Write($"|{DynamicItem}|\n");
            Console.Write($"|            stars:{stars}             |                          \n");
            Console.Write($"|                                |                                \n");
            Console.Write($"+--------------------------------+                              \n\n");
            Console.Write($"Press Enter to return to the room menu.                             ");
        }

        static void ItemBoxMiniGame(int itemBoxRarity, string item)
        {
            List<int> PlayerNumbers = ItemBoxGameEvenOddCalculation(itemBoxRarity);
            Thread slotThread = new Thread(() => spinWheelResult(ref stopWheel));
            slotThread.Start();

            while (!stopWheel)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    if (!stopWheel)
                    {
                        stopWheel = true;
                        ItemBoxDisplay(PlayerNumbers, item);
                        Console.ReadLine();
                    }
                }
            }
            slotThread.Join();
        }
        // Method to introduce the player to the Room they have entered
        public void IntroduceNewRoom()
        {
            Debug.Assert(currentRoom != null, "currentRoom Should not be null");
            intro.PrintLetterByLetter("You have entered:", 25);
            intro.PrintLetterByLetter($" {currentRoom.GetTitle()}", 50);
            Thread.Sleep(3000);
            Console.Clear();
        }

        // Rather than a description, a study feature gives a more realistic "Dungeon Explorer" feel
        public void StudyRoom()
        {
            if (!roomStudied)
            {
                Console.Clear();
                intro.PrintLetterByLetter($"{currentRoom.GetDescription()}", 25);
                Thread.Sleep(2000);
                roomStudied = true;
            }
            else
            {
                intro.PrintLetterByLetter("You have already studied this room.", 25);
            }
        }
        // a function to check if items exist within a room and if they do these items can be found, better items take more luck to find with the rng feature
        public string SearchRoom()
        {
            Console.Clear();
            if (!roomSearched)
            {
                if (RoomRng.ContainsItems())
                {
                    (string item, int itemWeight) = RoomRng.GetRandomItem();
                    int itemBoxRarity = RoomRng.GetItemRating(itemWeight);
                    roomSearched = true;

                    if (item != null && item != "null")
                    {
                        intro.PrintLetterByLetter($"You search the room thoroughly and found an item box!", 25);
                        Thread.Sleep(2000);
                        ItemBoxMiniGame(itemBoxRarity, item); 
                        stopWheel = false;
                        return item;
                    }
                    else
                    {
                        intro.PrintLetterByLetter($"No matter how hard you look you cannot seem to find any new items, perhaps you have completed the collection", 25);
                        Thread.Sleep(3000);
                        return "";
                    }
                }
                else
                {
                    intro.PrintLetterByLetter("You search the room from top to bottom but find nothing", 25);
                    Thread.Sleep(3000);
                    return "";
                }
            }
            else
            {
                intro.PrintLetterByLetter("You have already searched this room.", 25);
                Thread.Sleep(3000);
                return "";
            }
        }

        // using a Queue we can store previously visited rooms 
        private Queue<string> previousRooms = new Queue<string>();
        public void SaveRoomTitles()
        {
            // here we recieve the title of the current room to save
            string Room = currentRoom.GetTitle();
            
            // then add this room to the queue
            previousRooms.Enqueue(Room);

            // using a smiple comparrison of the count of the current rooms we can keep a list of the last 3 specifically
            if (previousRooms.Count > 3)
            {
                // here we remove the 4th last visited room as we do not need to save this in our mini map
                previousRooms.Dequeue(); 
            }
        }

        // method to return a list of previous rooms making them iterable for our mini map
        public List<string> GetPreviousRooms()
        {
            return previousRooms.ToList();
        }

        public void PrintMiniMap()
        {
            Console.Clear();
            List<string> previous = GetPreviousRooms();

            Console.WriteLine("+------------------------------------+");
            Console.WriteLine("|               N0tES                |");
            Console.WriteLine("|           RecEnt r0oms -¬          |");
            Console.WriteLine("+-------------------------\\---------+");
            Console.WriteLine("                        <-'           ");

            for (int i = 0; i < previous.Count && i < 3; i++)
            {
                string location = "";

                switch (i)
                {
                    case 2:
                        location = "2 rooms ago    -->".PadRight(25);
                        break;
                    case 1:
                        location = "Just left      -->".PadRight(25);
                        break;
                    case 0:
                        location = "Currently here -->".PadRight(25);
                        break;
                }
                Console.WriteLine($"{location}{previous[i]}");

                if (i < previous.Count - 1 && i < 2)
                {
                    Console.WriteLine("                   |                  ");
                }
            }
            Console.WriteLine("\n\nPress enter to return to the room menu");
            Console.ReadLine();
        }

        public void ViewInventory()
        {
            if (player == null || player.Inventory == null || player.Inventory.Count == 0)
            {
                Console.Clear();
                intro.PrintLetterByLetter("You have no items in your inventory yet.", 25);

                Console.Write("\n\nPress enter to return to the room menu");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.Clear();
                string inventoryContents = player.InventoryContents();
                bool multiple = player.Inventory.Count >= 2;
                intro.PrintLetterByLetter($"Item{(multiple ? "s" : "")} found: {inventoryContents}", 25);

                Console.Write("\n\nPress enter to return to the room menu");
                Console.ReadLine();
                Console.Clear();
            }
        }

        public void Start()
        {
            bool replay = true;
            while (replay)
            {
                bool playing = true;
                while (playing)
                {
                    Console.Clear();
                    intro.Displayintro(); // Void function only shows text for intro
                    bool choice = intro.HandleYouHaveAChoice(); // possible end to the game, see intro.HandleYouHaveAChoice();
                    player = new Player(intro.Username);
                    if (!choice)
                    {
                        break;
                    }


                    // --------------------------------End of intro--------------------------------

                    xpTotal = 0;

                    int xpTotalLen = xpTotal.ToString().Length;
                    bool roomLoop = true;

                    while (roomLoop) // create a loop in which the player can now explore through the different rooms
                    {
                        Console.Clear();
                        currentRoom = new Room(); // Get a new room along with description                     
                        if (currentRoom.GetTitle() == "null")
                        {
                            intro.displayOutro();
                            if (_GameCommands.Endgame())
                            {
                                break;
                            }
                        }

                        NewAreaIntro(); // Plays a random text lead to the next room | No user input
                        IntroduceNewRoom(); // Introduces player to room | No user input
                        SaveRoomTitles(); // saves room titles for the mini map No user input


                        // logic overhaul, switched to switch case for room exploration allowing the inventory and mini map to be viewed at all times
                        // Added xp and tasks for the player to complete being: search and study the room rewarding xp, incentivising the player to do the tasks which allows for game progression


                        roomStudied = false;
                        roomSearched = false;

                        while (!roomStudied || !roomSearched)
                        {
                            Console.Clear();
                            Console.WriteLine($"[1] study this room   || >> Tasks to complete <<     ||  Total xp: {xpTotal,5}/1000 ||");
                            Console.WriteLine($"[2] search this room  ||                             ||                       ||");
                            Console.WriteLine($"[3] view inventory    || Search this room {(roomSearched ? 1 : 0)}/1 {(roomSearched ? "+ 50xp" : "> 50xp")} ||                       ||");
                            Console.WriteLine($"[4] view mini-map     || Study this room  {(roomStudied ? 1 : 0)}/1 {(roomStudied ? "+ 50xp" : "> 50xp")} ||                       ||");
                            int action = _GameCommands.GetValidInteger("\nMake your choice: ", "1", "4");

                            switch (action)
                            {
                                case 1:
                                    if (roomStudied == false)
                                    {
                                        xpTotal += 50;
                                    }
                                    StudyRoom();
                                    roomStudied = true;                  
                                    break;

                                case 2:
                                    if (roomSearched == false)
                                    {
                                        xpTotal += 50;
                                    }
                                    string item = SearchRoom();
                                    if (item == "")
                                    {
                                        // Do nothing
                                    }
                                    else
                                    {
                                        player.PickUpItem(item);
                                    }
                                    roomSearched = true;
                                    break;

                                case 3:
                                    ViewInventory();
                                    break;

                                case 4:
                                    PrintMiniMap();
                                    break;         
                            }
                        }
                    }
                }
            }
        }
    }
}