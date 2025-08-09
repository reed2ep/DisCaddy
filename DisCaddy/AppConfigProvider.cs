using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisCaddy
{
    public interface IAppConfigProvider { Task<AppConfig> GetAsync(); }
    public sealed class AppConfigProvider : IAppConfigProvider
    {
        private readonly Lazy<Task<AppConfig>> _lazy;
        public AppConfigProvider() => _lazy = new(() => ConfigLoader.LoadAsync());
        public Task<AppConfig> GetAsync() => _lazy.Value;
    }
}
