using Duende.IdentityServer.Models;

namespace IdentityServer.WebApi
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
           new IdentityResource[]
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new[] { "role" })
           };

        public static IEnumerable<ApiScope> ApiScopes =>
           new ApiScope[]
           {
               new ApiScope("Cedepp.WebApi.Scope")

           };

        public static IEnumerable<ApiResource> ApiResources => new[] {
            new ApiResource("Cedepp.WebApi")
             {
                 Scopes=new List<string>{ "Cedepp.WebApi.Scope"},
                 ApiSecrets=new List<Secret>{new Secret("Cedepp.WebApi.Secret".Sha256())},
                 UserClaims = new List<string> { "role" }
             }
        };

        public static IEnumerable<Client> Clients =>
           new Client[]
           {
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = {new Secret("OnlyUserKnowsThisSecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:5173/signin-oidc" },
                    FrontChannelLogoutUri="https://localhost:5173/signout-oidc",
                    PostLogoutRedirectUris={ "https://localhost:5173/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {"openid", "profile", "Cedepp.WebApi.Scope"},
                    RequireConsent = true,
                    RequirePkce=true,
                    AllowPlainTextPkce=true,
                    AllowedCorsOrigins = { "https://localhost:5173" }
                }
           };
    }
}
