using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Economy;

namespace RebirthEclipse.Tests
{
    public class EconomyTests
    {
        private GameObject go;
        private CurrencyWallet wallet;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("Wallet");
            wallet = go.AddComponent<CurrencyWallet>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(go);

        [Test]
        public void AddGold_ConvertsToCopper_AtCorrectRate()
        {
            wallet.AddGold(1);
            Assert.AreEqual(CurrencyWallet.CopperPerGold, wallet.TotalCopper);
            Assert.AreEqual(1, wallet.GoldPart);
            Assert.AreEqual(0, wallet.SilverPart);
            Assert.AreEqual(0, wallet.CopperPart);
        }

        [Test]
        public void DenominationBreakdown_IsCorrect()
        {
            wallet.AddCopper(1 * CurrencyWallet.CopperPerGold + 23 * CurrencyWallet.CopperPerSilver + 45);
            Assert.AreEqual(1, wallet.GoldPart);
            Assert.AreEqual(23, wallet.SilverPart);
            Assert.AreEqual(45, wallet.CopperPart);
        }

        [Test]
        public void SpendMoreThanBalance_FailsAndLeavesBalanceUnchanged()
        {
            wallet.AddCopper(50);
            bool spent = wallet.TrySpendCopper(100);
            Assert.IsFalse(spent);
            Assert.AreEqual(50, wallet.TotalCopper);
        }

        [Test]
        public void SpendExactBalance_Succeeds_AndZeroesWallet()
        {
            wallet.AddCopper(100);
            Assert.IsTrue(wallet.TrySpendCopper(100));
            Assert.AreEqual(0, wallet.TotalCopper);
        }

        [Test]
        public void EclipseTokens_AreSeparateFromCopper_AndNotConvertible()
        {
            wallet.AddCopper(500);
            wallet.AddEclipseTokens(3);
            Assert.AreEqual(500, wallet.TotalCopper);
            Assert.AreEqual(3, wallet.EclipseTokens);

            // Spending tokens must not touch copper and vice versa.
            Assert.IsTrue(wallet.TrySpendEclipseTokens(2));
            Assert.AreEqual(1, wallet.EclipseTokens);
            Assert.AreEqual(500, wallet.TotalCopper);
        }

        [Test]
        public void NegativeOrZeroAdds_AreIgnored_NoWayToInflateBalanceByArithmetic()
        {
            wallet.AddCopper(-9999);
            wallet.AddCopper(0);
            wallet.AddEclipseTokens(-5);
            Assert.AreEqual(0, wallet.TotalCopper);
            Assert.AreEqual(0, wallet.EclipseTokens);
        }

        [Test]
        public void AddCopper_NearOverflow_ClampsInsteadOfWrappingNegative()
        {
            wallet.LoadFromSave(long.MaxValue - 10, 0);
            wallet.AddCopper(1000);
            Assert.AreEqual(long.MaxValue, wallet.TotalCopper);
            Assert.Greater(wallet.TotalCopper, 0, "Balance must never wrap to a negative via overflow.");
        }

        [Test]
        public void Format_DropsZeroDenominations()
        {
            Assert.AreEqual("5g 10c", CurrencyWallet.Format(5 * CurrencyWallet.CopperPerGold + 10));
            Assert.AreEqual("0c", CurrencyWallet.Format(0));
        }
    }
}
