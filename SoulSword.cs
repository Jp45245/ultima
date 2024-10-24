using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SoulSword : BaseSword
    {
        private int m_KillPoints;
        private int m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public int KillPoints
        {
            get { return m_KillPoints; }
            set { m_KillPoints = value; CheckLevel(); InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = Math.Max(0, Math.Min(5, value)); UpdateStats(); InvalidateProperties(); }
        }

        [Constructable]
        public SoulSword() : base(0xF5E)
        {
            Name = "Soul Sword";
            Hue = 1161;
            LootType = LootType.Blessed;
            m_Level = 0;
            m_KillPoints = 0;
            UpdateStats();
        }

        public SoulSword(Serial serial) : base(serial)
        {
        }

        public void CheckLevel()
        {
            int newLevel = m_KillPoints / 1000;
            if (newLevel != m_Level && newLevel <= 5)
            {
                Level = newLevel;
                if (RootParent is Mobile owner)
                    owner.SendMessage($"Your Soul Sword has reached level {Level}!");
            }
        }

        private void UpdateStats()
        {
            WeaponAttributes.HitLeechHits = 5 + (Level * 5);
            WeaponAttributes.HitLeechMana = 5 + (Level * 5);
            WeaponAttributes.HitLeechStam = 5 + (Level * 5);
            Attributes.WeaponDamage = 15 + (Level * 10);
            Attributes.WeaponSpeed = 10 + (Level * 5);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add($"Level {Level}");
            list.Add($"Kill Points: {KillPoints}/5000");
        }

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (defender is Mobile m && m.Hits <= 0 && !m.Player)
            {
                KillPoints += 10;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_KillPoints);
            writer.Write(m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_KillPoints = reader.ReadInt();
            m_Level = reader.ReadInt();
            UpdateStats();
        }
    }
}