namespace Cs2SuperPowers.Players;

public interface IValidation<in TEntity>
{
    bool Validate(TEntity? entity);
}