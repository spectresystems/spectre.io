namespace Spectre.IO.Testing
{
    /// <summary>
    /// An implementation of a fake <see cref="IPlatform"/>.
    /// </summary>
    public sealed class FakePlatform : IPlatform
    {
        /// <inheritdoc/>
        public PlatformFamily Family { get; set; }

        /// <inheritdoc/>
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakePlatform"/> class.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <param name="is64Bit">if set to <c>true</c>, the platform is 64-bit.</param>
        public FakePlatform(PlatformFamily family, bool is64Bit = true)
        {
            Family = family;
            Is64Bit = is64Bit;
        }
    }
}