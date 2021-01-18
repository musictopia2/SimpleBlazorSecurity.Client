using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
namespace SimpleBlazorSecurity.Client
{
    public static class Extensions
    {
        private static void RegisterBasics(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, SimpleAuthenticationProvider>();
            services.AddScoped<ILocalKeyClass, SampleKeyClass>();
        }

        private static void RegisterDefaultKey(this IServiceCollection services)
        {
            services.AddScoped<ILocalKeyClass, SampleKeyClass>();
        }

        public static void RegisterCustomSecurity(this IServiceCollection services)
        {
            services.RegisterBasics();
            services.RegisterDefaultKey();
        }
        public static void RegisterCustomSecurity<F>(this IServiceCollection services)
            where F: class, IFinishAuthentication
        {
            services.RegisterBasics();
            services.RegisterDefaultKey();
            services.AddScoped<IFinishAuthentication, F>();
        }
        public static void RegisterCustomSecurity<K, F>(this IServiceCollection services)
            where F: class, IFinishAuthentication
            where K: class, ILocalKeyClass
        {
            services.RegisterBasics();
            services.AddScoped<IFinishAuthentication, F>();
            services.AddScoped<ILocalKeyClass, K>();
        }

    }
}
