using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Localization.Contracts
{
    public interface ILocalizationProvider
    {
        UniTask<ILocalizationDefinition> LoadAsync(CancellationToken token);
    }
}