using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Effects
{
    // Tests for composing moves with multiple effects
    public class MoveEffectCompositionTests
    {
        #region Single Effect Tests

        [Test]
        public void Test_Move_With_Damage_Only()
        {
            var tackle = new MoveData
            {
                Name = "Tackle",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 40,
                Effects = { new DamageEffect() }
            };

            Assert.Multiple(() =>
            {
                Assert.That(tackle.Effects, Has.Count.EqualTo(1));
                Assert.That(tackle.HasEffect<DamageEffect>(), Is.True);
                Assert.That(tackle.HasEffect<StatusEffect>(), Is.False);
            });
        }

        [Test]
        public void Test_Move_With_Status_Only()
        {
            var thunderWave = new MoveData
            {
                Name = "Thunder Wave",
                Type = PokemonType.Electric,
                Category = MoveCategory.Status,
                Power = 0,
                Effects = { new StatusEffect(PersistentStatus.Paralysis, 100) }
            };

            Assert.Multiple(() =>
            {
                Assert.That(thunderWave.Effects, Has.Count.EqualTo(1));
                Assert.That(thunderWave.HasEffect<StatusEffect>(), Is.True);
                Assert.That(thunderWave.HasEffect<DamageEffect>(), Is.False);
            });
        }

        #endregion

        #region Multi-Effect Tests

        [Test]
        public void Test_Move_With_Damage_And_Status()
        {
            // Flamethrower: Damage + 10% Burn
            var flamethrower = new MoveData
            {
                Name = "Flamethrower",
                Type = PokemonType.Fire,
                Category = MoveCategory.Special,
                Power = 90,
                Effects =
                {
                    new DamageEffect(),
                    new StatusEffect(PersistentStatus.Burn, 10)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(flamethrower.Effects, Has.Count.EqualTo(2));
                Assert.That(flamethrower.HasEffect<DamageEffect>(), Is.True);
                Assert.That(flamethrower.HasEffect<StatusEffect>(), Is.True);
                
                var statusEffect = flamethrower.GetEffect<StatusEffect>();
                Assert.That(statusEffect, Is.Not.Null);
                Assert.That(statusEffect.Status, Is.EqualTo(PersistentStatus.Burn));
                Assert.That(statusEffect.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Test_Move_With_Damage_And_StatChange()
        {
            // Psychic: Damage + 10% -1 SpDef
            var psychic = new MoveData
            {
                Name = "Psychic",
                Type = PokemonType.Psychic,
                Category = MoveCategory.Special,
                Power = 90,
                Effects =
                {
                    new DamageEffect(),
                    new StatChangeEffect(Stat.SpDefense, -1, targetSelf: false, chancePercent: 10)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(psychic.Effects, Has.Count.EqualTo(2));
                Assert.That(psychic.HasEffect<DamageEffect>(), Is.True);
                Assert.That(psychic.HasEffect<StatChangeEffect>(), Is.True);
                
                var statEffect = psychic.GetEffect<StatChangeEffect>();
                Assert.That(statEffect.TargetStat, Is.EqualTo(Stat.SpDefense));
                Assert.That(statEffect.Stages, Is.EqualTo(-1));
            });
        }

        [Test]
        public void Test_Move_With_Damage_And_Recoil()
        {
            // Double-Edge: Damage + 33% Recoil
            var doubleEdge = new MoveData
            {
                Name = "Double-Edge",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 120,
                Effects =
                {
                    new DamageEffect(),
                    new RecoilEffect(33)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(doubleEdge.Effects, Has.Count.EqualTo(2));
                Assert.That(doubleEdge.HasEffect<RecoilEffect>(), Is.True);
                Assert.That(doubleEdge.GetEffect<RecoilEffect>().RecoilPercent, Is.EqualTo(33));
            });
        }

        [Test]
        public void Test_Move_With_Damage_And_Drain()
        {
            // Giga Drain: Damage + 50% Drain
            var gigaDrain = new MoveData
            {
                Name = "Giga Drain",
                Type = PokemonType.Grass,
                Category = MoveCategory.Special,
                Power = 75,
                Effects =
                {
                    new DamageEffect(),
                    new DrainEffect(50)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(gigaDrain.Effects, Has.Count.EqualTo(2));
                Assert.That(gigaDrain.HasEffect<DrainEffect>(), Is.True);
                Assert.That(gigaDrain.GetEffect<DrainEffect>().DrainPercent, Is.EqualTo(50));
            });
        }

        [Test]
        public void Test_Move_With_Self_Buff()
        {
            // Swords Dance: +2 Attack to self
            var swordsDance = new MoveData
            {
                Name = "Swords Dance",
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                Power = 0,
                TargetScope = TargetScope.Self,
                Effects =
                {
                    new StatChangeEffect(Stat.Attack, 2, targetSelf: true)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(swordsDance.Effects, Has.Count.EqualTo(1));
                var effect = swordsDance.GetEffect<StatChangeEffect>();
                Assert.That(effect.TargetSelf, Is.True);
                Assert.That(effect.Stages, Is.EqualTo(2));
            });
        }

        [Test]
        public void Test_Move_With_Damage_And_Flinch()
        {
            // Air Slash: Damage + 30% Flinch
            var airSlash = new MoveData
            {
                Name = "Air Slash",
                Type = PokemonType.Flying,
                Category = MoveCategory.Special,
                Power = 75,
                Effects =
                {
                    new DamageEffect(),
                    new FlinchEffect(30)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(airSlash.Effects, Has.Count.EqualTo(2));
                Assert.That(airSlash.HasEffect<FlinchEffect>(), Is.True);
                Assert.That(airSlash.GetEffect<FlinchEffect>().ChancePercent, Is.EqualTo(30));
            });
        }

        [Test]
        public void Test_Move_With_MultiHit()
        {
            // Fury Attack: Damage + 2-5 hits
            var furyAttack = new MoveData
            {
                Name = "Fury Attack",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 15,
                Effects =
                {
                    new DamageEffect(),
                    new MultiHitEffect(2, 5)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(furyAttack.Effects, Has.Count.EqualTo(2));
                var multiHit = furyAttack.GetEffect<MultiHitEffect>();
                Assert.That(multiHit.MinHits, Is.EqualTo(2));
                Assert.That(multiHit.MaxHits, Is.EqualTo(5));
            });
        }

        #endregion

        #region Complex Composition Tests

        [Test]
        public void Test_Move_With_Three_Effects()
        {
            // Thunder: Damage + 30% Paralysis + always hits in Rain (we'll just test 3 effects)
            var thunder = new MoveData
            {
                Name = "Thunder",
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Power = 110,
                Accuracy = 70,
                Effects =
                {
                    new DamageEffect(),
                    new StatusEffect(PersistentStatus.Paralysis, 30),
                    new FlinchEffect(0) // placeholder for weather effect
                }
            };

            Assert.That(thunder.Effects, Has.Count.EqualTo(3));
        }

        [Test]
        public void Test_GetEffect_Returns_Null_When_Not_Found()
        {
            var tackle = new MoveData
            {
                Name = "Tackle",
                Effects = { new DamageEffect() }
            };

            Assert.Multiple(() =>
            {
                Assert.That(tackle.GetEffect<StatusEffect>(), Is.Null);
                Assert.That(tackle.GetEffect<RecoilEffect>(), Is.Null);
            });
        }

        [Test]
        public void Test_Empty_Effects_List()
        {
            var splash = new MoveData
            {
                Name = "Splash",
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                Power = 0
                // No effects - Splash does nothing!
            };

            Assert.Multiple(() =>
            {
                Assert.That(splash.Effects, Is.Empty);
                Assert.That(splash.HasEffect<DamageEffect>(), Is.False);
            });
        }

        #endregion
    }
}

