namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Contract for any object that has a unique String ID.
    /// </summary>
    public interface IIdentifiable
    {
        string Id { get; }
    }
}

