namespace GameBackendAPI.Security
{
    public interface ICustomPasswordHasher
    {
        public string GetHash(string input);
        public bool Verify(string input, string hash);
    }
}
