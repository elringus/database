using System;
using System.Diagnostics.CodeAnalysis;

namespace Database
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : Exception
    {
        public NotFoundException () { }

        public NotFoundException (string message)
            : base(message) { }

        public NotFoundException (string message, Exception inner)
            : base(message, inner) { }
    }
}
