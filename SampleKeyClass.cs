namespace SimpleBlazorSecurity.Client
{
    public class SampleKeyClass : ILocalKeyClass
    {
        string ILocalKeyClass.KeyName => "authToken";
    }
}