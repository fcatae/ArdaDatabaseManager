using System;
using System.Linq;
using Xunit;

using ArdaDbMgr.Services;

namespace ArdaDbMgrTest
{
    public class FileServicesTest
    {
        [Fact]
        public void FileEnumeration()
        {
            var fileSvcs = new FileServices("sqlfiles");
            var fileList = fileSvcs.EnumerateFiles().ToArray();

            int numberFilesFound = fileList.Count();

            Assert.Equal(2, numberFilesFound);
        }
    }
}
