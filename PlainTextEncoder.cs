using System;
using System.Text.Encodings.Web;

namespace Lombiq.DataTables
{
    public class PlainTextEncoder : TextEncoder
    {
        public unsafe override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            throw new InvalidOperationException("This should never be called");
        }

        public unsafe override int FindFirstCharacterToEncode(char* text, int textLength) => -1;

        public override bool WillEncode(int unicodeScalar) => false;

        public override int MaxOutputCharactersPerInputCharacter => 1; // Doesn't matter.
    }
}
