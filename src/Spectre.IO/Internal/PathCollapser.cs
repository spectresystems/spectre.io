using System;
using System.Collections.Generic;
using System.Linq;

namespace Spectre.IO.Internal
{
    internal static class PathCollapser
    {
        public static string Collapse(Path path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var stack = new Stack<string>();
            var segments = path.FullPath.Split('/', '\\');
            foreach (var segment in segments)
            {
                if (segment == ".")
                {
                    continue;
                }

                if (segment == "..")
                {
                    if (stack.Count > 1)
                    {
                        stack.Pop();
                    }

                    continue;
                }

                stack.Push(segment);
            }

            string collapsed = string.Join("/", stack.Reverse());
            return string.IsNullOrEmpty(collapsed) ? "." : collapsed;
        }
    }
}