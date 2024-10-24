using System;
using System.Collections.Generic;
using Server;

namespace Server.Scripts.Commands
{
    public static class Config
    {
        private static int m_TokenExchangeRate = 1000; // Default: 1000 gold = 1 token
        private static Dictionary<string, int> m_ItemPrices = new Dictionary<string, int>()
        {
            { "EvolutionPetEgg", 10 },
            { "EvolutionMercenaryEgg", 15 },
            { "SoulSword", 20 },
            { "SoulBow", 20 },
            { "UnbindingDeed", 5 },
            { "PetLeash", 8 }
        };

        public static int TokenExchangeRate
        {
            get { return m_TokenExchangeRate; }
            set { m_TokenExchangeRate = Math.Max(1, value); }
        }

        public static int GetItemPrice(string itemType)
        {
            return m_ItemPrices.ContainsKey(itemType) ? m_ItemPrices[itemType] : 0;
        }

        public static void SetItemPrice(string itemType, int price)
        {
            if (m_ItemPrices.ContainsKey(itemType))
                m_ItemPrices[itemType] = Math.Max(0, price);
            else
                m_ItemPrices.Add(itemType, Math.Max(0, price));
        }
    }
}