using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldBoxModLoaderInstaller
{
    public class Uninstaller
    {
        [STAThreadAttribute]
        public static void Main()
        {
            var worldboxDirectory = ShowDialog("Select WorldBox directory");
            var managedDirectory = worldboxDirectory + "\\worldbox_Data\\Managed";
            var binDirectory = Path.GetFullPath("bin");
            Console.WriteLine(worldboxDirectory);
            Console.WriteLine(managedDirectory);
            Console.WriteLine(binDirectory);
            if (worldboxDirectory != default)
            {
                if (Directory.Exists(binDirectory))
                {
                    if (Directory.Exists(managedDirectory))
                    {
                        string[] bins = Directory.GetFiles(binDirectory);
                        foreach (string file in bins)
                        {
                            var fileName = Path.GetFileName(file);
                            Console.WriteLine(file);
                            var managedFile = managedDirectory + "\\" + fileName;
                            Console.WriteLine(managedFile);
                            if (fileName != "Assembly-CSharpTrue.dll")
                            {
                                if (File.Exists(managedFile))
                                    File.Delete(managedFile);
                                else
                                    continue;
                            }
                            else
                            {
                                var assemblyCsName = Path.GetDirectoryName(managedFile) + "\\Assembly-CSharp.dll";
                                File.Delete(assemblyCsName);
                                File.Copy(file, assemblyCsName, true);
                            }
                        }
                        Console.WriteLine("\nWorldBoxModLoader successfully uninstalled!");
                    }
                    else
                        Console.WriteLine("\nSelected directory is not WorldBox directory!");
                }
                else
                    Console.WriteLine("\nError occured during loading files from bin directory, make sure you haven't moved the program to another directory or deleted the bin folder.");
            }
            else
                Console.WriteLine("\nInstallation was cancelled.");

            Console.Write("\nPress any key to exit the program...");
            Console.ReadKey();
        }

        private static string ShowDialog(string description)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
                else
                    return default;
            }
        }
    }
}
