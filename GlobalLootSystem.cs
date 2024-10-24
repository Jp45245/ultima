using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    public class GlobalLootSystem
    {
        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;
        }

        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            if (e.Creature is BaseCreature creature)
            {
                if (creature.HitsMax >= 300)
                {
                    Item token = new Item(0xEED)
                    {
                        Name = "Evolution Token",
                        Stackable = true,
                        Amount = 1,
                        Hue = 1161,
                        LootType = LootType.Blessed
                    };

                    if (creature.Backpack == null)
                        creature.AddToBackpack(token);
                    else
                        creature.Backpack.DropItem(token);

                    // Additional rare loot chance
                    if (Utility.RandomDouble() < 0.05) // 5% chance
                    {
                        EvolutionMercenaryEgg egg = new EvolutionMercenaryEgg();
                        creature.AddToBackpack(egg);
                    }
                }
            }
        }
    }
}