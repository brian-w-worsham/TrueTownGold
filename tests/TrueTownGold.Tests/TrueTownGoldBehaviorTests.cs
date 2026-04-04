using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;
using Xunit;

namespace TrueTownGold.Tests
{
    public class TrueTownGoldBehaviorTests
    {
        [Fact]
        public void Behavior_CanBeInstantiated()
        {
            var behavior = new TrueTownGoldBehavior();

            Assert.NotNull(behavior);
        }

        [Fact]
        public void Behavior_InheritsFromCampaignBehaviorBase()
        {
            Assert.True(typeof(CampaignBehaviorBase).IsAssignableFrom(typeof(TrueTownGoldBehavior)));
        }

        [Fact]
        public void Behavior_IsPublic()
        {
            Assert.True(typeof(TrueTownGoldBehavior).IsPublic);
        }

        [Fact]
        public void Behavior_IsInRootNamespace()
        {
            Assert.Equal("TrueTownGold", typeof(TrueTownGoldBehavior).Namespace);
        }

        [Fact]
        public void Behavior_HasOnDailyTickTownMethodWithTownParameter()
        {
            MethodInfo method = typeof(TrueTownGoldBehavior).GetMethod(
                "OnDailyTickTown",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);
            Assert.Equal(typeof(void), method.ReturnType);
            Assert.Single(method.GetParameters());
            Assert.Equal(typeof(Town), method.GetParameters()[0].ParameterType);
        }

        [Fact]
        public void Behavior_EnsureTownHasTradeGold_NullTown_ReturnsFalse()
        {
            bool changed = TrueTownGoldBehavior.EnsureTownHasTradeGold(null);

            Assert.False(changed);
        }

        [Fact]
        public void Behavior_RegisterEventsMethodExists()
        {
            MethodInfo method = typeof(TrueTownGoldBehavior).GetMethod(
                "RegisterEvents",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.NotNull(method);
        }

        [Fact]
        public void Behavior_SyncDataMethodExists()
        {
            MethodInfo method = typeof(TrueTownGoldBehavior).GetMethod(
                "SyncData",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.NotNull(method);
        }

        [Fact]
        public void SubModule_InheritsFromMBSubModuleBase()
        {
            Assert.True(typeof(MBSubModuleBase).IsAssignableFrom(typeof(SubModule)));
        }

        [Fact]
        public void SubModule_IsPublic()
        {
            Assert.True(typeof(SubModule).IsPublic);
        }

        [Fact]
        public void SubModule_IsInRootNamespace()
        {
            Assert.Equal("TrueTownGold", typeof(SubModule).Namespace);
        }

        [Fact]
        public void SubModule_HasHarmonyField()
        {
            FieldInfo field = typeof(SubModule).GetField(
                "_harmony",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(field);
            Assert.Equal("HarmonyLib.Harmony", field.FieldType.FullName);
        }

        [Fact]
        public void SubModule_HarmonyField_IsNullBeforeLoad()
        {
            var subModule = new SubModule();
            FieldInfo field = typeof(SubModule).GetField(
                "_harmony",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.Null(field.GetValue(subModule));
        }

        [Fact]
        public void SubModule_OverridesOnSubModuleLoad()
        {
            MethodInfo method = typeof(SubModule).GetMethod(
                "OnSubModuleLoad",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);
            Assert.True(method.IsFamily || method.IsFamilyOrAssembly);
        }

        [Fact]
        public void SubModule_OverridesOnSubModuleUnloaded()
        {
            MethodInfo method = typeof(SubModule).GetMethod(
                "OnSubModuleUnloaded",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);
            Assert.True(method.IsFamily || method.IsFamilyOrAssembly);
        }

        [Fact]
        public void SubModule_OverridesOnGameStart()
        {
            MethodInfo method = typeof(SubModule).GetMethod(
                "OnGameStart",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);
            Assert.True(method.IsFamily || method.IsFamilyOrAssembly);
        }

        [Fact]
        public void Assembly_ExposesInternalsToTests()
        {
            var attributes = typeof(SubModule).Assembly
                .GetCustomAttributes<System.Runtime.CompilerServices.InternalsVisibleToAttribute>()
                .ToArray();

            Assert.Contains(attributes, attribute => attribute.AssemblyName == "TrueTownGold.Tests");
        }

        [Fact]
        public void Calculator_UsesPositiveMultiplierAndBounds()
        {
            Assert.True(TownGoldCalculator.GoldPerProsperity > 0f);
            Assert.True(TownGoldCalculator.MinimumTownGold > 0);
            Assert.True(TownGoldCalculator.MaximumTownGold >= TownGoldCalculator.MinimumTownGold);
        }
    }
}