namespace TextRPG_24_J
{
    public class Player
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; set; }
        public int BaseAttack { get; set; }
        public float CritRate { get; set; } = 0.15f; // 치명타 확률 (기본값 : 15%)
        public float CritMultiplier { get; set; } = 1.6f; // 치명타 배율 (기본값 : 160%)
        public float Evasion { get; set; } = 0.1f; // 회피 확률 (기본값 : 10%)
        public int BaseDefense { get; set; }
        public int HP { get; set; }
        public int MaxMana { get; set; }  // 최대 마나

        public int CurrentMana { get; set; } // 현재 마나

        public int Gold { get; set; }

        public List<Skills> SkillList { get; set; }

        public List<Item> EquippedItems { get; } = new List<Item>();
        public void Equip(Item item)
        {
            if (!item.IsEquipped)
            {
                EquippedItems.Add(item);
                item.IsEquipped = true;
            }
        }

        public void Unequip(Item item)
        {
            if (item.IsEquipped)
            {
                EquippedItems.Remove(item);
                item.IsEquipped = false;
            }
        }
        public int Attack
        {
            get
            {
                int value = BaseAttack;
                foreach (var item in EquippedItems)
                {
                    if (item.StatType == "공격력")
                        value += item.StatValue;
                }
                return value;
            }
        }

        public int Defense
        {
            get
            {
                int value = BaseDefense;
                foreach (var item in EquippedItems)
                {
                    if (item.StatType == "방어력")
                        value += item.StatValue;
                }
                return value;
            }
        }

        public Player(string name, string job, int level, int attack, int defense, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            BaseAttack = attack;
            BaseDefense = defense;
            HP = hp;
            Gold = gold;
            MaxMana = 50;
            CurrentMana = MaxMana;
            SkillList = new List<Skills>();
            GetSkill();
        }

        void GetSkill()
        {
            SkillList = Skills.GetSkill(Attack);
        }

        public void ShowStatus()
        {
            Console.WriteLine("\n상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");
            Console.WriteLine($"Lv. {Level:D2}");
            Console.WriteLine($"{Name} ( {Job} )");
            Console.WriteLine($"공격력 : {Attack}");
            Console.WriteLine($"치명타 확률 : {(CritRate * 100):F0}%");
            Console.WriteLine($"치명타 피해 : {(CritMultiplier * 100):F0}%");
            Console.WriteLine($"회피율 : {(Evasion * 100):F0}%");
            Console.WriteLine($"방어력 : {Defense}");
            Console.WriteLine($"체  력 : {HP}");
            Console.WriteLine($"마  나 : {CurrentMana}");
            Console.WriteLine($"Gold : {Gold} G\n");

        }

    }
}
