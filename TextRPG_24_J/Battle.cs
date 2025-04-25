using System.Threading;

namespace TextRPG_24_J
{


    public class Monster
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int Attack { get; set; }
        public bool IsDead => Hp <= 0;

        public Monster(string name, int level, int hp, int atk)
        {
            Name = name;
            Level = level;
            MaxHp = hp;
            Hp = hp;
            Attack = atk;
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
                        monsters.Add(new Monster("미니언", 2, 15, 5));
                        break;
                    case 1:
                        monsters.Add(new Monster("공허충", 3, 10, 9));
                        break;
                    case 2:
                        monsters.Add(new Monster("대포미니언", 5, 25, 8));
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


                    int variation = (int)Math.Ceiling(player.Attack * 0.1);
                    int finalDamage = random.Next(player.Attack - variation, player.Attack + variation + 1);
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
                    Console.WriteLine($"{target.Name}을(를) 공격했습니다! [데미지 : {finalDamage}]");
                    Thread.Sleep(1000);


                    if (monsters.All(m => m.IsDead))
                    {
                        Console.Clear();
                        Console.WriteLine("Battle!! - Result\n");
                        Console.WriteLine("Victory\n");
                        Console.WriteLine($"던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP 100 -> {player.HP}\n");
                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        return;
                    }


                    for (int i = 0; i < monsters.Count; i++)
                    {
                        var m = monsters[i];
                        if (m.IsDead) continue;

                        Console.Clear();
                        Console.WriteLine("Battle!!\n");
                        Console.WriteLine($"{m.Name}의 공격!\n{player.Name}을(를) 맞췄습니다.");


                        int monVar = (int)Math.Ceiling(m.Attack * 0.1);
                        int rawDmg = random.Next(m.Attack - monVar, m.Attack + monVar + 1);


                        int actualDmg = rawDmg - player.Defense;
                        if (actualDmg < 0) actualDmg = 0;

                        int prevHp = player.HP;
                        player.HP -= actualDmg;
                        if (player.HP < 0) player.HP = 0;


                        Console.WriteLine($"[데미지 : {actualDmg}] (원래 공격력: {rawDmg}, 방어력: {player.Defense})\n");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP {prevHp} -> {player.HP}\n");

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
                        Console.WriteLine("0. 다음\n>> ");
                        Console.ReadLine();
                        return;                                       


                    }

                    // 몬스터 턴
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        var m = monsters[i];
                        if (m.IsDead) continue;

                        Console.Clear();
                        Console.WriteLine("Battle!!\n");
                        Console.WriteLine($"{m.Name}의 공격!\n{player.Name}을(를) 맞췄습니다.");

                        int monVar = (int)Math.Ceiling(m.Attack * 0.1);
                        int rawDmg = random.Next(m.Attack - monVar, m.Attack + monVar + 1);
                        int actualDmg = rawDmg - player.Defense;
                        if (actualDmg < 0) actualDmg = 0;

                        int prevHp = player.HP;
                        player.HP -= actualDmg;
                        if (player.HP < 0) player.HP = 0;

                        Console.WriteLine($"[데미지 : {actualDmg}] (원래 공격력: {rawDmg}, 방어력: {player.Defense})\n");
                        Console.WriteLine($"Lv.{player.Level} {player.Name}\nHP {prevHp} -> {player.HP}\n");

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