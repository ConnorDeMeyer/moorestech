using System;
using System.IO;

namespace Test.TestConfig
{
    public class TestConfigPath
    {
        public string GetPath(string fileName)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory); 
            DirectoryInfo diParent = di.Parent.Parent.Parent.Parent;
            return Path.Combine(diParent.FullName, "Test","TestConfig","Json",fileName);
        }
    }
}