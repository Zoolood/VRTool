using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Combiner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Any(x => x.Contains("combine")))
            {
                string mainFile = "combined.ts";

                using (var fs = new FileStream(mainFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    foreach (string file in GetAllFiles(AppDomain.CurrentDomain.BaseDirectory, "*.ts", ".ts").ToList())
                    {
                        byte[] fileData = File.ReadAllBytes(file);

                        fs.Write(fileData, 0, fileData.Length);
                    }
                }
            }
            else
            {
                List<string> directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory).ToList();

                foreach (var dir in directories)
                {
                    if (Directory.GetFiles(dir).Count() == 0)
                        continue;

                    string mainFile = Path.GetFileName(Path.GetDirectoryName(dir + "\\")) + ".dat";

                    using (var fs = new FileStream(mainFile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        foreach (string file in GetAllFiles(dir))
                        {
                            byte[] fileData = File.ReadAllBytes(file);

                            fs.Write(fileData, 0, fileData.Length);
                        }
                    }
                }
            }
        }

        public static List<string> GetAllFiles(string dir, string extension = "*.dat", string extension2 = ".dat")
        {
            var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
            List<decimal> files = new List<decimal>();
            Directory.GetFiles(dir, extension).ToList().ForEach(x =>
            {
                if (!x.Contains("combin"))
                {
                    x = Path.GetFileNameWithoutExtension(x);
                    files.Add(decimal.Parse(x, numberFormatInfo));
                }
            });

            files = files.OrderBy(x => x).ToList();

            List<string> fileListLoop = new List<string>();

            files.ForEach(x =>
            {
                string fileName = x.ToString(new CultureInfo("en-US")) + extension2;
                fileListLoop.Add(Path.Combine(dir, fileName));
            });

            return fileListLoop;
        }
    }
}
