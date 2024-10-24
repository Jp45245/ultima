using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class EvolutionToken : Item
    {
        [Constructable]
        public EvolutionToken() : this(1)
        {
        }

        [Constructable]
        public EvolutionToken(int amount) : base(0xEED)
        {
            Name = "Evolution Token";
            Stackable = true;
            Amount = amount;
            Hue = 1161;
            LootType = LootType.Blessed;
        }

        public EvolutionToken(Serial serial) : base(serial)
        {
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
    }
}