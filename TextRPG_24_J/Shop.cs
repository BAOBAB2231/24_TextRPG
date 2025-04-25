using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace TextRPG_24_J
{
    public static class Shop
    {
        public static List<ShopItem> shopItems = new List<ShopItem>();

        static Shop()
        {
            
            shopItems.Add(new ShopItem("무쇠갑옷", "방어력 +5", "튼튼한 갑옷", ItemType.Armor, 500));
            shopItems.Add(new ShopItem("스파르타의 창", "공격력 +7", "전설의 창", ItemType.Weapon, 700));
            shopItems.Add(new ShopItem("낡은 검", "공격력 +2", "평범한 검", ItemType.Weapon, 200));

            shopItems.Add(new ShopItem("청동 갑옷", "방어력 +9", "튼튼한 청동으로 만들어진 갑옷입니다.", ItemType.Armor, 900));
            shopItems.Add(new ShopItem("강철 검", "공격력 +12", "예리한 강철로 만든 강한 검입니다.", ItemType.Weapon, 1200));
            shopItems.Add(new ShopItem("모험가의 망토", "방어력 +3", "가벼운 방어구로 이동이 편리합니다.", ItemType.Armor, 300));

            shopItems.Add(new ShopItem("흘러내리는 언어", "공격력 +30", "아무도 언어가 오염되리라는 생각을 못한 대가.", ItemType.Weapon, 3000));
            shopItems.Add(new ShopItem("넥타르", "공격력 +30", "신께서 이르시되 나의 피는 포도주요...", ItemType.Weapon, 3000));
            shopItems.Add(new ShopItem("적대적 양손잡이", "방어력 +30", "왼손은 오른손을 부정한다.", ItemType.Armor, 3000));
        }

        public static void Open(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=====  상점에 오신 것을 환영합니다! =====\n");
                Console.WriteLine("1. 무기 구매");
                Console.WriteLine("2. 방어구 구매");
                Console.WriteLine("3. 장비 판매");
                Console.WriteLine("4. 나가기\n");
                Console.Write("원하시는 메뉴를 선택해주세요: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowItemsByType(ItemType.Weapon, player);
                        break;
                    case "2":
                        ShowItemsByType(ItemType.Armor, player);
                        break;
                    case "3":
                        SellItems(player);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static void ShowItemsByType(ItemType type, Player player)
        {
            var filteredItems = shopItems.Where(item => item.Type == type).ToList();

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {type} 목록 =====\n");

                for (int i = 0; i < filteredItems.Count; i++)
                {
                    filteredItems[i].Display(i + 1);
                }

                Console.WriteLine("\n0. 뒤로가기");
                Console.WriteLine($"소지 골드: {player.Gold} G");
                Console.Write("\n구매할 아이템 번호를 입력해주세요: ");
                string input = Console.ReadLine();

                if (input == "0") return;

                if (int.TryParse(input, out int index) && index >= 1 && index <= filteredItems.Count)
                {
                    TryPurchase(filteredItems[index - 1], player);
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
        }

        static void TryPurchase(ShopItem item, Player player)
        {
            Console.WriteLine($"\n{item.Name}을(를) {item.Price}G에 구매하시겠습니까?");
            Console.WriteLine("1. 구매");
            Console.WriteLine("2. 취소");
            Console.Write(">> ");
            string confirm = Console.ReadLine();

            if (confirm.ToLower() == "1")
            {
                if (player.Gold >= item.Price)
                {
                    player.Gold -= item.Price;

                    
                    Inventory.Add(new Item(item.Name, item.StatText, item.Description, item.Type));
                    Console.WriteLine($"{item.Name}을(를) 구매했습니다!");
                }
                else
                {
                    Console.WriteLine("골드가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("구매를 취소했습니다.");
            }

            Console.WriteLine("\n아무 키나 누르면 계속합니다...");
            Console.ReadKey();
        }
        static void SellItems(Player player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== 장비 판매 =====\n");

                var inventory = Inventory.GetItems(); // 인벤토리에서 모든 아이템 가져옴

                if (inventory.Count == 0)
                {
                    Console.WriteLine("판매할 아이템이 없습니다.");
                    Console.WriteLine("\n0. 뒤로가기");
                    Console.ReadLine();
                    return;
                }

                for (int i = 0; i < inventory.Count; i++)
                {
                    var item = inventory[i];
                    Console.WriteLine($"{i + 1}. {item.Name} - {item.StatText} ({item.Type})");
                }

                Console.WriteLine("\n0. 뒤로가기");
                Console.Write("\n판매할 아이템 번호를 입력해주세요: ");
                string input = Console.ReadLine();

                if (input == "0") return;

                if (int.TryParse(input, out int index) && index >= 1 && index <= inventory.Count)
                {
                    var itemToSell = inventory[index - 1];
                    if (itemToSell.IsEquipped)
                    {
                        player.Unequip(itemToSell);
                    }

                    int sellPrice = itemToSell.StatValue * 90;
                    Inventory.Remove(itemToSell);
                    player.Gold += sellPrice;

                    Console.WriteLine($"{itemToSell.Name}을(를) {sellPrice}G에 판매했습니다.");
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }

                Console.WriteLine("\n아무 키나 누르면 계속합니다...");
                Console.ReadKey();
            }
        }

    }
}
