namespace GameBackendAPI.Security.Hashing
{
    public class SodiumHasher : ICustomPasswordHasher
    {
        private const int _operationLimit = 3;
        private const int _memoryLimit = 19922944;

        public bool Verify(string input, string hash)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Sodium.PasswordHash.ArgonHashStringVerify(hash, input);
        }

        public string GetHash(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be empty or null!", paramName: nameof(input));
            }

            return Sodium.PasswordHash.ArgonHashString(input, _operationLimit, _memoryLimit);
        }
    }
}
