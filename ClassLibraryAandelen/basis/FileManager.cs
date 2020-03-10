using Microsoft.Win32;
using System;
using System.IO;
using Markdig;

namespace ClassLibraryAandelen.basis
{
    public class FileManager
    {
        //variabele dat verwijst naar het absolute pad naar de solution
        public static String pathLocalSolution = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        /// <summary>
        /// Opent een savedialog en laat de gebruiker een folder kiezen en een naam laten geven aan de file.
        /// </summary>
        /// <returns>een absoluut pad naar file name zonder de extensie ernaar</returns>
        public static String PathToSaveFile()
        {
            SaveFileDialog saveFile = null;
            string path = "";
            try
            {
                saveFile = new SaveFileDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Filter= "*.md|*.html"
                };
                if(saveFile.ShowDialog() == true) path = Path.ChangeExtension(saveFile.FileName, null);
            }
            catch (Exception)
            {
                throw new Exception();
            }
            return path;
        }

        /// <summary>
        /// Maakt een file aan in het gegeven pad en zet alle Markdown inhoud erin.
        /// </summary>
        /// <param name="path">absoluut pad waarnaar de file wordt aangemaakt</param>
        /// <param name="content">inhoud dat zal gezet worden in de gemaakt file</param>
        public static void SaveFileMd(String path, String content)
        {
            using(StreamWriter file = File.CreateText(path + ".md"))
            {
                file.Write(content);
            }
        }

        /// <summary>
        /// Maakt een file aan in het gegeven pad. Met de inhoud dat wordt doorgegeven en css file in resources worden 
        /// deze geconverteert naar html met Markdig.
        /// </summary>
        /// <param name="path">absoluut pad waarnaar de file wordt aangemaakt</param>
        /// <param name="content">inhoud dat zal gezet worden in de gemaakt file</param>
        public static void SaveFileHtml(String path, String content)
        {
            using(StreamReader css = new StreamReader(pathLocalSolution + @"\Resources\basic.css"))
            using (StreamWriter file = File.CreateText(path + ".html"))
            {
                file.Write(Markdown.ToHtml($"<Style>{css.ReadToEnd()}</Style>\n" + content,
                    new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()));
            }
        }
    }
}
