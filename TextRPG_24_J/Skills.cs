using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG_24_J
{
    public enum SkillType
    {
        SingleTarget,
        RandomTarget,
        AllTarget,
        SelfTarget
    }
    public class Skills
    {
        public string Name { get; set; }

        public int MPCost { get; set; }

        public string Description { get; set; } //스킬 설명

        public float DamageMultiplier { get; set; } //데미지 배율

        public int NumberofTargets { get; set; } //

        public SkillType Type { get; set; }

        public int HealAmount { get; set; }

        static QuestBoard? board;
        static QuestUI? quest;

        public static void SetQuestClass(QuestBoard _board, QuestUI _quest)
        {
            board = _board;
            quest = _quest;
        }

        public Skills(string name, int mpCost, string description, float damageMultiplier, SkillType type, int numberofTargets = 1, int healAmount = 0)
        {
            Name = name;
            MPCost = mpCost;
            Description = description;

            DamageMultiplier = damageMultiplier;

            NumberofTargets = numberofTargets;

            Type = type;

            HealAmount = healAmount;

        }
        public static List<Skills> GetSkill(int attackPower)
        {
            List<Skills> skillList = new List<Skills>();

            skillList.Add(new Skills(
                name: "알파 스트라이크",
                mpCost: 5,
                description: "공격력 X 2 로 하나의 적을 공격합니다.\n",
                damageMultiplier: 2,
                SkillType.SingleTarget));

            skillList.Add(new Skills(
                name: "더블 스트라이크",
                mpCost: 15,
                description: "공격력 X 1.5 로 두명의 적을 공격합니다.\n",
                damageMultiplier: 1.5f,
                SkillType.RandomTarget,
                numberofTargets: 2));

            skillList.Add(new Skills(
                name: "명상",
                mpCost: 10,
                description: "명상에 들어가서 공격력X2 만큼 회복합니다.\n",
                damageMultiplier: 0,
                SkillType.SelfTarget,
                numberofTargets: 0,
                healAmount: attackPower * 2));

            return skillList;
        }

        #region 스킬 사용 메서드
        // 스킬 사용 메서드
        public static bool UseSkill(Skills skill, Player player, List<Monster> monsters, Random random)
        {
            // MP 체크
            if (player.CurrentMana < skill.MPCost)
            {
                Console.WriteLine("마나가 부족합니다.");
                Thread.Sleep(1000);
                return false;
            }

            // MP 소모
            player.CurrentMana -= skill.MPCost;

            // 스킬 타입에 따른 처리
            switch (skill.Type)
            {
                case SkillType.SingleTarget:
                    return UseSingleTargetSkill(skill, player, monsters, random);

                case SkillType.RandomTarget:
                    return UseRandomTargetSkill(skill, player, monsters, random);

                case SkillType.AllTarget:
                    return UseAllTargetSkill(skill, player, monsters, random);

                case SkillType.SelfTarget:
                    return UseSelfTargetSkill(skill, player, random);

                default:
                    Console.WriteLine("지원하지 않는 스킬 타입입니다.");
                    player.CurrentMana += skill.MPCost; // MP 환불
                    Thread.Sleep(1000);
                    return false;
            }
        }

        // 단일 타겟 스킬 사용
        private static bool UseSingleTargetSkill(Skills skill, Player player, List<Monster> monsters, Random random)
        {
            // 대상 선택
            Console.WriteLine("\n대상을 선택해주세요.");
            for (int i = 0; i < monsters.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {monsters[i].Name} {(monsters[i].IsDead ? "(Dead)" : "")}");
            }
            Console.WriteLine("0. 취소");
            Console.Write(">> ");
            string targetSelect = Console.ReadLine();

            if (targetSelect == "0")
            {
                player.CurrentMana += skill.MPCost; // MP 환불
                return false;
            }

            if (!int.TryParse(targetSelect, out int targetIdx) || targetIdx < 1 || targetIdx > monsters.Count)
            {
                Console.WriteLine("잘못된 입력입니다.");
                player.CurrentMana += skill.MPCost; // MP 환불
                Thread.Sleep(1000);
                return false;
            }

            Monster target = monsters[targetIdx - 1];
            if (target.IsDead)
            {
                Console.WriteLine("이미 죽은 몬스터입니다.");
                player.CurrentMana += skill.MPCost; // MP 환불
                Thread.Sleep(1000);
                return false;
            }

            // 데미지 계산 및 적용
            int skillDamage = (int)(player.Attack * skill.DamageMultiplier);
            target.Hp -= skillDamage;
            if (target.Hp < 0)
            {
                target.Hp = 0;
                if (target.Name == "미니언" && board?.QuestList[0].IsQuestAccept == true)
                {
                    if (quest != null)
                        quest.QuestMonster["미니언"]++;
                }
                target.Hp = 0;
            }

            Console.WriteLine($"{skill.Name} 스킬로 {target.Name}을(를) 공격했습니다! [데미지: {skillDamage}]");
            Thread.Sleep(1000);
            return true;
        }

        // 랜덤 타겟 스킬 사용
        private static bool UseRandomTargetSkill(Skills skill, Player player, List<Monster> monsters, Random random)
        {
            List<Monster> aliveMonsters = monsters.Where(m => !m.IsDead).ToList();
            if (aliveMonsters.Count < skill.NumberofTargets)
            {
                Console.WriteLine($"대상이 부족합니다. (필요: {skill.NumberofTargets}명, 현재: {aliveMonsters.Count}명)");
                player.CurrentMana += skill.MPCost; // MP 환불
                Thread.Sleep(1000);
                return false;
            }

            // 랜덤으로 타겟 선택
            List<Monster> targets = new List<Monster>();
            for (int i = 0; i < skill.NumberofTargets; i++)
            {
                int randIdx = random.Next(aliveMonsters.Count);
                targets.Add(aliveMonsters[randIdx]);
                aliveMonsters.RemoveAt(randIdx);
            }

            // 데미지 계산 및 적용
            int skillDamage = (int)(player.Attack * skill.DamageMultiplier);
            Console.WriteLine($"{skill.Name} 스킬을 사용했습니다!");

            foreach (var mon in targets)
            {
                mon.Hp -= skillDamage;
                if (mon.Hp < 0)
                {
                    mon.Hp = 0;
                    if (mon.Name == "미니언" && board?.QuestList[0].IsQuestAccept == true)
                    {
                        if (quest != null)
                            quest.QuestMonster["미니언"]++;
                    }
                }
                Console.WriteLine($"{mon.Name}에게 {skillDamage}의 데미지를 입혔습니다!");
            }

            Thread.Sleep(1000);
            return true;
        }

        // 전체 타겟 스킬 사용
        private static bool UseAllTargetSkill(Skills skill, Player player, List<Monster> monsters, Random random)
        {
            int skillDamage = (int)(player.Attack * skill.DamageMultiplier);
            Console.WriteLine($"{skill.Name} 스킬을 사용했습니다!");

            bool hitAny = false;
            foreach (var mon in monsters)
            {
                if (mon.IsDead) continue;

                hitAny = true;
                mon.Hp -= skillDamage;
                if (mon.Hp < 0) mon.Hp = 0;
                Console.WriteLine($"{mon.Name}에게 {skillDamage}의 데미지를 입혔습니다!");
            }

            if (!hitAny)
            {
                Console.WriteLine("공격할 대상이 없습니다.");
                player.CurrentMana += skill.MPCost; // MP 환불
                Thread.Sleep(1000);
                return false;
            }

            Thread.Sleep(1000);
            return true;
        }

        // 자기 자신 타겟 스킬 사용
        private static bool UseSelfTargetSkill(Skills skill, Player player, Random random)
        {
            int healAmount = skill.HealAmount;
            int prevHP = player.HP;
            player.HP += healAmount;

            // 최대 체력 제한
            if (player.HP > 100) player.HP = 100;

            Console.WriteLine($"{skill.Name} 스킬을 사용했습니다!");
            Console.WriteLine($"HP가 {prevHP}에서 {player.HP}로 회복되었습니다! [회복량: {player.HP - prevHP}]");
            Thread.Sleep(1000);
            return true;
        }
        #endregion


    }
}
