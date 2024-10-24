using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("corpse of Pixie Poopy")]
    public class PixiePoopy : BaseCreature
    {
        [Constructable]
        public PixiePoopy() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Pixie Poopy";
            Body = 128;
            BaseSoundID = 0x4B8;

            SetStr(196, 225);
            SetDex(186, 205);
            SetInt(186, 225);

            SetHits(118, 135);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 40);
            SetResistance(ResistanceType.Cold, 25, 40);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 80.1, 95.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 40;
        }

        public PixiePoopy(Serial serial) : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 0);
            PackItem(new EvolutionPetEgg());
            PackItem(new CustomPetBondingPotion());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private void ThrowPoop()
        {
            if (Combatant == null || !Alive || Map == null)
                return;

            Mobile target = Combatant as Mobile;

            if (target == null || target.Map != Map || !InRange(target.Location, 4))
                return;

            // Implement poop throwing logic here
        }
    }
}