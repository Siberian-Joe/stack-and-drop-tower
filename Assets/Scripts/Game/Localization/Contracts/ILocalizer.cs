namespace Game.Localization.Contracts
{
    public interface ILocalizer
    {
        bool TryGet(string key, out string value);
        string Get(string key);
    }
}