using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool roomSearched = false;
        private bool roomStudied = false;

        public Game()
        {
            _GameCommands = new GameCommands();
            intro = new Intro();
            currentRoom = new Room();
        }

        // Recieving a random room intro, using PrintLetterByLetter method for a consistant game flow
        public void NewAreaIntro()
        {
            Debug.Assert(currentRoom != null, "currentRoom Should not be null");
            intro.PrintLetterByLetter(currentRoom.GetRoomIntro(), 50);
            Thread.Sleep(2000);
            Console.Clear();
        }

        // Method to introduce the player to the Room they have entered
        public void IntroduceNewRoom()
        {
            Debug.Assert(currentRoom != null, "currentRoom Should not be null");
            intro.PrintLetterByLetter("You have entered:", 50);
            intro.PrintLetterByLetter($" {currentRoom.GetTitle()}", 200);
            Thread.Sleep(2000);
            Console.Clear();
        }

        // Explore function utilising _game_Commands.Choice
        public bool Explore()
        {
            Console.Clear();
            intro.PrintLetterByLetter("(1). Study this room\n", 50);
            intro.PrintLetterByLetter("(2). Search this room for items\n\n", 50);
            return _GameCommands.Choice("Make your choice: ", "1", "2");
        }

        // Rather than a description, a study feature gives a more realistic "Dungeon Explorer" feel
        public void StudyRoom()
        {
            if (!roomStudied)
            {
                Console.Clear();
                intro.PrintLetterByLetter($"{currentRoom.GetDescription()}", 50);
                Thread.Sleep(2000);
                roomStudied = true;
            }
            else
            {
                intro.PrintLetterByLetter("You have already studied this room.", 50);
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
                    string item = RoomRng.GetRandomItem();
                    roomSearched = true;

                    if (item != null && item != "null")
                    {
                        intro.PrintLetterByLetter("You search the room thoroughly and found: ", 50);
                        return item;
                    }
                    else
                    {
                        intro.PrintLetterByLetter($"No matter how hard you look you cannot seem to find any new items, perhaps you have completed the collection", 50);
                        Thread.Sleep(2000);
                        return "";
                    }
                }
                else
                {
                    intro.PrintLetterByLetter("You search the room from top to bottom but find nothing", 50);
                    Thread.Sleep(2000);
                    return "";
                }
            }
            else
            {
                intro.PrintLetterByLetter("You have already searched this room.", 50);
                Thread.Sleep(2000);
                return "";
            }
        }

        public void ViewInventory()
        {
            if (player.Inventory.Count == 0)
            {
                return;
            }

            intro.PrintLetterByLetter("Would you like to view the items you have collected?\n\n", 25);
            intro.PrintLetterByLetter("(Y) Yes\n", 50);
            intro.PrintLetterByLetter("(N) No\n\n", 50);
            intro.PrintLetterByLetter("Make your choice: ", 50);
            bool view = _GameCommands.Choice("", "Y", "N" );

            if (view)
            {
                Console.Clear();
                string inventoryContents = player.InventoryContents();
                intro.PrintLetterByLetter($"Item(s) found: {inventoryContents}", 25);
                Thread.Sleep(2000);
                Console.Clear();
            }
            else
            {
                Console.Clear();
                // Continue to next room 
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

                    bool roomLoop = true;

                    while (roomLoop) // create a loop in which the player can now explore through the different rooms
                    {
                        currentRoom = new Room(); // Get a new room along with description
                        if (currentRoom.GetTitle() == "null")
                        {
                            intro.displayOutro();
                            if (_GameCommands.Endgame())
                            {
                                break;
                            }
                        }

                        NewAreaIntro(); // Random intro to the new room
                        IntroduceNewRoom(); // Tell the player which room they have entered
                        roomStudied = false;
                        roomSearched = false;
                        while (!roomStudied || !roomSearched)
                        {
                            bool action = Explore();
                            Debug.Assert(action == true || action == false, "Action should be either true or false");

                            if (action)
                            {
                                StudyRoom();
                                roomStudied = true;

                            }
                            else if (!action)
                            {
                                string item = SearchRoom();
                                if (item == "")
                                {
                                    roomSearched = true;
                                }
                                else
                                {
                                    player.PickUpItem(item);
                                    roomSearched = true;
                                }
                            }
                        }
                        Console.Clear();
                        ViewInventory();
                    }
                }
            }
        }
    }
}

/* note for reviewers, This code is currently incomplete and still has a few more methods to implement that can be seen in room.cs such as
 a mostly functioning rng generator for items and the type of item! repeat rooms and items will be removed in the final product so if
you have any suggestions on how to impliment this efficiently that would be greatly appriciated */