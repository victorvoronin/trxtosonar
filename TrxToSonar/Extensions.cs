using System;
using System.IO;
using System.Linq;
using TrxToSonar.Model.Sonar;
using TrxToSonar.Model.Trx;
using File = TrxToSonar.Model.Sonar.File;

namespace TrxToSonar
{
    public static class Extensions
    {
        public static UnitTest GetUnitTest(this UnitTestResult unitTestResult, TrxDocument trxDocument)
        {
            return trxDocument.TestDefinitions.FirstOrDefault(x => x.Id == unitTestResult.TestId);
        }

        public static string GetTestClass(this UnitTest unitTest)
        {
            var parts = unitTest.Name.Split(".");
            
            return parts.Length > 1 ? parts[parts.Length - 1] : string.Empty;
        }
        
        public static File GetFile(this SonarDocument sonarDocument, string testFile)
        {
            return sonarDocument.Files.FirstOrDefault(x => x.Path == testFile);
        }

        public static string GetTestFile(this UnitTest unitTest, string solutionDirectory, bool useAbsolutePath)
        {
            var fullClassName = unitTest?.TestMethod?.ClassName;

            if (string.IsNullOrEmpty(fullClassName)) throw new NullReferenceException("Class name was not provided");

            var className = fullClassName.Split(".").Last();

            var testProjectSignature = Path.Combine(".Tests", "bin");
            var indexOfSignature = unitTest.TestMethod.CodeBase.IndexOf(testProjectSignature);
            var projectDirectory = unitTest.TestMethod.CodeBase.Substring(0, indexOfSignature + 6);

            var files = Directory.GetFiles(projectDirectory, $"{className}.cs", SearchOption.AllDirectories);

            if (!files.Any()) throw new FileNotFoundException($"Cannot find file with class {className}. Check that file has the same name as the class.");
            var result = files.First();
            

            if (!useAbsolutePath)
            {                
                result = result.Substring(solutionDirectory.Length + 1);
            }

            return result;
        }
    }
}