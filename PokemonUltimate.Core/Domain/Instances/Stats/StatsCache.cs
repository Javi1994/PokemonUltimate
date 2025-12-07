using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Domain.Instances.Stats
{
    /// <summary>
    /// Cache for calculated Pokemon stats.
    /// Invalidates when level, nature, or species changes.
    /// </summary>
    internal class StatsCache
    {
        private int? _cachedLevel;
        private Nature? _cachedNature;
        private PokemonSpeciesData _cachedSpecies;

        private int? _cachedMaxHP;
        private int? _cachedAttack;
        private int? _cachedDefense;
        private int? _cachedSpAttack;
        private int? _cachedSpDefense;
        private int? _cachedSpeed;

        /// <summary>
        /// Checks if the cache is valid for the given parameters.
        /// </summary>
        public bool IsValid(int level, Nature nature, PokemonSpeciesData species)
        {
            return _cachedLevel == level &&
                   _cachedNature == nature &&
                   _cachedSpecies == species;
        }

        /// <summary>
        /// Invalidates the cache, forcing recalculation on next access.
        /// </summary>
        public void Invalidate()
        {
            _cachedLevel = null;
            _cachedNature = null;
            _cachedSpecies = null;
            _cachedMaxHP = null;
            _cachedAttack = null;
            _cachedDefense = null;
            _cachedSpAttack = null;
            _cachedSpDefense = null;
            _cachedSpeed = null;
        }

        /// <summary>
        /// Gets the cached MaxHP or null if not cached.
        /// </summary>
        public int? GetMaxHP() => _cachedMaxHP;

        /// <summary>
        /// Gets the cached Attack or null if not cached.
        /// </summary>
        public int? GetAttack() => _cachedAttack;

        /// <summary>
        /// Gets the cached Defense or null if not cached.
        /// </summary>
        public int? GetDefense() => _cachedDefense;

        /// <summary>
        /// Gets the cached SpAttack or null if not cached.
        /// </summary>
        public int? GetSpAttack() => _cachedSpAttack;

        /// <summary>
        /// Gets the cached SpDefense or null if not cached.
        /// </summary>
        public int? GetSpDefense() => _cachedSpDefense;

        /// <summary>
        /// Gets the cached Speed or null if not cached.
        /// </summary>
        public int? GetSpeed() => _cachedSpeed;

        /// <summary>
        /// Sets all cached stats and marks cache as valid for the given parameters.
        /// </summary>
        public void SetStats(int level, Nature nature, PokemonSpeciesData species,
            int maxHP, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            _cachedLevel = level;
            _cachedNature = nature;
            _cachedSpecies = species;
            _cachedMaxHP = maxHP;
            _cachedAttack = attack;
            _cachedDefense = defense;
            _cachedSpAttack = spAttack;
            _cachedSpDefense = spDefense;
            _cachedSpeed = speed;
        }
    }
}
