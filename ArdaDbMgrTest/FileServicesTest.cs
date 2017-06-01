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
            var fileSvcs = new FileServices();
            var fileList = fileSvcs.EnumerateFiles("sqlfiles").ToArray();

            int numberFilesFound = fileList.Count();

            Assert.Equal(1, numberFilesFound);
        }
    }
}
