using Server.ContextMenus;
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
    public class EvolutionMercenaryEgg : Item
    {
        [Constructable]
        public EvolutionMercenaryEgg() : base(0x47E6)
        {
            Name = "Evolution Mercenary Egg";
            Hue = 1161;
            LootType = LootType.Blessed;
        }

        public EvolutionMercenaryEgg(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }
            from.SendGump(new ConfirmMercenaryHatchGump(this));
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

    public class ConfirmMercenaryHatchGump : Gump
    {
        private EvolutionMercenaryEgg m_Egg;

        public ConfirmMercenaryHatchGump(EvolutionMercenaryEgg egg) : base(50, 50)
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
                EvolutionMercenary merc = new EvolutionMercenary(from);
                merc.MoveToWorld(from.Location, from.Map);
                merc.IsBonded = true;
                m_Egg.Delete();
                from.SendMessage("You have successfully hatched the Evolution Mercenary Egg!");
            }
        }
    }

    [CorpseName("corpse of an Evolution Mercenary")]
    public class EvolutionMercenary : BaseCreature, IEvolutionCreature
    {
        private int m_KillPoints;

         [CommandProperty(AccessLevel.GameMaster)]
        public int KillPoints
        {
            get { return m_KillPoints; }
            set { m_KillPoints = value; CheckEvolution(); InvalidateProperties(); }
        }

        public int Level { get { return (KillPoints / 1000) + 1; } }
        public int MaxKillPoints { get { return 10000; } }

        public override bool AlwaysMurderer { get { return false; } }
        public override bool ShowFameTitle { get { return false; } }
        
        public override bool CanBeControlledBy(Mobile m)
        {
            return true;
        }

       public override void OnDoubleClick(Mobile from)
{
    if (from == ControlMaster)
        from.Use(this);
}

        [Constructable]
        public EvolutionMercenary(Mobile owner) : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            KillPoints = 0;
            
            SetStr(100);
            SetDex(100);
            SetInt(100);
            SetHits(100);
            SetStam(100);
            SetMana(100);
            
            SetDamage(10, 15);
            SetDamageType(ResistanceType.Physical, 100);
            
            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 20);
            SetResistance(ResistanceType.Cold, 20);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 20);
            
            SetSkill(SkillName.Swords, 50.0);
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

        public EvolutionMercenary(Serial serial) : base(serial) { }

        public void CheckEvolution()
        {
            SetAppearance();
        }

        public void SetAppearance()
        {
            int level = Level;
            Name = $"Evolution Mercenary (Level {level})";
            BodyValue = 400;
            Female = false;

            AddItem(new ShortPants());
            AddItem(new Shirt());
            
            SetStr(100 + (level * 20));
            SetDex(100 + (level * 20));
            SetInt(100 + (level * 20));
            SetHits(100 + (level * 20));
            SetStam(100 + (level * 20));
            SetMana(100 + (level * 20));
            SetDamage(10 + level, 15 + level);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_KillPoints);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_KillPoints = reader.ReadInt();
            SetAppearance();
        }
    }
}