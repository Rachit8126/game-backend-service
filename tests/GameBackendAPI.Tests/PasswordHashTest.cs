using GameBackendAPI.Security.Hashing;

namespace GameBackendAPI.Tests
{
    public class PasswordHashTest
    {
        private SodiumHasher _sodiumHasher = new();

        [Fact]
        public void TestHashingDuplicacy()
        {
            string testPassword = "Hello@World";
            string hash1 = _sodiumHasher.GetHash(testPassword);
            string hash2 = _sodiumHasher.GetHash(testPassword);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void TestHashVerify()
        {
            string password = "Hello@World";
            string hash = _sodiumHasher.GetHash(password);

            Assert.True(_sodiumHasher.Verify(password, hash));
        }

        [Fact]
        public void TestWrongPassword()
        {
            string password = "Hello@World";
            string passwordToCheck = "Hello@World1";
            string hash = _sodiumHasher.GetHash(password);

            Assert.False(_sodiumHasher.Verify(passwordToCheck, hash));
        }

        [Fact]
        public void TestHashPrefix()
        {
            string password = "Hello@World";
            string hash = _sodiumHasher.GetHash(password);
            Assert.StartsWith("$argon2id$", hash);
        }

        [Fact]
        public void TestMalformedHash()
        {
            string password = "Hello@World";
            string hash = "PasswordHasher.Instance.HashPassword(password)";

            Assert.False(_sodiumHasher.Verify(password, hash));
        }

        [Fact]
        public void TestEmptyPasswordHash()
        {
            string password = "";
            Assert.Throws<ArgumentException>(() => _sodiumHasher.GetHash(password));
        }

        [Fact]
        public void TestNullPasswordHash()
        {
            string? password = null;
            Assert.Throws<ArgumentException>(() => _sodiumHasher.GetHash(password));
        }

        [Fact]
        public void TestNullHash()
        {
            string password = "HelloWorld";

            Assert.False(_sodiumHasher.Verify(password, null));
        }
    }
}
