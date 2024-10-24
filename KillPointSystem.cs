using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.Commands;

namespace Server.Misc
{
    public class KillPointSystem
    {
        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;
            CommandSystem.Register("AddKillPoints", AccessLevel.GameMaster, new CommandEventHandler(AddKillPoints_OnCommand));
            CommandSystem.Register("RemoveKillPoints", AccessLevel.GameMaster, new CommandEventHandler(RemoveKillPoints_OnCommand));
            CommandSystem.Register("CheckKillPoints", AccessLevel.Player, new CommandEventHandler(CheckKillPoints_OnCommand));
        }

        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature creature = e.Creature as BaseCreature;
            if (creature == null)
                return;

            Mobile killer = e.Killer;
            if (killer == null)
                return;

            if (killer is PlayerMobile player)
            {
                AssignKillPointsToFollowers(player, creature);
            }
            else if (killer is IEvolutionCreature evolutionCreature)
            {
                AssignKillPoints(evolutionCreature, creature);
            }
        }

        private static void AssignKillPointsToFollowers(PlayerMobile player, BaseCreature killedCreature)
        {
            foreach (Mobile follower in new List<Mobile>(player.Followers))
            {
                if (follower is IEvolutionCreature evolutionCreature)
                {
                    AssignKillPoints(evolutionCreature, killedCreature);
                }
            }
        }

        private static void AssignKillPoints(IEvolutionCreature evolutionCreature, BaseCreature killedCreature)
        {
            int killPoints = CalculateKillPoints(killedCreature);
            evolutionCreature.KillPoints += killPoints;
            
            if (evolutionCreature.KillPoints > evolutionCreature.MaxKillPoints)
                evolutionCreature.KillPoints = evolutionCreature.MaxKillPoints;
            
            evolutionCreature.CheckEvolution();
            
            if (killedCreature.Fame >= 10000)
            {
                DropEvolutionTokens(killedCreature, Utility.RandomMinMax(3, 5));
            }
            else if (killedCreature.Fame >= 5000)
            {
                if (Utility.RandomDouble() < 0.35)
                    DropEvolutionTokens(killedCreature, Utility.RandomMinMax(1, 3));
            }
            else
            {
                if (Utility.RandomDouble() < 0.25)
                    DropEvolutionTokens(killedCreature, 1);
            }
        }

        private static int CalculateKillPoints(BaseCreature creature)
        {
            int basePoints = (int)(creature.Fame / 100);
            
            if (creature.HitsMax >= 1000)
                basePoints += 50;
            else if (creature.HitsMax >= 500)
                basePoints += 25;
                
            return Math.Max(1, basePoints);
        }

        private static void DropEvolutionTokens(BaseCreature creature, int amount)
        {
            Item token = new Item(0xEED)
            {
                Name = "Evolution Token",
                Stackable = true,
                Amount = amount,
                Hue = 1161,
                LootType = LootType.Blessed
            };
            
            if (creature.Backpack == null)
                creature.AddToBackpack(token);
            else
                creature.Backpack.DropItem(token);
        }

        [Usage("CheckKillPoints")]
        [Description("Checks the kill points of a targeted pet or mercenary.")]
        public static void CheckKillPoints_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the pet or mercenary you want to check.");
            e.Mobile.Target = new CheckKillPointsTarget();
        }

        [Usage("AddKillPoints <amount>")]
        [Description("Adds kill points to a targeted pet or mercenary.")]
        public static void AddKillPoints_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                e.Mobile.Target = new AddKillPointsTarget(e.GetInt32(0));
            }
            else
            {
                e.Mobile.SendMessage("Usage: AddKillPoints <amount>");
            }
        }

        [Usage("RemoveKillPoints <amount>")]
        [Description("Removes kill points from a targeted pet or mercenary.")]
        public static void RemoveKillPoints_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                e.Mobile.Target = new RemoveKillPointsTarget(e.GetInt32(0));
            }
            else
            {
                e.Mobile.SendMessage("Usage: RemoveKillPoints <amount>");
            }
        }

        private class CheckKillPointsTarget : Target
        {
            public CheckKillPointsTarget() : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IEvolutionCreature evolutionCreature)
                {
                    int currentLevel = evolutionCreature.Level;
                    int nextLevelPoints = (currentLevel + 1) * 1000;
                    int pointsNeeded = nextLevelPoints - evolutionCreature.KillPoints;

                    from.SendMessage($"Kill Points: {evolutionCreature.KillPoints}");
                    from.SendMessage($"Current Level: {currentLevel}");
                    from.SendMessage($"Points needed for next level: {pointsNeeded}");
                }
                else
                {
                    from.SendMessage("That is not a valid pet or mercenary.");
                }
            }
        }

        private class AddKillPointsTarget : Target
        {
            private int m_Amount;

            public AddKillPointsTarget(int amount) : base(10, false, TargetFlags.None)
            {
                m_Amount = amount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IEvolutionCreature creature)
                {
                    creature.KillPoints += m_Amount;
                    from.SendMessage($"Added {m_Amount} kill points to the creature.");
                }
                else
                {
                    from.SendMessage("That is not a valid pet or mercenary.");
                }
            }
        }

        private class RemoveKillPointsTarget : Target
        {
            private int m_Amount;

            public RemoveKillPointsTarget(int amount) : base(10, false, TargetFlags.None)
            {
                m_Amount = amount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IEvolutionCreature creature)
                {
                    creature.KillPoints = Math.Max(0, creature.KillPoints - m_Amount);
                    from.SendMessage($"Removed {m_Amount} kill points from the creature.");
                }
                else
                {
                    from.SendMessage("That is not a valid pet or mercenary.");
                }
            }
        }
    }

    public interface IEvolutionCreature
    {
        int KillPoints { get; set; }
        int Level { get; }
        int MaxKillPoints { get; }
        void CheckEvolution();
    }
}