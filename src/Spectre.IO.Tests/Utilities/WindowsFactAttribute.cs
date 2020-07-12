using Spectre.IO.Internal;
using Xunit;

namespace Spectre.IO.Testing.Xunit
{
    public sealed class WindowsFactAttribute : FactAttribute
    {
        private static readonly PlatformFamily _family = EnvironmentHelper.GetPlatformFamily();

        public WindowsFactAttribute(string reason = null)
        {
            if (_family != PlatformFamily.Windows)
            {
                Skip = reason ?? "Windows test.";
            }
        }
    }
}