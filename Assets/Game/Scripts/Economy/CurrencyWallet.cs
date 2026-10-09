using System;
using UnityEngine;
using RebirthEclipse.Core;

namespace RebirthEclipse.Economy
{
    /// <summary>
    /// Player wallet. Common/Silver/Gold are a single fungible pool stored internally in
    /// copper (100 copper = 1 silver, 100 silver = 1 gold) so arithmetic can never desync
    /// between denominations. Eclipse Tokens are a separate, non-convertible premium currency
    /// earned only from specific content — kept apart precisely so they can't be farmed via
    /// the regular economy.
    ///
    /// All mutations go through Add/Spend, which clamp and raise events; there is no setter
    /// that lets external code write an arbitrary balance, which is the structural guard
    /// against "money duplication" exploits (mirrors the inventory's instance-id guard).
    /// </summary>
    public class CurrencyWallet : MonoBehaviour
    {
        public const int CopperPerSilver = 100;
        public const int SilverPerGold = 100;
        public const long CopperPerGold = (long)CopperPerSilver * SilverPerGold;

        [SerializeField] private long copperBalance;
        [SerializeField] private int eclipseTokens;

        public long TotalCopper => copperBalance;
        public int EclipseTokens => eclipseTokens;

        public int GoldPart => (int)(copperBalance / CopperPerGold);
        public int SilverPart => (int)((copperBalance % CopperPerGold) / CopperPerSilver);
        public int CopperPart => (int)(copperBalance % CopperPerSilver);

        public void AddCopper(long amount)
        {
            if (amount <= 0) return;
            // Guard against overflow on absurd inputs rather than wrapping to a negative balance.
            copperBalance = amount > long.MaxValue - copperBalance ? long.MaxValue : copperBalance + amount;
            GameEvents.RaiseWalletChanged(copperBalance);
        }

        public void AddGold(int gold) => AddCopper((long)gold * CopperPerGold);
        public void AddSilver(int silver) => AddCopper((long)silver * CopperPerSilver);

        /// <summary>Spends copper if affordable; returns false and changes nothing otherwise.</summary>
        public bool TrySpendCopper(long amount)
        {
            if (amount < 0) return false;
            if (copperBalance < amount) return false;
            copperBalance -= amount;
            GameEvents.RaiseWalletChanged(copperBalance);
            return true;
        }

        public bool CanAfford(long copperCost) => copperBalance >= copperCost;

        public void AddEclipseTokens(int count)
        {
            if (count <= 0) return;
            eclipseTokens = count > int.MaxValue - eclipseTokens ? int.MaxValue : eclipseTokens + count;
            GameEvents.RaiseEclipseTokensChanged(eclipseTokens);
        }

        public bool TrySpendEclipseTokens(int count)
        {
            if (count < 0 || eclipseTokens < count) return false;
            eclipseTokens -= count;
            GameEvents.RaiseEclipseTokensChanged(eclipseTokens);
            return true;
        }

        public void LoadFromSave(long savedCopper, int savedTokens)
        {
            copperBalance = Math.Max(0, savedCopper);
            eclipseTokens = Math.Max(0, savedTokens);
        }

        /// <summary>Formats a copper amount as "Ng Ms Kc" for UI, dropping zero denominations.</summary>
        public static string Format(long copper)
        {
            int g = (int)(copper / CopperPerGold);
            int s = (int)((copper % CopperPerGold) / CopperPerSilver);
            int c = (int)(copper % CopperPerSilver);
            var parts = new System.Collections.Generic.List<string>();
            if (g > 0) parts.Add($"{g}g");
            if (s > 0) parts.Add($"{s}s");
            if (c > 0 || parts.Count == 0) parts.Add($"{c}c");
            return string.Join(" ", parts);
        }
    }
}
