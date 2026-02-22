using Game.Startup.Contracts;

namespace Game.Startup.Runtime
{
    public sealed class AppStartup : StartupBase<IAppStartupTask>, IAppStartup
    {
        public AppStartup(StartupPipeline<IAppStartupTask> pipeline) : base(pipeline)
        {
        }
    }
}