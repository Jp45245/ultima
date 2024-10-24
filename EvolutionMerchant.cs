using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    public class EvolutionMerchant : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        [Constructable]
        public EvolutionMerchant() : base("the Evolution Merchant")
        {
            SetSkill(SkillName.ItemID, 64.0, 100.0);
        }

        public EvolutionMerchant(Serial serial) : base(serial)
        {
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBEvolutionMerchant());
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

    public class SBEvolutionMerchant : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        private class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new TokenBuyInfo(typeof(EvolutionMercenaryEgg), "EvolutionMercenaryEgg"));
                Add(new TokenBuyInfo(typeof(EvolutionPetEgg), "EvolutionPetEgg"));
                Add(new TokenBuyInfo(typeof(SoulSword), "SoulSword"));
                Add(new TokenBuyInfo(typeof(SoulBow), "SoulBow"));
                Add(new TokenBuyInfo(typeof(UnbindingDeed), "UnbindingDeed"));
                Add(new TokenBuyInfo(typeof(PetLeash), "PetLeash"));
            }
        }

        private class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    public class TokenBuyInfo : GenericBuyInfo
    {
        private string m_ItemType;

        public TokenBuyInfo(Type type, string itemType) : base(type, 1, 20, 0, 0)
        {
            m_ItemType = itemType;
            Price = Config.GetItemPrice(m_ItemType);
        }

        public int GetPrice(Mobile from)
        {
            return Config.GetItemPrice(m_ItemType);
        }

        public bool CanBe(Mobile from)
        {
            int tokens = CountTokens(from);
            int price = Config.GetItemPrice(m_ItemType);

            if (tokens < price)
            {
                from.SendMessage($"You need {price} Evolution Tokens to purchase this item. You only have {tokens}.");
                return false;
            }

            return true;
        }

        public void OnPurchase(Mobile from)
        {
            int price = Config.GetItemPrice(m_ItemType);
            ConsumeTokens(from, price);
            from.SendMessage($"You spent {price} Evolution Tokens on your purchase.");
        }

        private int CountTokens(Mobile from)
        {
            int count = 0;
            foreach (Item item in from.Backpack.Items)
            {
                if (item.Name == "Evolution Token")
                    count += item.Amount;
            }
            return count;
        }

        private void ConsumeTokens(Mobile from, int amount)
        {
            int remaining = amount;
            List<Item> tokens = new List<Item>();

            foreach (Item item in from.Backpack.Items)
            {
                if (item.Name == "Evolution Token")
                    tokens.Add(item);
            }

            foreach (Item token in tokens)
            {
                if (token.Amount >= remaining)
                {
                    token.Amount -= remaining;
                    if (token.Amount <= 0)
                        token.Delete();
                    break;
                }
                else
                {
                    remaining -= token.Amount;
                    token.Delete();
                }
            }
        }
    }
}