namespace ProtonDrive.Native.Authentication.Contracts;

/// <summary>
/// FIDO2 authentication options for Linux
/// </summary>
public class Fido2AuthenticationOptions
{
    public string? RelyingPartyId { get; set; }
    public byte[]? Challenge { get; set; }
    public int TimeoutMilliseconds { get; set; } = 60000;
}

/// <summary>
/// Public key credential request options
/// </summary>
public class PublicKeyCredentialRequestOptions
{
    public byte[]? Challenge { get; set; }
    public string? RpId { get; set; }
    public List<PublicKeyCredentialDescriptor>? AllowCredentials { get; set; }
}

/// <summary>
/// Public key credential descriptor
/// </summary>
public class PublicKeyCredentialDescriptor
{
    public string? Type { get; set; }
    public byte[]? Id { get; set; }
}

/// <summary>
/// WebAuthn client data
/// </summary>
public class WebAuthNClientData
{
    public string? Type { get; set; }
    public string? Challenge { get; set; }
    public string? Origin { get; set; }
}
