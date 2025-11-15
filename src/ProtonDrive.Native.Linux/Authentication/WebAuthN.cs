using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProtonDrive.Native.Authentication;

public static class WebAuthN
{
    public static bool IsAvailable => false; // WebAuthn not yet implemented for Linux

    public static Task<object> GetAssertionResponseAsync(
        object parameters,
        nint hWnd = 0,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("WebAuthn is not yet implemented for Linux");
    }
}
