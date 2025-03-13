using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DungeonExplorer
{
    public class Player : Intro
    {
        public string Name { get; private set; }
        public List<string> Inventory { get; private set; }
        public Player(string name) 
        {
            Name = name;
            Inventory = new List<string>();
        }
        public void PickUpItem(string item)
        {
                Inventory.Add(item);
                PrintLetterByLetter($"{item}", 50);
                Thread.Sleep(2500);
        }
        public string InventoryContents()
        {
            return string.Join(", ", Inventory);
        }
    }
}