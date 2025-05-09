﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG_24_J
{
    public class DropItem
    {
        public void Reward(Player player, List<Monster> monsters)
        {
            int goldToAdd = 0;

            Console.WriteLine("[획득 아이템]");
            for (int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                goldToAdd += monster.Gold;
                if (monster.Dropitem != null)
                {
                    Inventory.AddItem(monster.Dropitem);
                    Console.WriteLine($"{monster.Dropitem.Name}");
                }
            }
            player.Gold += goldToAdd;
            Console.WriteLine($"획득 골드: {goldToAdd} G  (현재 골드: {player.Gold} G)");


        }
    }
}
