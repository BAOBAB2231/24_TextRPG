
namespace TextRPG_24_J
{
   public enum ItemType
    {
        Weapon,
        Armor,
        Accessory
    }

    public class Item
    {
        public string Name { get; }
        public string Description { get; }
        public string StatText { get; }
        public ItemType Type { get; }
        public bool IsEquipped { get; set; }

        public string StatType { get; }   
        public int StatValue { get; }     

        public Item(string name, string statText, string description, ItemType type, bool isEquipped = false)
        {
            Name = name;
            StatText = statText;
            Description = description;
            Type = type;
            IsEquipped = isEquipped;

            var parts = statText.Split(' ');
            StatType = parts[0];
            StatValue = int.Parse(parts[1].Replace("+", ""));
        }

        public void Display(int index = -1)
        {
            string equippedTag = IsEquipped ? "[E]" : "   ";
            string prefix = index >= 0 ? $"{index}. " : "- ";
            Console.WriteLine($"{prefix}{equippedTag}{Name.PadRight(15)} | {StatText} | {Description}");
        }
    }
}
