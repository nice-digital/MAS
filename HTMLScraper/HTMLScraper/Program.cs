using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;

namespace HTMLScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = ConfigurationManager.AppSettings["readPath"];

            if (Directory.Exists(path))
            {
                string[] fileEntries = Directory.GetFiles(path);

                foreach (string filename in fileEntries)
                {
                    using (StreamWriter sw = File.AppendText(ConfigurationManager.AppSettings["logPath"]))
                        sw.WriteLine("Processing " + filename);

                    GetXMLTag(filename);
                }
            }
        }

        private static void GetXMLTag(string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);
            var elemList = doc.GetElementsByTagName("ukmicommenturi");
            var currentCommentLink = elemList[0].InnerXml;
            
            if (!string.IsNullOrEmpty(currentCommentLink))
                GetHTMLFile(currentCommentLink);
        }

        private static void GetHTMLFile(string currentCommentLink)
        {
            string htmlCode;
            using (WebClient client = new WebClient())
                htmlCode = client.DownloadString(currentCommentLink);

            var savePath = CreateSaveURL(currentCommentLink);

            var file = new FileInfo(savePath);
            file.Directory.Create();

            using (StreamWriter outputFile = new StreamWriter(savePath))
                outputFile.WriteLine(htmlCode);

            using (StreamWriter sw = File.AppendText(ConfigurationManager.AppSettings["logPath"]))
            {
                sw.WriteLine("HTML scraped");
            }
        }

        private static string CreateSaveURL(string currentCommentLink)
        {
            var baseurl = ConfigurationManager.AppSettings["writePath"];
            var filename = "index.html";
            currentCommentLink = currentCommentLink.Replace("https://www.medicinesresources.nhs.uk/en/Medicines-Awareness/", "");
            currentCommentLink = currentCommentLink.Replace("/", @"\");

            return baseurl + currentCommentLink + filename; ;
        }
    }
}
