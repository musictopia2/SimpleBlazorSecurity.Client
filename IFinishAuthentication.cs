using System.Threading.Tasks;
namespace SimpleBlazorSecurity.Client
{
    public interface IFinishAuthentication
    {
        /// <summary>
        /// this will run after you finished authenticated.   may update something as a result.
        /// </summary>
        /// <returns></returns>
        Task AfterAuthenticationAsync();
    }
}
