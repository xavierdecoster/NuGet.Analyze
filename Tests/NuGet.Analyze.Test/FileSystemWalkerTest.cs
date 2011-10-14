using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Analyze.Folder;
using NuGet.Common;

namespace NuGet.Analyze.Test
{
    /// <summary>
    ///This is a test class for FileSystemWalkerTest and is intended
    ///to contain all FileSystemWalkerTest Unit Tests
    ///</summary>
    [TestClass]
    public class FileSystemWalkerTest
    {
        private static readonly IConsole console = new TestConsole();
        private static readonly IConsolePrinter consolePrinter = new ConsolePrinter(console);
        private static readonly IConfigInterpreter configInterpreter = new ConfigInterpreter();
        private static FileSystemWalker fileSystemWalker;

        private readonly string repository = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            fileSystemWalker = new FileSystemWalker(consolePrinter, configInterpreter, new FileSystemPathTranslator());
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for GetRepositoryWorkingDirectory
        ///</summary>
        [TestMethod]
        public void GetRepositoryWorkingDirectoryTest()
        {
            var workingDirectory = fileSystemWalker.GetRepositoryWorkingDirectory(repository);
            Assert.IsNotNull(workingDirectory);
            Assert.IsTrue(Directory.Exists(repository));
        }

        /// <summary>
        ///A test for GetSolutionsInDirectory
        ///</summary>
        [TestMethod]
        public void GetSolutionsInDirectoryTest()
        {
            var solutions = fileSystemWalker.GetSolutionsInDirectory(repository);
            Assert.IsNotNull(solutions);
            Assert.IsTrue(solutions.IsEmpty());
        }

        /// <summary>
        ///A test for AnalyzeRepository
        ///</summary>
        [TestMethod]
        public void AnalyzeRepository()
        {
            fileSystemWalker.AnalyzeRepository(repository, verbose: true);
            Assert.Inconclusive();
        }
    }
}
