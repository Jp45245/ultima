using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using Server.Interfaces;

namespace Server.Mobiles
{
    public class EvolutionPetEgg : Item
    {
        [Constructable]
        public EvolutionPetEgg() : base(0x47E6)
        {
            Name = "Evolution Pet Egg";
            Hue = 1161;
            LootType = LootType.Blessed;
        }

        public EvolutionPetEgg(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }
            from.SendGump(new ConfirmPetHatchGump(this));
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

    public class ConfirmPetHatchGump : Gump
    {
        private EvolutionPetEgg m_Egg;

        public ConfirmPetHatchGump(EvolutionPetEgg egg) : base(50, 50)
        {
            m_Egg = egg;
            AddPage(0);
            AddBackground(0, 0, 240, 135, 9250);
            AddAlphaRegion(10, 10, 220, 115);
            AddHtml(20, 20, 200, 80, "Do you want to hatch this egg?", false, false);
            AddButton(40, 95, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
            AddButton(170, 95, 0xF1, 0xF2, 0, GumpButtonType.Reply, 0);
            AddLabel(75, 95, 0x34, "Yes");
            AddLabel(205, 95, 0x34, "No");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                Mobile from = sender.Mobile;
                if (m_Egg.Deleted || !m_Egg.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001);
                    return;
                }
                EvolutionPet pet = new EvolutionPet(from);
                pet.MoveToWorld(from.Location, from.Map);
                pet.IsBonded = true;
                m_Egg.Delete();
                from.SendMessage("You have successfully hatched the Evolution Pet Egg!");
            }
        }
    }

    [CorpseName("corpse of an Evolution Pet")]
    public class EvolutionPet : BaseCreature, IEvolutionCreature
    {
        private int m_Stage;
        private int m_KillPoints;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Stage
        {
            get { return m_Stage; }
            set { m_Stage = Math.Max(0, Math.Min(2, value)); InvalidateProperties(); SetAppearance(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KillPoints
        {
            get { return m_KillPoints; }
            set { m_KillPoints = value; CheckEvolution(); InvalidateProperties(); }
        }

        public int Level { get { return Stage + 1; } }
        public int MaxKillPoints { get { return 10000; } }

        public override bool AlwaysMurderer { get { return false; } }
        public override bool ShowFameTitle { get { return false; } }

        [Constructable]
        public EvolutionPet(Mobile owner) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Stage = 0;
            KillPoints = 0;
            
            SetStr(100);
            SetDex(100);
            SetInt(100);
            SetHits(100);
            SetStam(100);
            SetMana(0);
            
            SetDamage(5, 10);
            SetDamageType(ResistanceType.Physical, 100);
            
            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 20);
            SetResistance(ResistanceType.Cold, 20);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 20);
            
            SetSkill(SkillName.Wrestling, 50.0);
            SetSkill(SkillName.Tactics, 50.0);
            SetSkill(SkillName.MagicResist, 50.0);
            SetSkill(SkillName.Anatomy, 50.0);
            
            Fame = 0;
            Karma = 0;
            
            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = 0.0;

            if (owner != null)
                SetControlMaster(owner);

            SetAppearance();
        }

        public EvolutionPet(Serial serial) : base(serial) { }

        public void CheckEvolution()
        {
            if (Stage == 0 && KillPoints >= 1000) Stage = 1;
            else if (Stage == 1 && KillPoints >= 5000) Stage = 2;
        }

        public void SetAppearance()
        {
            switch (Stage)
            {
                case 0:
                    Name = "Baby Evolution Pet";
                    BodyValue = 217;
                    Hue = 1150;
                    break;
                case 1:
                    Name = "Adolescent Evolution Pet";
                    BodyValue = 225;
                    Hue = 1150;
                    SetStr(200);
                    SetDex(200);
                    SetInt(200);
                    SetHits(200);
                    SetDamage(10, 20);
                    break;
                case 2:
                    Name = "Adult Evolution Pet";
                    BodyValue = 250;
                    Hue = 1150;
                    SetStr(400);
                    SetDex(400);
                    SetInt(400);
                    SetHits(400);
                    SetDamage(20, 30);
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Stage);
            writer.Write(m_KillPoints);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Stage = reader.ReadInt();
            m_KillPoints = reader.ReadInt();
            SetAppearance();
        }
    }
}