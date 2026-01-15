using Microsoft.AspNetCore.Mvc.Testing;

namespace Common.Web.Test
{
    /// <summary>
    /// Provides a base class for functional tests targeting web applications, managing the test application's lifecycle
    /// and HTTP client setup.
    /// </summary>
    /// <remarks>This class is intended to be inherited by test classes that require a configured test server
    /// and HTTP client for integration or end-to-end testing of web applications. The test lifecycle methods ensure
    /// that resources are properly initialized and disposed for each test run.</remarks>
    /// <typeparam name="T">The type of the test web application factory used to create and configure the test server and HTTP client.</typeparam>
    /// <typeparam name="U">The type representing the web application's startup or entry point class.</typeparam>
    [TestClass]
    public abstract class WebApplicationFunctionalTests<T, U> where T : TestWebApplicationFactory<U> where U : class
    {
        protected T _applicationFactory;
        protected HttpClient _client;

        [TestInitialize]
        public void BuildClient()
        {
            _applicationFactory = BuildApplicationFactory();
            _client = _applicationFactory.CreateClient();
        }

        /// <summary>
        /// Creates and returns an instance of the application factory of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/> representing the application factory.</returns>
        protected abstract T BuildApplicationFactory();

        [TestCleanup]
        public void TestCleanup()
        {
            _client?.Dispose();
            _applicationFactory?.Dispose();
        }
    }
}
