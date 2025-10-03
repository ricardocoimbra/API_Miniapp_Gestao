using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Config
{
    [ExcludeFromCodeCoverage]
    public class CacheConfig
    {
        public string Connection { get; set; } = string.Empty;
        public int TTLPredecessao { get; set; }
        public int TTLPadrao { get; set; }
    }
}
