namespace PokemonUltimate.Core.Interfaces
{
    // Contract for any object that has a unique String ID.
    public interface IIdentifiable
    {
        string Id { get; }
    }
}
