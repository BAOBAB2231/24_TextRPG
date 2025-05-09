﻿using System.Threading;

namespace TextRPG_24_J
{


    public class Monster
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public int Gold { get; set; }
        public Item Dropitem { get; set; }
        public float CritRate { get; set; } = 0.15f; // 치명타 확률 (기본값 : 15%)
        public float CritMultiplier { get; set; } = 1.6f; // 치명타 배율 (기본값 : 160%)
        public float Evasion { get; set; } = 0.1f; // 회피 확률 (기본값 : 10%)
        public bool IsDead => Hp <= 0;

        public Monster(string name, int level, int hp, int atk, int gold, Item dropitem)
        {
            Name = name;
            Level = level;
            MaxHp = hp;
            Hp = hp;
            Attack = atk;
            Gold = gold;
            Dropitem = dropitem;
        }
    }

    public static class Battle
    {
        static List<Monster> monsters = new();
        static Random random = new();
        static QuestBoard? board;
        static QuestUI? quest;

        public static void SetQuestClass(QuestBoard _board, QuestUI _quest)
        {
            board = _board;
            quest = _quest;
        }

        public static void Show(Player player)
        {
            Console.Clear();
            Console.WriteLine("Battle!!\n");


            monsters.Clear();
            int count = random.Next(1, 5);
            for (int i = 0; i < count; i++)
            {
                int type = random.Next(3);
                switch (type)
                {
                    case 0:
                        monsters.Add(new Monster("미니언", 2, 15, 5, 50, Shop.shopItems[2]));
                        break;
                    case 1:
                        monsters.Add(new Monster("공허충", 3, 10, 9, 100, Shop.shopItems[5]));
                        break;
                    case 2:
                        monsters.Add(new Monster("대포미니언", 5, 25, 8, 200, Shop.shopItems[0]));
                        break;
                }
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Battle!!\n");


                for (int i = 0; i < monsters.Count; i++)
                {
                    Monster m = monsters[i];
                    if (m.IsDead)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"{i + 1} Lv.{m.Level} {m.Name}  Dead");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1} Lv.{m.Level} {m.Name}  HP {m.Hp}  ATK {m.Attack}");
                    }
                }

                Console.WriteLine("\n[내정보]");
                Console.WriteLine($"Lv.{player.Level} {player.Name} ({player.Job})");
                Console.WriteLine($"HP {player.HP}/100");
                Console.WriteLine($"MP {player.CurrentMana}/50\n");
                Console.WriteLine("1. 공격");
                Console.WriteLine("2. 스킬");
                Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                if (input == "1")
                {

                    Console.WriteLine("\n대상을 선택해주세요.");
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {monsters[i].Name} {(monsters[i].IsDead ? "(Dead)" : "")}");
                    }
                    Console.WriteLine("0. 취소");
                    Console.Write(">> ");
                    string select = Console.ReadLine();

                    if (select == "0") continue;
                    if (!int.TryParse(select, out int targetIndex) || targetIndex < 1 || targetIndex > monsters.Count)
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Monster target = monsters[targetIndex - 1];
                    if (target.IsDead)
                    {
                        Console.WriteLine("이미 죽은 몬스터입니다.");
                        Thread.Sleep(1000);
                        continue;
                    }

                    bool isEvaded = random.NextDouble() < target.Evasion; // 회피 판정
                    bool isCrit = random.NextDouble() < player.CritRate; // 치명타 판정

                    int variation = (int)Math.Ceiling(player.Attack * 0.1);
                    int finalDamage = random.Next(player.Attack - variation, player.Attack + variation + 1);

                    if (isCrit) // 치명타가 발동하면 최종 데미지에 치명타 배율을 곱한다.
                        finalDamage = (int)Math.Ceiling(finalDamage * player.CritMultiplier);

                    if (!isEvaded) // 회피가 발동하면 데미지를 주지 않음.
                        target.Hp -= finalDamage;

                    if (target.Hp < 0)
                    {
                        if (target.Name == "미니언" && board?.QuestList[0].IsQuestAccept == true)
                        {
                            if (quest != null)
                            quest.QuestMonster["미니언"]++;
                        }
                        target.Hp = 0;
                    }

                    if (!isEvaded) // 회피가 발동했을 시 문구 추가.
                        Console.WriteLine($"{target.Name}을(를) 공격했습니다! [데미지 : {finalDamage}] {(isCrit ? "- 치명타 공격!!" : "")}"); // 치명타 발동 시 문구 추가.
                    else
                        Console.WriteLine($"{target.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.");

                    Thread.Sleep(1000);


                    if (monsters.All(m => m.IsDead))
                    {
                        Console.Clear();
                        Console.WriteLine("Battle!! - Result\n");
                        Console.WriteLine("Victory\n");
                        Console.WriteLine($"던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n");
                        int recoveredMana = 10;
                        player.CurrentMana += recoveredMana;
                        if (player.CurrentMana > player.MaxMana)
                        {
                            player.CurrentMana = player.MaxMana;
                        }
                        Console.WriteLine($"MP를 {recoveredMana} 회복합니다.\n");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP 100 -> {player.HP}");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nMP {player.CurrentMana - recoveredMana} ->  {player.CurrentMana}\n");
                        int expGain = 0;
                        foreach (var m in monsters)
                        {
                            switch (m.Name)
                            {
                                case "미니언": expGain += 10; break;
                                case "공허충": expGain += 25; break;
                                case "대포미니언": expGain += 40; break;
                            }
                        }
                        //레벨업 
                        player.Exp += expGain;
                        int needExp;
                        needExp = player.Level - 1;
                        needExp = needExp * 40;
                        if (needExp <= 0)
                            needExp = 40;
                        Console.WriteLine($"획득 경험치 : {expGain} (현재 {player.Exp}/다음 {needExp})");
                        player.CheckLevelUp();
                        DropItem dropItem = new DropItem();
                        dropItem.Reward(player, monsters);
                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        return;
                    }


                    for (int i = 0; i < monsters.Count; i++)
                    {
                        var m = monsters[i];
                        if (m.IsDead) continue;

                        isEvaded = random.NextDouble() < player.Evasion; // 회피 판정
                        isCrit = random.NextDouble() < m.CritRate; // 치명타 판정

                        int monVar = (int)Math.Ceiling(m.Attack * 0.1);
                        int rawDmg = random.Next(m.Attack - monVar, m.Attack + monVar + 1);


                        int actualDmg = rawDmg - player.Defense;

                        if (isCrit) // 치명타가 발동하면 최종 데미지에 치명타 배율을 곱한다.
                            actualDmg = (int)Math.Ceiling(actualDmg * m.CritMultiplier);

                        if (actualDmg < 0) actualDmg = 0;

                        int prevHp = player.HP;

                        if (!isEvaded) // 회피가 발동하면 데미지를 주지 않음.
                            player.HP -= actualDmg;

                        if (player.HP < 0) player.HP = 0;

                        Console.Clear();
                        Console.WriteLine("Battle!!\n");

                        if (!isEvaded) // 회피가 발동했을 시 문구 추가.
                        {
                            Console.WriteLine($"{m.Name}의 공격!\n{player.Name}을(를) 맞췄습니다.");

                            Console.WriteLine($"[데미지 : {actualDmg}] {(isCrit ? "- 치명타 공격!!" : "")} (원래 공격력: {rawDmg}, 방어력: {player.Defense})\n"); // 치명타 발동 시 문구 추가.
                            Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP {prevHp} -> {player.HP}\n");
                        }
                        else
                        {
                            Console.WriteLine($"{m.Name}의 공격!");
                            Console.WriteLine($"{player.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");
                        }

                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        if (player.HP <= 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Battle!! - Result\n");
                            Console.WriteLine("You Lose\n");
                            Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP 100 -> 0\n");
                            Console.WriteLine("0. 다음\n>> ");
                            Console.ReadLine();
                            return;
                        }
                    }
                }
                else if (input == "2")
                {
                    // 스킬 사용 로직 추가
                    Console.WriteLine("\n사용할 스킬을 선택해주세요.");
                    for (int i = 0; i < player.SkillList.Count; i++)
                    {
                        Skills skill = player.SkillList[i];
                        Console.WriteLine($"{i + 1}. {skill.Name} (MP: {skill.MPCost}) - {skill.Description}");
                    }
                    Console.WriteLine("0. 취소");
                    Console.Write(">> ");
                    string skillSelect = Console.ReadLine();

                    if (skillSelect == "0") continue;
                    if (!int.TryParse(skillSelect, out int skillIndex) || skillIndex < 1 || skillIndex > player.SkillList.Count)
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(1000);
                        continue;
                    }

                    Skills selectedSkill = player.SkillList[skillIndex - 1];

                    // Skills 클래스의 UseSkill 메서드 호출
                    bool skillUsed = Skills.UseSkill(selectedSkill, player, monsters, random);

                    if (!skillUsed) continue;

                    // 모든 몬스터가 죽었는지 확인
                    if (monsters.All(m => m.IsDead))
                    {
                        

                        Console.Clear();
                        Console.WriteLine("Battle!! - Result\n");
                        Console.WriteLine("Victory\n");
                        Console.WriteLine($"던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n");
                        int recoveredMana = 10;
                        player.CurrentMana += recoveredMana;
                        if (player.CurrentMana > player.MaxMana)
                        {
                            player.CurrentMana = player.MaxMana;
                        }
                        Console.WriteLine($"MP를 {recoveredMana} 회복합니다.\n");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP 100 -> {player.HP}");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nMP {player.CurrentMana - recoveredMana} ->  {player.CurrentMana}\n");
                        int expGain = 0;
                        foreach (var m in monsters)
                        {
                            switch (m.Name)
                            {
                                case "미니언": expGain += 10; break;
                                case "공허충": expGain += 25; break;
                                case "대포미니언": expGain += 40; break;
                            }
                        }
                        // 레벨업

                        player.Exp += expGain;
                        int needExp;
                        needExp = player.Level - 1;
                        needExp = needExp * 40;
                        if (needExp <= 0)
                            needExp = 40;
                        Console.WriteLine($"획득 경험치 : {expGain} (현재 {player.Exp}/다음 {needExp})");
                        player.CheckLevelUp();

                        DropItem dropItem = new DropItem();
                        dropItem.Reward(player, monsters);
                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        return;                                       


                    }

                    // 몬스터 턴
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        var m = monsters[i];
                        if (m.IsDead) continue;

                        bool isEvaded = random.NextDouble() < player.Evasion; // 회피 판정
                        bool isCrit = random.NextDouble() < m.CritRate; // 치명타 판정

                        int monVar = (int)Math.Ceiling(m.Attack * 0.1);
                        int rawDmg = random.Next(m.Attack - monVar, m.Attack + monVar + 1);
                        int actualDmg = rawDmg - player.Defense;

                        if (isCrit) // 치명타가 발동하면 최종 데미지에 치명타 배율을 곱한다.
                            actualDmg = (int)Math.Ceiling(actualDmg * m.CritMultiplier);

                        if (actualDmg < 0) actualDmg = 0;

                        int prevHp = player.HP;

                        if (!isEvaded) // 회피가 발동하면 데미지를 주지 않음.
                            player.HP -= actualDmg;

                        if (player.HP < 0) player.HP = 0;

                        Console.Clear();
                        Console.WriteLine("Battle!!\n");

                        if (!isEvaded) // 회피가 발동했을 시 문구 추가.
                        {
                            Console.WriteLine($"{m.Name}의 공격!\n{player.Name}을(를) 맞췄습니다.");

                            Console.WriteLine($"[데미지 : {actualDmg}] {(isCrit ? "- 치명타 공격!!" : "")} (원래 공격력: {rawDmg}, 방어력: {player.Defense})\n"); // 치명타 발동 시 문구 추가.
                            Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP {prevHp} -> {player.HP}\n");
                        }
                        else
                        {
                            Console.WriteLine($"{m.Name}의 공격!");
                            Console.WriteLine($"{player.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");
                        }

                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        if (player.HP <= 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Battle!! - Result\n");
                            Console.WriteLine("You Lose\n");
                            Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP 100 -> 0\n");
                            Console.WriteLine("0. 다음\n>> ");
                            Console.ReadLine();
                            return;
                        }
                    }
                }
            }
        }
    }
}