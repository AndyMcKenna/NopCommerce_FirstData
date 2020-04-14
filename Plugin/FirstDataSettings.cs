using Nop.Core.Configuration;

namespace BitShift.Plugin.Payments.FirstData
{
    public class FirstDataSettings : ISettings
    {
        public string SandboxURL { get; set; }
        public string ProductionURL { get; set; }
    }
}
