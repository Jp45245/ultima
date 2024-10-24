using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class UnbindingDeed : Item
    {
        [Constructable]
        public UnbindingDeed() : base(0x14F0)
        {
            Name = "Unbinding Deed";
            Hue = 1161;
            LootType = LootType.Blessed;
        }

        public UnbindingDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
                return;
            }

            from.SendMessage("Target the bonded pet you wish to unbind.");
            from.Target = new UnbindingTarget(this);
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

        private class UnbindingTarget : Target
        {
            private UnbindingDeed m_Deed;

            public UnbindingTarget(UnbindingDeed deed) : base(10, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Deed.Deleted)
                    return;

                if (targeted is BaseCreature creature)
                {
                    if (creature.IsBonded && creature.ControlMaster == from)
                    {
                        creature.IsBonded = false;
                        creature.ControlMaster = null;
                        creature.SetControlMaster(null);
                        m_Deed.Delete();
                        from.SendMessage("The pet has been successfully unbound.");
                    }
                    else
                    {
                        from.SendMessage("That is not your bonded pet.");
                    }
                }
                else
                {
                    from.SendMessage("That is not a pet.");
                }
            }
        }
    }
}

