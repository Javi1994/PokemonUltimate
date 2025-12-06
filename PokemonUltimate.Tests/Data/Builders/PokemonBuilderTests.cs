using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using System.Linq;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonBuilder = PokemonUltimate.Content.Builders.Pokemon;

namespace PokemonUltimate.Tests.Data.Builders
{
    [TestFixture]
    public class PokemonBuilderTests
    {
        [Test]
        public void Define_Should_Set_Name_And_PokedexNumber()
        {
            var pokemon = PokemonBuilder.Define("Pikachu", 25).Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
                Assert.That(pokemon.PokedexNumber, Is.EqualTo(25));
            });
        }

        [Test]
        public void Type_Should_Set_Single_Type()
        {
            var pokemon = PokemonBuilder.Define("Pikachu", 25)
                .Type(PokemonType.Electric)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Electric));
                Assert.That(pokemon.SecondaryType, Is.Null);
                Assert.That(pokemon.IsDualType, Is.False);
            });
        }

        [Test]
        public void Types_Should_Set_Dual_Types()
        {
            var pokemon = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
                Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Flying));
                Assert.That(pokemon.IsDualType, Is.True);
            });
        }

        [Test]
        public void Stats_Should_Set_BaseStats()
        {
            var pokemon = PokemonBuilder.Define("Pikachu", 25)
                .Stats(35, 55, 40, 50, 50, 90)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.BaseStats.HP, Is.EqualTo(35));
                Assert.That(pokemon.BaseStats.Attack, Is.EqualTo(55));
                Assert.That(pokemon.BaseStats.Defense, Is.EqualTo(40));
                Assert.That(pokemon.BaseStats.SpAttack, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.SpDefense, Is.EqualTo(50));
                Assert.That(pokemon.BaseStats.Speed, Is.EqualTo(90));
            });
        }

        [Test]
        public void Moves_Should_Configure_Learnset()
        {
            var pokemon = PokemonBuilder.Define("Pikachu", 25)
                .Moves(m => m
                    .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.Growl)
                    .AtLevel(10, MoveCatalog.QuickAttack)
                    .ByTM(MoveCatalog.Thunderbolt))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.Learnset, Has.Count.EqualTo(4));
                Assert.That(pokemon.GetStartingMoves().Count(), Is.EqualTo(2));
                Assert.That(pokemon.CanLearn(MoveCatalog.ThunderShock), Is.True);
                Assert.That(pokemon.CanLearn(MoveCatalog.Thunderbolt), Is.True);
            });
        }

        [Test]
        public void EvolvesTo_Should_Add_Evolution()
        {
            var raichu = PokemonBuilder.Define("Raichu", 26)
                .Type(PokemonType.Electric)
                .Build();

            var pikachu = PokemonBuilder.Define("Pikachu", 25)
                .Type(PokemonType.Electric)
                .EvolvesTo(raichu, e => e.WithItem("Thunder Stone"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pikachu.CanEvolve, Is.True);
                Assert.That(pikachu.Evolutions, Has.Count.EqualTo(1));
                Assert.That(pikachu.Evolutions[0].Target, Is.EqualTo(raichu));
                Assert.That(pikachu.Evolutions[0].HasCondition<ItemCondition>(), Is.True);
            });
        }

        [Test]
        public void EvolvesTo_Should_Support_Multiple_Evolutions()
        {
            // Simulate Eevee with multiple evolution paths
            var vaporeon = new PokemonSpeciesData { Name = "Vaporeon", PokedexNumber = 134 };
            var jolteon = new PokemonSpeciesData { Name = "Jolteon", PokedexNumber = 135 };
            var flareon = new PokemonSpeciesData { Name = "Flareon", PokedexNumber = 136 };

            var eevee = PokemonBuilder.Define("Eevee", 133)
                .Type(PokemonType.Normal)
                .EvolvesTo(vaporeon, e => e.WithItem("Water Stone"))
                .EvolvesTo(jolteon, e => e.WithItem("Thunder Stone"))
                .EvolvesTo(flareon, e => e.WithItem("Fire Stone"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(eevee.Evolutions, Has.Count.EqualTo(3));
                Assert.That(eevee.Evolutions[0].Target.Name, Is.EqualTo("Vaporeon"));
                Assert.That(eevee.Evolutions[1].Target.Name, Is.EqualTo("Jolteon"));
                Assert.That(eevee.Evolutions[2].Target.Name, Is.EqualTo("Flareon"));
            });
        }

        [Test]
        public void EvolvesTo_Should_Support_Complex_Conditions()
        {
            var espeon = new PokemonSpeciesData { Name = "Espeon", PokedexNumber = 196 };

            var eevee = PokemonBuilder.Define("Eevee", 133)
                .Type(PokemonType.Normal)
                .EvolvesTo(espeon, e => e
                    .WithFriendship()
                    .During(TimeOfDay.Day))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(eevee.Evolutions[0].Conditions, Has.Count.EqualTo(2));
                Assert.That(eevee.Evolutions[0].HasCondition<FriendshipCondition>(), Is.True);
                Assert.That(eevee.Evolutions[0].HasCondition<TimeOfDayCondition>(), Is.True);
            });
        }

        [Test]
        public void Full_Builder_Chain_Should_Work()
        {
            var charizard = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
                    .AtLevel(36, MoveCatalog.Flamethrower)
                    .ByTM(MoveCatalog.Earthquake))
                .Build();

            var charmeleon = PokemonBuilder.Define("Charmeleon", 5)
                .Type(PokemonType.Fire)
                .Stats(58, 64, 58, 80, 65, 80)
                .Moves(m => m
                    .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                    .AtLevel(17, MoveCatalog.Ember))
                .EvolvesTo(charizard, e => e.AtLevel(36))
                .Build();

            Assert.Multiple(() =>
            {
                // Charizard checks
                Assert.That(charizard.Name, Is.EqualTo("Charizard"));
                Assert.That(charizard.IsDualType, Is.True);
                Assert.That(charizard.BaseStats.Total, Is.EqualTo(534));
                Assert.That(charizard.CanEvolve, Is.False);

                // Charmeleon checks
                Assert.That(charmeleon.Name, Is.EqualTo("Charmeleon"));
                Assert.That(charmeleon.IsDualType, Is.False);
                Assert.That(charmeleon.CanEvolve, Is.True);
                Assert.That(charmeleon.Evolutions[0].Target, Is.EqualTo(charizard));
                Assert.That(charmeleon.Evolutions[0].GetCondition<LevelCondition>().MinLevel, Is.EqualTo(36));
            });
        }

        #region Gender Builder Tests

        [Test]
        public void GenderRatio_Should_Set_Percentage()
        {
            var pokemon = PokemonBuilder.Define("Charmander", 4)
                .GenderRatio(87.5f)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.GenderRatio, Is.EqualTo(87.5f));
                Assert.That(pokemon.HasBothGenders, Is.True);
            });
        }

        [Test]
        public void Genderless_Should_Set_Negative_Ratio()
        {
            var pokemon = PokemonBuilder.Define("Magnemite", 81)
                .Genderless()
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.GenderRatio, Is.EqualTo(-1f));
                Assert.That(pokemon.IsGenderless, Is.True);
            });
        }

        [Test]
        public void MaleOnly_Should_Set_100_Ratio()
        {
            var pokemon = PokemonBuilder.Define("Tauros", 128)
                .MaleOnly()
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.GenderRatio, Is.EqualTo(100f));
                Assert.That(pokemon.IsMaleOnly, Is.True);
            });
        }

        [Test]
        public void FemaleOnly_Should_Set_0_Ratio()
        {
            var pokemon = PokemonBuilder.Define("Chansey", 113)
                .FemaleOnly()
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(pokemon.GenderRatio, Is.EqualTo(0f));
                Assert.That(pokemon.IsFemaleOnly, Is.True);
            });
        }

        [Test]
        public void Default_GenderRatio_Is_50()
        {
            var pokemon = PokemonBuilder.Define("Pikachu", 25).Build();

            Assert.That(pokemon.GenderRatio, Is.EqualTo(50f));
        }

        #endregion

        #region Variant Tests

        [Test]
        public void AsMegaVariant_Should_Set_BaseForm_And_VariantType()
        {
            var charizard = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Build();

            var megaCharizardX = PokemonBuilder.Define("Mega Charizard X", 6)
                .Types(PokemonType.Fire, PokemonType.Dragon)
                .Stats(78, 130, 111, 130, 85, 100)
                .AsMegaVariant(charizard)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(megaCharizardX.BaseForm, Is.EqualTo(charizard));
                Assert.That(megaCharizardX.VariantType, Is.EqualTo(PokemonVariantType.Mega));
                Assert.That(megaCharizardX.IsMegaVariant, Is.True);
                Assert.That(megaCharizardX.IsVariant, Is.True);
                Assert.That(charizard.Variants, Contains.Item(megaCharizardX));
            });
        }

        [Test]
        public void AsDinamaxVariant_Should_Set_BaseForm_And_VariantType()
        {
            var charizard = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Build();

            var charizardDinamax = PokemonBuilder.Define("Charizard Dinamax", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(156, 84, 78, 109, 85, 100) // HP doubled
                .AsDinamaxVariant(charizard)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(charizardDinamax.BaseForm, Is.EqualTo(charizard));
                Assert.That(charizardDinamax.VariantType, Is.EqualTo(PokemonVariantType.Dinamax));
                Assert.That(charizardDinamax.IsDinamaxVariant, Is.True);
                Assert.That(charizardDinamax.IsVariant, Is.True);
                Assert.That(charizard.Variants, Contains.Item(charizardDinamax));
            });
        }

        [Test]
        public void AsTeraVariant_Should_Set_BaseForm_VariantType_And_TeraType()
        {
            var charizard = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Build();

            var charizardTeraFire = PokemonBuilder.Define("Charizard Tera Fire", 6)
                .Type(PokemonType.Fire) // Mono-type
                .Stats(78, 84, 78, 109, 85, 100)
                .AsTeraVariant(charizard, PokemonType.Fire)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(charizardTeraFire.BaseForm, Is.EqualTo(charizard));
                Assert.That(charizardTeraFire.VariantType, Is.EqualTo(PokemonVariantType.Tera));
                Assert.That(charizardTeraFire.TeraType, Is.EqualTo(PokemonType.Fire));
                Assert.That(charizardTeraFire.IsTeraVariant, Is.True);
                Assert.That(charizardTeraFire.IsVariant, Is.True);
                Assert.That(charizard.Variants, Contains.Item(charizardTeraFire));
            });
        }

        [Test]
        public void AsMegaVariant_With_Null_BaseForm_Should_Throw()
        {
            var builder = PokemonBuilder.Define("Mega Charizard X", 6)
                .Types(PokemonType.Fire, PokemonType.Dragon);

            Assert.Throws<System.ArgumentNullException>(() => builder.AsMegaVariant(null));
        }

        [Test]
        public void AsDinamaxVariant_With_Null_BaseForm_Should_Throw()
        {
            var builder = PokemonBuilder.Define("Charizard Dinamax", 6);

            Assert.Throws<System.ArgumentNullException>(() => builder.AsDinamaxVariant(null));
        }

        [Test]
        public void AsTeraVariant_With_Null_BaseForm_Should_Throw()
        {
            var builder = PokemonBuilder.Define("Charizard Tera Fire", 6);

            Assert.Throws<System.ArgumentNullException>(() => builder.AsTeraVariant(null, PokemonType.Fire));
        }

        [Test]
        public void Multiple_Variants_Should_Be_Added_To_BaseForm_Variants_List()
        {
            var charizard = PokemonBuilder.Define("Charizard", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 84, 78, 109, 85, 100)
                .Build();

            var megaCharizardX = PokemonBuilder.Define("Mega Charizard X", 6)
                .Types(PokemonType.Fire, PokemonType.Dragon)
                .Stats(78, 130, 111, 130, 85, 100)
                .AsMegaVariant(charizard)
                .Build();

            var megaCharizardY = PokemonBuilder.Define("Mega Charizard Y", 6)
                .Types(PokemonType.Fire, PokemonType.Flying)
                .Stats(78, 104, 78, 159, 115, 100)
                .AsMegaVariant(charizard)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(charizard.Variants, Has.Count.EqualTo(2));
                Assert.That(charizard.Variants, Contains.Item(megaCharizardX));
                Assert.That(charizard.Variants, Contains.Item(megaCharizardY));
            });
        }

        [Test]
        public void AsRegionalVariant_Should_Set_BaseForm_VariantType_And_RegionalForm()
        {
            var vulpix = PokemonBuilder.Define("Vulpix", 37)
                .Type(PokemonType.Fire)
                .Stats(38, 41, 40, 50, 65, 65)
                .Build();

            var alolanVulpix = PokemonBuilder.Define("Alolan Vulpix", 37)
                .Type(PokemonType.Ice) // Different type
                .Stats(38, 41, 40, 50, 65, 65)
                .AsRegionalVariant(vulpix, "Alola")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(alolanVulpix.BaseForm, Is.EqualTo(vulpix));
                Assert.That(alolanVulpix.VariantType, Is.EqualTo(PokemonVariantType.Regional));
                Assert.That(alolanVulpix.RegionalForm, Is.EqualTo("Alola"));
                Assert.That(alolanVulpix.IsRegionalVariant, Is.True);
                Assert.That(alolanVulpix.IsVariant, Is.True);
                Assert.That(vulpix.Variants, Contains.Item(alolanVulpix));
            });
        }

        [Test]
        public void AsRegionalVariant_With_Null_BaseForm_Should_Throw()
        {
            var builder = PokemonBuilder.Define("Alolan Vulpix", 37);

            Assert.Throws<System.ArgumentNullException>(() => builder.AsRegionalVariant(null, "Alola"));
        }

        [Test]
        public void AsRegionalVariant_With_Empty_Region_Should_Throw()
        {
            var vulpix = PokemonBuilder.Define("Vulpix", 37).Build();
            var builder = PokemonBuilder.Define("Alolan Vulpix", 37);

            Assert.Throws<System.ArgumentException>(() => builder.AsRegionalVariant(vulpix, null));
            Assert.Throws<System.ArgumentException>(() => builder.AsRegionalVariant(vulpix, ""));
            Assert.Throws<System.ArgumentException>(() => builder.AsRegionalVariant(vulpix, "   "));
        }

        #endregion
    }
}

