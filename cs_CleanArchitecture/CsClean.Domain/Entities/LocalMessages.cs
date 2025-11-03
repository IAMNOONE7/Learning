using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClean.Domain.Entities
{
    // NOTE TO SELF: Domain is pure C# — no WPF, no file IO, no Excel libs.
    // Keep only business data + simple invariants.
    public sealed class LocalMessages
    {
        // NOTE: entity identity = Name (for now). If I later need a GUID, I'll add it.
        public string Name { get; }
        public IReadOnlyList<LocalMessageItem> Items { get; }

        public LocalMessages(string name, IEnumerable<LocalMessageItem> items)
        {
            // Guard rails so invalid state can't be constructed.
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("LocalMessage.Name must not be empty", nameof(name));

            Name = name.Trim();
            Items = items?.ToList() ?? new List<LocalMessageItem>();
        }
    }

    public sealed class LocalMessageItem
    {
        public int Index { get; }
        public string Message { get; }

        public LocalMessageItem(int index, string message)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Item message cannot be empty", nameof(message));

            Index = index;
            Message = message.Trim();
        }
    }
}
