namespace PokemonUltimate.Core.Enums
{
    // Defines who a move can target during battle.
    public enum TargetScope
    {
        // Targets the user (Swords Dance, Recover)
        Self,
        
        // Targets a single enemy (Tackle, Thunderbolt)
        SingleEnemy,
        
        // Targets a single ally (Helping Hand)
        SingleAlly,
        
        // Targets all enemies (Earthquake, Surf)
        AllEnemies,
        
        // Targets all allies (rare)
        AllAllies,
        
        // Targets everyone except user (Explosion)
        AllOthers,
        
        // Targets all adjacent Pokemon including allies (Earthquake in doubles)
        AllAdjacent,
        
        // Targets all adjacent enemies only (Razor Leaf, Heat Wave)
        AllAdjacentEnemies,
        
        // Can target anyone (Telekinesis)
        Any,
        
        // Targets a random enemy (Outrage, Petal Dance)
        RandomEnemy,
        
        // Affects the entire field (Stealth Rock, Weather)
        Field,
        
        // Targets user and all allies (Aromatherapy, Heal Bell)
        UserAndAllies
    }
}

