using System;
using System.Collections.Generic;

namespace TextRPG_24_J
{
    public class Player
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; set; }
        public int BaseAttack { get; set; }
        public float CritRate { get; set; } = 0.15f; // 치명타 확률
        public float CritMultiplier { get; set; } = 1.6f; // 치명타 배율
        public float Evasion { get; set; } = 0.1f; // 회피 확률
        public int BaseDefense { get; set; }
        public int MaxHp { get; set; }     // 최대 체력
        public int HP { get; set; }        // 현재 체력
        public int MaxMana { get; set; }   // 최대 마나
        public int CurrentMana { get; set; } // 현재 마나
        public int Gold { get; set; }      // 골드
        public int Exp { get; set; } = 0;  // 현재 경험치

        public List<Skills> SkillList { get; set; }
        public List<Item> EquippedItems { get; } = new List<Item>();

        public Player(string name, string job, int level, int attack, int defense, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            BaseAttack = attack;
            BaseDefense = defense;
            MaxHp = hp;
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

        // 실제 공격력 계산 (아이템 포함)
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

        // 실제 방어력 계산 (아이템 포함)
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

        // 경험치 누적 후 레벨업 처리
        public void CheckLevelUp()
        {
            while (true)
            {
                int needExp = (Level - 1) * 40;
                if (needExp <= 0) needExp = 40;

                if (Exp < needExp)
                    break;

                Exp -= needExp;
                Level++;
                MaxHp += 5;            // 최대체력 +5
                BaseAttack += 2;       // 공격력 +2
                Console.WriteLine($"▶ 레벨업!! Lv.{Level}, 최대체력 {MaxHp}, 공격력 {Attack} 달성!");
            }
        }

        // 상태창 출력
        public void ShowStatus()
        {
            Console.Clear();
            Console.WriteLine("\n--- 상태 보기 ---\n");
            Console.WriteLine($"Lv.{Level:D2} {Name} ({Job})");
            Console.WriteLine($"HP: {HP}/{MaxHp}");
            Console.WriteLine($"MP: {CurrentMana}/{MaxMana}");
            Console.WriteLine($"공격력: {Attack}");
            Console.WriteLine($"방어력: {Defense}");
            Console.WriteLine($"치명타 확률: {(CritRate * 100):F0}%");
            Console.WriteLine($"치명타 피해: {(CritMultiplier * 100):F0}%");
            Console.WriteLine($"회피율: {(Evasion * 100):F0}%");
            Console.WriteLine($"Gold: {Gold} G");
            int nextExp = (Level - 1) * 40;
            if (nextExp <= 0) nextExp = 40;
            Console.WriteLine($"EXP: {Exp}/{nextExp}\n");

            Console.WriteLine("0. 나가기");
            Console.Write(">> ");
            while (Console.ReadLine()?.Trim() != "0")
            {
                Console.WriteLine("잘못된 입력입니다. 0을 눌러 나가세요.");
                Console.Write(">> ");
            }
        }

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
    }
}
