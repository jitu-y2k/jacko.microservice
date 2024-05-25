using System;
using System.IO;
using System.Reflection;

namespace Jacko.Services.ShoppingCartAPI.Tests
{

    public static class DirectoryHelper
    {
        public static string GetCurrentTestDirectory()
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var callingMethod = stackTrace.GetFrame(1).GetMethod();
            var testClassType = callingMethod.ReflectedType;
            var assembly = testClassType.Assembly;

            var codeBaseUri = new Uri(assembly.CodeBase);
            var codeFolderPath = Path.GetDirectoryName(codeBaseUri.LocalPath);

            return codeFolderPath;
        }
    }
}

