using System;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;

namespace Server.Scripts.Commands
{
    public class PetSystemCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("CreateEvolutionPet", AccessLevel.GameMaster, new CommandEventHandler(CreateEvolutionPet_OnCommand));
            CommandSystem.Register("CreateEvolutionPetEgg", AccessLevel.GameMaster, new CommandEventHandler(CreateEvolutionPetEgg_OnCommand));
            CommandSystem.Register("CreateEvolutionPetSpawner", AccessLevel.Administrator, new CommandEventHandler(CreateEvolutionPetSpawner_OnCommand));
            CommandSystem.Register("CreatePixiePoopy", AccessLevel.GameMaster, new CommandEventHandler(CreatePixiePoopy_OnCommand));
            CommandSystem.Register("CreatePixiePoopySpawner", AccessLevel.Administrator, new CommandEventHandler(CreatePixiePoopySpawner_OnCommand));
            CommandSystem.Register("CreateCustomPetBondingPotion", AccessLevel.GameMaster, new CommandEventHandler(CreateCustomPetBondingPotion_OnCommand));
            CommandSystem.Register("CreateEvolutionMercenary", AccessLevel.GameMaster, new CommandEventHandler(CreateEvolutionMercenary_OnCommand));
            CommandSystem.Register("CreateEvolutionMercenaryEgg", AccessLevel.GameMaster, new CommandEventHandler(CreateEvolutionMercenaryEgg_OnCommand));
            CommandSystem.Register("CreateEvolutionMercenarySpawner", AccessLevel.Administrator, new CommandEventHandler(CreateEvolutionMercenarySpawner_OnCommand));
            CommandSystem.Register("CreateCurrencyExchanger", AccessLevel.Administrator, new CommandEventHandler(CreateCurrencyExchanger_OnCommand));
            CommandSystem.Register("CreatePetLeash", AccessLevel.GameMaster, new CommandEventHandler(CreatePetLeash_OnCommand));
            CommandSystem.Register("SetTokenRate", AccessLevel.Administrator, new CommandEventHandler(SetTokenRate_OnCommand));
            CommandSystem.Register("SetItemPrice", AccessLevel.Administrator, new CommandEventHandler(SetItemPrice_OnCommand));
            CommandSystem.Register("getpets", AccessLevel.Player, new CommandEventHandler(GetPets_OnCommand));
        }

        [Usage("SetTokenRate <goldAmount>")]
        [Description("Sets how much gold is needed for 1 Evolution Token")]
        public static void SetTokenRate_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                int rate = e.GetInt32(0);
                Config.TokenExchangeRate = rate;
                e.Mobile.SendMessage($"Exchange rate set to {rate} gold per Evolution Token");
            }
            else
                e.Mobile.SendMessage("Usage: SetTokenRate <goldAmount>");
        }

        [Usage("SetItemPrice <itemType> <tokenAmount>")]
        [Description("Sets the token price for vendor items")]
        public static void SetItemPrice_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 2)
            {
                string itemType = e.GetString(0);
                int price = e.GetInt32(1);
                Config.SetItemPrice(itemType, price);
                e.Mobile.SendMessage($"Price for {itemType} set to {price} Evolution Tokens");
            }
            else
                e.Mobile.SendMessage("Usage: SetItemPrice <itemType> <tokenAmount>");
        }

        [Usage("getpets")]
        [Description("Retrieves all pets linked to the player.")]
        public static void GetPets_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            int petCount = 0;

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseCreature creature)
                {
                    if (creature.ControlMaster == from && !creature.Deleted && creature.Map == null)
                    {
                        creature.MoveToWorld(from.Location, from.Map);
                        petCount++;
                    }
                }
            }

            if (petCount > 0)
                from.SendMessage($"Retrieved {petCount} pets to your location.");
            else
                from.SendMessage("No pets found to retrieve.");
        }

        [Usage("CreateEvolutionPet")]
        [Description("Creates an Evolution Pet at your location.")]
        public static void CreateEvolutionPet_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionPet pet = new EvolutionPet(from);
            pet.MoveToWorld(from.Location, from.Map);
            from.SendMessage("An Evolution Pet has been created at your location.");
        }

        [Usage("CreateEvolutionPetEgg")]
        [Description("Creates an Evolution Pet Egg in your backpack.")]
        public static void CreateEvolutionPetEgg_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionPetEgg egg = new EvolutionPetEgg();
            from.AddToBackpack(egg);
            from.SendMessage("An Evolution Pet Egg has been added to your backpack.");
        }

        [Usage("CreateEvolutionPetSpawner")]
        [Description("Creates an Evolution Pet Spawner at your location.")]
        public static void CreateEvolutionPetSpawner_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Evolution Pet Spawner creation is not implemented yet.");
        }

        [Usage("CreatePixiePoopy")]
        [Description("Creates a Pixie Poopy at your location.")]
        public static void CreatePixiePoopy_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PixiePoopy pixie = new PixiePoopy();
            pixie.MoveToWorld(from.Location, from.Map);
            from.SendMessage("A Pixie Poopy has been created at your location.");
        }

        [Usage("CreatePixiePoopySpawner")]
        [Description("Creates a Pixie Poopy Spawner at your location.")]
        public static void CreatePixiePoopySpawner_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Pixie Poopy Spawner creation is not implemented yet.");
        }

        [Usage("CreateCustomPetBondingPotion")]
        [Description("Creates a Custom Pet Bonding Potion in your backpack.")]
        public static void CreateCustomPetBondingPotion_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Custom Pet Bonding Potion creation is not implemented yet.");
        }

        [Usage("CreateEvolutionMercenary")]
        [Description("Creates an Evolution Mercenary at your location.")]
        public static void CreateEvolutionMercenary_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionMercenary merc = new EvolutionMercenary(from);
            merc.MoveToWorld(from.Location, from.Map);
            from.SendMessage("An Evolution Mercenary has been created at your location.");
        }

        [Usage("CreateEvolutionMercenaryEgg")]
        [Description("Creates an Evolution Mercenary Egg in your backpack.")]
        public static void CreateEvolutionMercenaryEgg_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            EvolutionMercenaryEgg egg = new EvolutionMercenaryEgg();
            from.AddToBackpack(egg);
            from.SendMessage("An Evolution Mercenary Egg has been added to your backpack.");
        }

        [Usage("CreateEvolutionMercenarySpawner")]
        [Description("Creates an Evolution Mercenary Spawner at your location.")]
        public static void CreateEvolutionMercenarySpawner_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage("Evolution Mercenary Spawner creation is not implemented yet.");
        }

        [Usage("CreateCurrencyExchanger")]
        [Description("Creates a Universal Currency Exchanger at your location.")]
        public static void CreateCurrencyExchanger_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            UniversalCurrencyExchanger exchanger = new UniversalCurrencyExchanger();
            exchanger.MoveToWorld(from.Location, from.Map);
            from.SendMessage("A Universal Currency Exchanger has been created at your location.");
        }

        [Usage("CreatePetLeash")]
        [Description("Creates a Pet Leash in your backpack.")]
        public static void CreatePetLeash_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            PetLeash leash = new PetLeash();
            from.AddToBackpack(leash);
            from.SendMessage("A Pet Leash has been added to your backpack.");
        }
    }
}