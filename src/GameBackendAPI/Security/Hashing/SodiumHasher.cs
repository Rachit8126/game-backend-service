namespace GameBackendAPI.Security.Hashing
{
    public class SodiumHasher : ICustomPasswordHasher
    {
        private readonly string _dummyHash;

        private const int _operationLimit = 3;
        private const int _memoryLimit = 19922944;

        public SodiumHasher()
        {
            _dummyHash = Sodium.PasswordHash.ArgonHashString("HelloWorld", _operationLimit, _memoryLimit);
        }

        public bool Verify(string input, string? hash)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            if (String.IsNullOrEmpty(hash))
            {
                Sodium.PasswordHash.ArgonHashStringVerify(_dummyHash, input);
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
