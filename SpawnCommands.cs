using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting; // Add this line
using Server.Interfaces; // Add this line if IEvolutionCreature is in this namespace

namespace Server.Scripts.Commands
{
    public class SpawnCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SpawnPixiePoopy", AccessLevel.GameMaster, new CommandEventHandler(SpawnPixiePoopy_OnCommand));
            CommandSystem.Register("SpawnEvolutionPet", AccessLevel.GameMaster, new CommandEventHandler(SpawnEvolutionPet_OnCommand));
            CommandSystem.Register("SpawnEvolutionMercenary", AccessLevel.GameMaster, new CommandEventHandler(SpawnEvolutionMercenary_OnCommand));
            CommandSystem.Register("CheckKillPoints", AccessLevel.Player, new CommandEventHandler(CheckKillPoints_OnCommand));
        }

        [Usage("SpawnPixiePoopy")]
        [Description("Spawns a Pixie Poopy monster at your location.")]
        public static void SpawnPixiePoopy_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PixiePoopy monster = new PixiePoopy();
            monster.MoveToWorld(from.Location, from.Map);
            from.SendMessage("Pixie Poopy has been spawned at your location.");
        }

        [Usage("SpawnEvolutionPet")]
        [Description("Spawns an Evolution Pet at your location.")]
        public static void SpawnEvolutionPet_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionPet pet = new EvolutionPet(from);
            pet.MoveToWorld(from.Location, from.Map);
            from.SendMessage("An Evolution Pet has been spawned at your location.");
        }

        [Usage("SpawnEvolutionMercenary")]
        [Description("Spawns an Evolution Mercenary at your location.")]
        public static void SpawnEvolutionMercenary_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionMercenary merc = new EvolutionMercenary(from);
            merc.MoveToWorld(from.Location, from.Map);
            from.SendMessage("An Evolution Mercenary has been spawned at your location.");
        }

        [Usage("CheckKillPoints")]
        [Description("Checks the kill points of a targeted pet.")]
        public static void CheckKillPoints_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the pet you want to check kill points for.");
            e.Mobile.Target = new KillPointsTarget();
        }

        private class KillPointsTarget : Target
        {
            public KillPointsTarget() : base(10, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IEvolutionCreature creature)
                {
                    from.SendMessage($"Kill Points: {creature.KillPoints}, Level: {creature.Level}");
                }
                else
                {
                    from.SendMessage("That is not a valid evolution creature.");
                }
            }
        }
    }
}