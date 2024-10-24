using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Misc;

namespace Server.Items
{
    public class PetLeash : Item
    {
        private BaseCreature m_ShrunkPet;

        [Constructable]
        public PetLeash() : base(0x1374)
        {
            Weight = 1.0;
            Name = "Pet Leash";
            LootType = LootType.Blessed;
            Hue = 1161;
        }

        public PetLeash(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Target the pet you wish to shrink.");
            from.Target = new ShrinkTarget();
        }

        private int GetStatueID(BaseCreature pet)
        {
            if (pet is IMount) return 0x2D89;
            
            switch (pet.Body.BodyID)
            {
                case 0xD9: return 0x2D97; // Wolf/Dog
                case 0xC9: return 0x2D85; // Cat
                case 0xC:  // Dragons
                case 0x3B: return 0x2D82;
                case 0x9:  return 0x2D86; // Demon
                case 0x15: return 0x2D9A; // Snake/Reptile
                default:   return 0x2614; // Default statue
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_ShrunkPet);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_ShrunkPet = reader.ReadMobile() as BaseCreature;
        }

        private class ShrinkTarget : Target
        {
            public ShrinkTarget() : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature pet)
                {
                    if (pet.ControlMaster != from)
                    {
                        from.SendMessage("That is not your pet.");
                        return;
                    }

                    Item statue = new PetStatue(pet);
                    from.AddToBackpack(statue);
                    pet.Internalize();
                    from.SendMessage("You have shrunk your pet into a statue.");
                }
                else
                {
                    from.SendMessage("That is not a valid pet.");
                }
            }
        }
    }

    public class PetStatue : Item
    {
        private BaseCreature m_Pet;
        private int m_StoredKillPoints;
        private int m_StoredLevel;

        public PetStatue(BaseCreature pet) : base(0x2614)
        {
            m_Pet = pet;
            Name = $"Statue of {pet.Name}";
            Weight = 0.0;
            LootType = LootType.Blessed;
            Hue = pet.Hue;

            if (pet is IEvolutionCreature evolutionPet)
            {
                m_StoredKillPoints = evolutionPet.KillPoints;
                m_StoredLevel = evolutionPet.Level;
            }

            if (pet is IMount) 
                ItemID = 0x2D89;
            else
                ItemID = GetStatueIDFromBody(pet.Body.BodyID);
        }

        private int GetStatueIDFromBody(int bodyID)
        {
            switch (bodyID)
            {
                case 0xD9: return 0x2D97; // Wolf/Dog
                case 0xC9: return 0x2D85; // Cat
                case 0xC:  // Dragons
                case 0x3B: return 0x2D82;
                case 0x9:  return 0x2D86; // Demon
                case 0x15: return 0x2D9A; // Snake/Reptile
                default:   return 0x2614; // Default statue
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Pet != null && from == m_Pet.ControlMaster)
            {
                m_Pet.MoveToWorld(from.Location, from.Map);

                if (m_Pet is IEvolutionCreature evolutionPet)
                {
                    evolutionPet.KillPoints = m_StoredKillPoints;
                    evolutionPet.CheckEvolution();
                }

                this.Delete();
                from.SendMessage("Your pet has been restored.");
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Pet is IEvolutionCreature)
            {
                list.Add($"Kill Points: {m_StoredKillPoints}");
                list.Add($"Evolution Level: {m_StoredLevel}");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Pet);
            writer.Write(m_StoredKillPoints);
            writer.Write(m_StoredLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Pet = reader.ReadMobile() as BaseCreature;
            m_StoredKillPoints = reader.ReadInt();
            m_StoredLevel = reader.ReadInt();
        }
    }
}