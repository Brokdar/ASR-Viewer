using System;
using Xunit;

namespace Reader.Test
{
    public class ReadingFile
    {
        private readonly AsrReader _reader = new AsrReader();

        [Fact]
        public void ItExists()
        {
            Assert.NotNull(_reader);
        }

        [Fact]
        public void GivenDirectoryPath_ThenThrowsArgumentException()
        {
            const string path = "Home/Files";

            Assert.Throws<ArgumentException>(() => _reader.Read(path));
        }

        [Fact]
        public void GivenInvalidExtension_ThenThrowsArgumentException()
        {
            const string path = "Home/Files/data.txt";

            Assert.Throws<ArgumentException>(() => _reader.Read(path));
        }

        [Theory]
        [InlineData(@"P:\ASR_Viewer\Source\Sandbox\Test1.arxml", "Test1.arxml")]
        [InlineData(@"P:\ASR_Viewer\Source\Sandbox\Test2.arxml", "Test2.arxml")]
        public void GivenCorrectPath_ThenFileInformationAvailable(string path, string filename)
        {
            var doc = _reader.Read(path);

            Assert.NotNull(doc);
            Assert.Equal(filename, doc.Info.Name);
            Assert.Equal(path, doc.Info.Path);
            Assert.NotEmpty(doc.Packages);
        }
    }
}