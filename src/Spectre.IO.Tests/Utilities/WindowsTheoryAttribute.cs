using Spectre.IO.Internal;
using Xunit;

namespace Spectre.IO.Testing.Xunit
{
    public sealed class WindowsTheoryAttribute : TheoryAttribute
    {
        private static readonly PlatformFamily _family = EnvironmentHelper.GetPlatformFamily();

        public WindowsTheoryAttribute(string reason = null)
        {
            if (_family != PlatformFamily.Windows)
            {
                Skip = reason ?? "Windows test.";
            }
        }
    }
}