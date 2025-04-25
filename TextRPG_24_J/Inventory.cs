
using System;
using System.Collections.Generic;

namespace TextRPG_24_J
{


    public static class Inventory
    {
        static List<Item> items = new List<Item>();
        public static void AddItem(Item item)
        {
            items.Add(item);
        }

        public static void Remove(Item item)
        {
            items.Remove(item);
        }

        public static List<Item> GetItems()
        {
            return new List<Item>(items); 
        }
        static Inventory()
        {
            
        }

        public static void Show(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("인벤토리");
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

                Console.WriteLine("[아이템 목록]");
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Display(i + 1);  
                }

                Console.WriteLine("\n1. 장착 관리");
                Console.WriteLine("2. 나가기");
                Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    ManageEquipment(player);  
                }
                else if (input == "2")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }
        public static void Add(Item item)
        {
            items.Add(item);
        }
        
        public static void ManageEquipment(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(" 장착 관리\n");

                var equipableItems = items
                    .Where(item => item.Type == ItemType.Weapon || item.Type == ItemType.Armor)
                    .ToList();

                for (int i = 0; i < equipableItems.Count; i++)
                {
                    equipableItems[i].Display(i+1);
                }

                Console.WriteLine("\n원하는 아이템 번호를 입력하세요. (취소: 0)");
                Console.Write(">> ");
                string input = Console.ReadLine();

                if (input == "0") break;

                if (int.TryParse(input, out int index) && index >= 1 && index <= equipableItems.Count)
                {
                    var selected = equipableItems[index - 1];  

                   
                    foreach (var item in items)
                    {
                        if (item.Type == selected.Type && item.IsEquipped)
                            item.IsEquipped = false;
                    }

                    
                    selected.IsEquipped = true;

                    
                    if (!player.EquippedItems.Contains(selected))
                        player.EquippedItems.Add(selected);

                    Console.WriteLine($"\n'{selected.Name}'을(를) 장착했습니다!");
                    Console.WriteLine("스탯이 갱신되었습니다.");
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }

                Console.WriteLine("\n아무 키나 누르면 계속...");
                Console.ReadKey();
            }
        }
    }
}
