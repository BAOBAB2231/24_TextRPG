

namespace TextRPG_24_J
{
    public class ShopItem : Item
    {
        public int Price { get; }

        public ShopItem(string name, string statText, string description, ItemType type, int price)
            : base(name, statText, description, type)
        {
            Price = price;
        }

        public void Display(int index)
        {
            Console.WriteLine($"{index}. {Name.PadRight(15)} | {StatText} | {Description} | 가격: {Price} G");
        }
    }
}
