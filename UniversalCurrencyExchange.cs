using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class UniversalCurrencyExchanger : Item
    {
        [Constructable]
        public UniversalCurrencyExchanger() : base(0xE34)
        {
            Movable = false;
            Name = "Universal Currency Exchanger";
        }

        public UniversalCurrencyExchanger(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }
            from.SendGump(new TokenExchangeGump(from));
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

    public class TokenExchangeGump : Gump
    {
        private Mobile m_From;

        public TokenExchangeGump(Mobile from) : base(50, 50)
        {
            m_From = from;

            AddPage(0);
            AddBackground(0, 0, 300, 200, 9200);
            AddAlphaRegion(10, 10, 280, 180);

            AddHtml(20, 20, 260, 60, $"Exchange Rate: {Config.TokenExchangeRate} gold = 1 Evolution Token", false, false);
            
            AddButton(20, 90, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(55, 90, 0x480, "Buy 1 Token");
            
            AddButton(20, 120, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddLabel(55, 120, 0x480, "Buy 5 Tokens");
            
            AddButton(20, 150, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddLabel(55, 150, 0x480, "Buy 10 Tokens");
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0) return;

            int amount = info.ButtonID;
            int totalCost = amount * Config.TokenExchangeRate;

            if (m_From.Backpack.ConsumeTotal(typeof(Gold), totalCost))
            {
                Item token = new Item(0xEED);
                token.Name = "Evolution Token";
                token.Stackable = true;
                token.Amount = amount;
                token.Hue = 1161;
                token.LootType = LootType.Blessed;

                m_From.AddToBackpack(token);
                m_From.SendMessage($"You purchased {amount} Evolution Token{(amount == 1 ? "" : "s")} for {totalCost} gold.");
            }
            else
            {
                m_From.SendMessage($"You need {totalCost} gold to purchase {amount} Evolution Token{(amount == 1 ? "" : "s")}.");
            }
        }
    }
}