using System;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class CustomPetBondingPotion : Item
    {
        [Constructable]
        public CustomPetBondingPotion() : base(0xF0E)
        {
            Name = "Custom Pet Bonding Potion";
            Hue = 0x48D;
        }

        public CustomPetBondingPotion(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.SendMessage("Select the pet you wish to bond with.");
            from.Target = new BondingTarget(this);
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

        private class BondingTarget : Target
        {
            private CustomPetBondingPotion m_Potion;

            public BondingTarget(CustomPetBondingPotion potion) : base(10, false, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)targeted;

                    if (pet.ControlMaster == from)
                    {
                        pet.IsBonded = true;
                        from.SendMessage("You have successfully bonded with your pet!");
                        m_Potion.Delete();
                    }
                    else
                    {
                        from.SendMessage("That is not your pet.");
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