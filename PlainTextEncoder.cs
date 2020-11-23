using System;
using System.Text.Encodings.Web;

namespace Lombiq.DataTables
{
    public class PlainTextEncoder : TextEncoder
    {
        public override unsafe bool TryEncodeUnicodeScalar(
            int unicodeScalar,
            char* buffer,
            int bufferLength,
            out int numberOfCharactersWritten) =>
            throw new InvalidOperationException("This should never be called");

        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength) => -1;

        public override bool WillEncode(int unicodeScalar) => false;

        public override int MaxOutputCharactersPerInputCharacter => 1; // Doesn't matter.
    }
}
