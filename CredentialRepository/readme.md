Credential Repository
---------------------
The Credential Repository provides a REST API based framework for storing credentials. Credentials are stored (encrypted) in a database using Entity Framework. This is based on the OWIN framework.

Encryption
----------
Encryption is done by using a certificate. In `CredentialRepositoryOptions`, the location and thumbprint of a certificate to be used for encryption/decryption is used. 

The API requires that the password you are trying store already be encrypted with a randomly generated key before sending to the API. The key is sent to the API as a Base64-encoded string, and then the certificate is used to encrypt the key. 

When accessing the `EncryptedKey` property of a `Credential` object, you will get the Base64-encoded string (non-encrypted); however, when it is stored in the database, it is stored in an ecrypted format.

Passwords are never decrypted (though, combined with decoding the Base64 key, this is possible).

API Usage
---------
The endpoint can be altered in the `CredentialRepositoryOptions`. The default endpoint is `/api/v2/credentials`. Standard Get/Post/Put/Delete methods are accepted.

Retrieving Credentials
======================
All credentials can be retrieved using a standard Get request:
> GET /api/v2/credentials

Specific credentials can be retrieved by passing the Id as a parameter:
> GET /api/v2/credentials

```powershell
PS> Invoke-RestMethod -Uri 'http://localhost/api/v2/credentials/1' -Method Get -UseDefaultCredentials -ContentType 'application/json'

CertificateThumbprint : 62553233752FA5772F793B80C7B4DA9C85E590A3
StoreLocation         : 2
StoreName             : My
EncryptedKey          : Yg==
EncryptedPassword     : b
EntityGuid            : 67a80dfe-15a0-4508-b54f-03bb2be52115
Id                    : 1
Name                  : test3
Username              : testUser
```

Inserting or Updating Credentials
=================================
Inserting credentials is done by submitting a POST with a JSON body for the credential:
> POST /api/v2/credentials
> 
> {
>     "Username":  "testUser",
>     "EncryptedKey":  "Yg==",
>     "Name":  "test3",
>     "EncryptedPassword":  "b"
> }

Updating is done in the same manner via a Put and passing the Id:
> PUT /api/v2/credentials/1
> 
> {
>     "Id":    1,
>     "Username":  "testUser",
>     "EncryptedKey":  "Yg==",
>     "Name":  "test3",
>     "EncryptedPassword":  "b"
> }

Usage
-----
In your OWIN configuration, use the following:

```csharp
var options = new CredentialRepositoryOptions(
    "62553233752FA5772F793B80C7B4DA9C85E590A3",
    StoreLocation.LocalMachine,
    "My",
    "CredentialRepositoryContext") { IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always, DatabaseInitializer = new ContextInitializer() };

app.UseCredentialRepository(options);
```

Database Connectivity
=====================
In the above, `CredentialRepositoryContext` refers to a connection string in your web/app config file:

```xml
<add name="CredentialRepositoryContext" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\ProgramData\TempDatabase\CredentialRepository.mdf;Integrated Security=True;Connect Timeout=30" providerName="System.Data.SqlClient" />
  </connectionStrings>
```

Instead of specifying the connection string in your configuration file, you can also pass it directly into the `CredentialRepositoryOptions`.

Since Entity Framework is being leveraged, any database compatible with EF6 can be used.

Permissions
===========
There is row-level security built into the stored credentials. The Credential Repository inherits whatever authorization scheme is used by the calling assembly. For example, if you are using Windows Integrated Authentication, this would be present in your OWIN configuration:

```csharp
var httpListener = (HttpListener)app.Properties["System.Net.HttpListener"];
httpListener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
```

By doing this, Credential Repository will read the credentials of the domain user.

By default, no one has permission to do anything. The ideal way to populate the initial permissions is to specify a database initializer, such as the following:

```csharp
public class ContextInitializer : DropCreateDatabaseAlways<CredentialRepositoryContext>
{
    /// <summary>
    /// A method that should be overridden to actually add data to the context for seeding.
    ///             The default implementation does nothing.
    /// </summary>
    /// <param name="context">The context to seed. </param>
    protected override void Seed(CredentialRepositoryContext context)
    {
        var permissions = new List<Permission> { new Permission { Access = AccessControlType.Allow, Action = PermissionActions.All, EntityGuid = new Guid(), Identity = "AD.Username", IdentityProvider = "DomainName", Model = "Credential"} };
        context.Permissions.AddRange(permissions);
        context.SaveChanges();
        base.Seed(context);
    }
}
```

This relies on the `Permission` object, which allows you to specify the following:
> Access: Allow or Deny
> Action: Actions such as insert, delete, read, etc.
> EntityGuid: Method for limiting permission to specific rows (passing a default GUID of all zeroes enables access to all rows)
> Identity: The username to grant/restrict access for
> IdentityProvider: When using Windows Authentication, the domain name
> Model: Always `Credential` in this case.

Additionally, permissions can be added through the API (this will add a permission to row Id 3):
> POST /api/v2/credentials/3/permissions