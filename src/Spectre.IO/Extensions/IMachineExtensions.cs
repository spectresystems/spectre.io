namespace Spectre.IO;

/// <summary>
/// Contains extensions for <see cref="IMachine"/>.
/// </summary>
public static class IMachineExtensions
{
    /// <summary>
    /// Creates a new <see cref="IGlobber"/> instance.
    /// </summary>
    /// <param name="machine">The machine to base the globber on.</param>
    /// <returns>A <see cref="IGlobber"/> instance.</returns>
    public static IGlobber CreateGlobber(this IMachine machine)
    {
        return new Globber(machine.FileSystem, machine.Environment);
    }
}