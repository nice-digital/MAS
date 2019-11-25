using Shouldly;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MAS.Tests.Infrastructure
{
    public static class Extensions
    {
        public static void ShouldMatchApproved(this string content,
                [System.Runtime.CompilerServices.CallerMemberName] string memberName = null,
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = null)
        {
         
            var expectedFileDir = Path.GetDirectoryName(sourceFilePath);
            var callingFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
            var expectedFileName = $"{callingFileNameWithoutExtension}.{memberName}.approved.txt";

            var expectedFilePath = Path.Combine(expectedFileDir, expectedFileName);

            if (File.Exists(expectedFilePath))
            {
                var expectedText = File.ReadAllText(expectedFilePath, Encoding.UTF8);

                var normalisedExpectedText = expectedText.Replace("\r", "");//normalising line endings.
                var normalisedContent = content.Replace("\r", "");

                if (normalisedExpectedText.Equals(normalisedContent))
                { //this is the happy path. everything matches, so just exit without throwing and the test will be green.
                    return;
                }
            }
            else
            {
                File.WriteAllText(expectedFilePath, "", Encoding.UTF8);
            }

            var actualFilePath = expectedFilePath.Replace(".approved.txt", ".actual.txt");
            File.WriteAllText(actualFilePath, content, Encoding.UTF8);

            var kDiffPath = "C:\\Program Files\\KDiff3\\kdiff3.exe";
            if (File.Exists(kDiffPath))
            {
                Process.Start(kDiffPath, string.Format("{0} {1} -m -o {0} --cs \"CreateBakFiles=0\"", expectedFilePath, actualFilePath));
            }
            else
            {
                var dir = Directory.GetCurrentDirectory();
                var vsDiffPath = Path.GetFullPath(Path.Combine(dir, "Difftools", "vsDiffMerge.exe"));
                Process.Start(vsDiffPath, string.Format("\"{0}\" \"{1}\" /t", expectedFilePath, actualFilePath));
            }

            throw new ShouldMatchApprovedException("String didn't match", actualFilePath, expectedFilePath);
        }
    }
}
