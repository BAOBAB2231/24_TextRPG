using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;
using TextRPG_24_J;

namespace TextRPG_24_J
{
    public class Quest
    {
        public string QuestTitle { get; set; }
        public string QuestDescription { get; set; }
        public bool IsDone => QuestState == QuestComplete;
        public List<Item> QuestReward { get; set; }
        public int Gold { get; set; }
        public string QuestRequest { get; set; }
        public bool IsQuestAccept { get; set; } = false;
        public int QuestState { get; set; }
        public int QuestComplete { get; set; }
    }


    public class QuestBoard
    {
        List<ShopItem> shopItems;
        Player player;
        public QuestBoard(Player player, List<ShopItem> shopItems)
        {
            this.shopItems = shopItems;
            this.player = player;
            AddQuest();
        }

        public List<Quest> QuestList = new List<Quest>();

        private void AddQuest()
        {
            QuestList.Add(new Quest
            {
                QuestTitle = "마을을 위협하는 미니언 처치",
                QuestDescription = "이봐! 마을 근처에 미니언들이 너무 많아졌다고 생각하지 않나?\n마을주민들의 안전을 위해서라도 저것들 수를 좀 줄여야 한다고!\n모험가인 자네가 좀 처치해주게!",
                QuestRequest = "미니언 5마리 처치",
                QuestReward = new List<Item> { shopItems[0] },
                QuestState = 0,
                QuestComplete = 5,
                Gold = 500
            });

            QuestList.Add(new Quest
            {
                QuestTitle = "장비를 장착해보자",
                QuestDescription = "그 옷차림으로 몬스터랑 싸우겠다고? 하하! 미안, 미안. 자, 이건 내가 특별히 챙겨둔 장비야.\n입어보고 와, 적어도 바람막이는 될 테니까!",
                QuestRequest = "인벤토리로 가서 종류와 상관없이 아이템을 장착",
                QuestReward = new List<Item> { shopItems[4] },
                QuestState = 0,
                QuestComplete = 1,
                Gold = 300
            });

            QuestList.Add(new Quest
            {
                QuestTitle = "더욱 더 강해지기!",
                QuestDescription = "오오~ 드디어 한 단계 올라섰군! 이젠 잡몹들한테 밀리진 않겠는데?\n하지만 자만은 금물이야! 진짜 싸움은 지금부터라고~",
                QuestRequest = "5레벨 달성",
                QuestReward = new List<Item> { shopItems[2], shopItems[5] },
                QuestState = player.Level,
                QuestComplete = 5,
                Gold = 800
            });

            QuestList.Add(new Quest
            {
                QuestTitle = "개발자의 숨겨진 비밀",
                QuestDescription = "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR \n" +
                "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR",
                QuestRequest = "????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "??????????????????????????????????????????????????????????????????????? \n" +
                "???????????????????????????????????????????????????????????????????????",
                QuestReward = new List<Item> { shopItems[6] },
                QuestState = 0,
                QuestComplete = 5,
                Gold = 999999999
            });

        }

        public void ShowQuestList()
        {
            for (int i = 0; i < QuestList.Count; i++)
            {
                string accepted = QuestList[i].IsQuestAccept ? " [진행중]" : "";
                Console.WriteLine($"{i + 1}. {QuestList[i].QuestTitle}{accepted}");
            }
        }
    }



    public class QuestUI
    {
        QuestBoard board;
        Player player;
        public QuestUI(QuestBoard board, Player player)
        {
            this.board = board;
            this.player = player;
        }

        public Dictionary<string, int> QuestMonster = new Dictionary<string, int>();

        public void QuestMenu()
        {
            bool isloop = true;

            while (isloop)
            {
                Console.Clear();
                Console.WriteLine("\nQuest!!\n");
                board.ShowQuestList();
                Console.WriteLine("\n0. 나가기");
                Console.WriteLine("\n원하시는 퀘스트를 선택해주세요.");
                Console.Write(">>");

                string input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "0":
                        isloop = false;
                        break;
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                        ShowQuestDescription(int.Parse(input) - 1);
                        break;
                    default:
                        Console.WriteLine("\n잘못된 입력입니다.\n");
                        Pause();
                        break;
                }
            }
        }


        public void ShowQuestDescription(int intInput)
        {
            Quest quest = board.QuestList[intInput];

            while (quest.IsQuestAccept == true)
            {
                board.QuestList[0].QuestState = QuestMonster["미니언"];

                Console.Clear();
                Console.WriteLine("\nQuest!!\n");
                Console.WriteLine(quest.QuestTitle);
                Console.WriteLine($"\n{quest.QuestDescription}\n");
                Console.WriteLine($"- {quest.QuestRequest} ({quest.QuestState}/{quest.QuestComplete})\n");
                Console.WriteLine("- 보상");
                for (int i = 0; i < quest.QuestReward.Count; i++)
                {
                    Console.WriteLine($"{quest.QuestReward[i].Name}");
                }
                Console.WriteLine(quest.Gold + "G\n");
                QuestStatus(intInput);
                Console.WriteLine("0. 나가기");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                string input = Console.ReadLine().Trim();

                if (input == "1")
                {
                    for (int i = 0; i < board.QuestList.Count; i++)
                    { board.QuestList[i].QuestState = 0; }
                    quest.IsQuestAccept = false;
                    break;
                }
                else if (input == "2")
                {
                    ReceiveReward(quest);
                    break;
                }
                else if (input == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Pause();
                }
            }

            while (quest.IsQuestAccept == false)
            {
                Console.Clear();
                Console.WriteLine("\nQuest!!\n");
                Console.WriteLine(quest.QuestTitle);
                Console.WriteLine($"\n{quest.QuestDescription}\n");
                Console.WriteLine($"- {quest.QuestRequest} ({quest.QuestState}/{quest.QuestComplete})\n");
                Console.WriteLine("- 보상");
                for (int i = 0; i < quest.QuestReward.Count; i++)
                {
                    Console.WriteLine($"{quest.QuestReward[i].Name}");
                }
                Console.WriteLine(quest.Gold + "G\n");
                Console.WriteLine("1. 수락\n");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                Console.Write(">>");

                string input = Console.ReadLine().Trim();

                if (input == "1")
                {
                    quest.IsQuestAccept = true;
                    if (quest == board.QuestList[0])
                    {
                        QuestMonster.Remove("미니언");
                        QuestMonster.Add("미니언", 0);
                    }
                    break;
                }
                else if (input == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    Pause();
                }
            }
        }

        void QuestStatus(int intInput)
        {

            Quest quest = board.QuestList[intInput];
            if (quest.IsDone == false)
            {
                Console.WriteLine("1. 퀘스트 포기\n");
            }
            else
            {
                Console.WriteLine("1. 퀘스트 포기");
                Console.WriteLine("2. 보상 받기\n");
            }
        }


        void ReceiveReward(Quest quest)
        {
            if (quest.IsDone)
            {
                player.Gold += quest.Gold;
                for (int i = 0; i < quest.QuestReward.Count; i++)
                Inventory.Add(quest.QuestReward[i]);
                quest.IsQuestAccept = false;
                quest.QuestState = 0;
                Console.Write($"보상으로 {quest.Gold}G 골드를 받고, ");
                for (int i = 0; i < quest.QuestReward.Count; i++)
                    Console.Write($"{quest.QuestReward[i].Name} ");
                Console.Write($"를 획득했습니다!");
                Pause();
            }
            else
            {
                Console.WriteLine("퀘스트가 완료되지 않았습니다!");
                Pause();
            }
        }

        void Pause()
        {
            Console.WriteLine("\n아무 키나 누르면 계속합니다...");
            Console.ReadKey();
        }
    }
}


