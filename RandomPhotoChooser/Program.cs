using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RandomPhotoChooser
{
    class Program
    {
        enum EntryType
        {
            None,
            File,
            Directory
        }
        static Random m_Random = null;
        static int Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                    return ShowUsage();
                string dir = args[0];
                if (!Directory.Exists(dir))
                    return ShowUsage("Directory {0} doesn't exists.", dir);
                string outputFile = args[2];
                string outputFileExt = Path.GetExtension(outputFile).ToLower();
                if (!outputFileExt.Equals(".jpg"))
                    return ShowUsage("Output file format must be jpg.");

                string extensions = args[1].ToLower().Replace("*", "");
                m_Random = new Random(DateTime.Now.Millisecond);
                string file = DrawFile(dir, extensions.Split('|').ToList());
                if (string.IsNullOrEmpty(file))
                    return ShowUsage("Could't find file with extension {0} in {1}.", extensions, dir);
                string fileExt = Path.GetExtension(file).ToLower();
                if (outputFileExt.Equals(fileExt))
                {
                    Console.WriteLine("Copying file {0} to {1} ...", file, outputFile);
                    File.Copy(file, outputFile, true);
                }
                else
                {
                    Console.WriteLine("Converting file {0} to {1} ...", file, outputFile);
                    Image2JPG.Convert(file, outputFile);
                }
                Console.WriteLine("Done.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}.", ex.Message);
                return 1;
            }
            finally
            {
#if DEBUG
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
#endif
            }
        }

        private static string DrawFile(string directory, List<string> extensions)
        {
            List<string> entries = Directory.GetFileSystemEntries(directory, "*.*", SearchOption.TopDirectoryOnly).ToList();
            while (entries.Count > 0)
            {
                int idx = m_Random.Next(0, entries.Count);
                string entry = entries[idx];
                EntryType type = GetType(entry);

                if (type == EntryType.File)
                {
                    if (extensions.Contains((new FileInfo(entry)).Extension.ToLower()))
                        return entry;
                }
                else if (type == EntryType.Directory)
                {
                    entry = DrawFile(entry, extensions);
                    if (!string.IsNullOrEmpty(entry))
                        return entry;
                }
                entries.RemoveAt(idx);
            }
            return string.Empty;
        }
        static EntryType GetType(string path)
        {
            if (Directory.Exists(path))
                return EntryType.Directory;
            if (File.Exists(path))
                return EntryType.File;
            return EntryType.None;
        }
        static int ShowUsage()
        {
            return ShowUsage(null, null);
        }
        static int ShowUsage(string errorMessage, params string[] args)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(string.Format(errorMessage, args));
                Console.WriteLine(Environment.NewLine);
            }
            Console.WriteLine("Usage: {0}.exe <photos_directory> <extensions_filter> <destination_file>", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            Console.WriteLine("Options:");
            Console.WriteLine("\t<photos_directory>  - path to directory with photos (C:\\Photos)");
            Console.WriteLine("\t<extensions_filter> - file extension filter (.jpg|.png)");
            Console.WriteLine("\t<destination_file>  - path to destintion file (D:\\Photos\\file.jpg)");
            return 1;
        }
    }
}
