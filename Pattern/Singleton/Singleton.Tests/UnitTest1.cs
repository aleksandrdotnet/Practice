using NUnit.Framework;

namespace Singleton.Tests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsLazySingletonTest()
        {
            var a = LazySingleton.Instance;
            var b = LazySingleton.Instance;

            Assert.NotNull(a);
            Assert.NotNull(b);
            Assert.That(a, Is.SameAs(b));
        }
    }
}