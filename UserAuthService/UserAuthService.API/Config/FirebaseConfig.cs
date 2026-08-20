using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace UserAuthService.API.Config;

public static class FirebaseConfig
{
    public static IServiceCollection AddFirebaseConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var relativePath = configuration["Firebase:CredentialFilePath"] ?? "Firebase/firebase-service-account.json";
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);

        if (File.Exists(fullPath) && FirebaseApp.DefaultInstance == null)
        {
#pragma warning disable CS0618
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(fullPath)
            });
#pragma warning restore CS0618
        }

        return services;
    }
}
