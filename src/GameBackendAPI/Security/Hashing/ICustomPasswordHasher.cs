namespace GameBackendAPI.Security.Hashing
{
    public interface ICustomPasswordHasher
    {
        public string GetHash(string input);
        public bool Verify(string input, string? hash);
    }
}
