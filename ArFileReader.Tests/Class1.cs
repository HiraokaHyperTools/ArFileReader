using NUnit.Framework;

namespace ArFileReader.Tests
{
    public class Class1
    {
        [Test]
        [TestCase(@"cygwin-x64/libinvokezlibversion.a")]
        [TestCase(@"cygwin-x64/libinvokezlibversion.dll.a")]
        [TestCase(@"mingw-x86/libinvokezlibversion.a")]
        [TestCase(@"mingw-x86/libinvokezlibversion.dll.a")]
        public void ParseTest(string libFile)
        {
            var libFileBytes = File.ReadAllBytes(ResolvePath(libFile));

            ArFileParser.Parse(libFileBytes)
                .ToList()
                .ForEach(
                    it =>
                    {
                        Console.WriteLine($"{it.FileName} {it.FileSize}");
                    }
                );
        }

        [Test]
        [TestCase(@"cygwin-x64/libinvokezlibversion.a")]
        [TestCase(@"cygwin-x64/libinvokezlibversion.dll.a")]
        [TestCase(@"mingw-x86/libinvokezlibversion.a")]
        [TestCase(@"mingw-x86/libinvokezlibversion.dll.a")]
        public void ReparseAsGnuTest(string libFile)
        {
            var libFileBytes = File.ReadAllBytes(ResolvePath(libFile));

            ArFileParser.ReparseAsGnu(libFileBytes, ArFileParser.Parse(libFileBytes))
                .ToList()
                .ForEach(
                    it =>
                    {
                        Console.WriteLine($"{it.FileName} {it.FileSize}");
                    }
                );
        }

        private static string ResolvePath(string path) => Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "..",
            "..",
            "..",
            "Samples",
            path
        );
    }
}