using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.IO.Internal;

namespace Spectre.IO
{
    /// <summary>
    /// The file system globber.
    /// </summary>
    public sealed class Globber : IGlobber
    {
        private readonly GlobParser _parser;
        private readonly GlobVisitor _visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Globber"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        public Globber(IFileSystem fileSystem, IEnvironment environment)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            _parser = new GlobParser(environment);
            _visitor = new GlobVisitor(fileSystem, environment);
        }

        /// <inheritdoc/>
        public IEnumerable<Path> Match(string pattern, GlobberSettings settings)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (string.IsNullOrWhiteSpace(pattern))
            {
                return Enumerable.Empty<Path>();
            }

            // Make sure we got some settings.
            settings ??= new GlobberSettings();

            // Parse the pattern into an AST.
            var root = _parser.Parse(pattern, settings.Comparer ?? PathComparer.Default);

            // Visit all nodes in the parsed patterns and filter the result.
            return _visitor.Walk(root, settings)
                .Select(x => x.Path)
                .Distinct(settings.Comparer ?? PathComparer.Default);
        }
    }
}