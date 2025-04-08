
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    /// <summary>ddasf
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<Tuple<string, string>> renamedFiles = new List<Tuple<string, string>>(); // Liste für UndoBtn
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UndoRenameFiles() // Methode UndoBtn Funktion
        {
            foreach (var renamedFile in renamedFiles)
            {
                string originalFilePath = renamedFile.Item1;
                string newFilePath = renamedFile.Item2;

                if (File.Exists(newFilePath))
                {
                    File.Move(newFilePath, originalFilePath);
                }
            }

            renamedFiles.Clear();
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e) //UndoBtn Methode Ausführen
        {
            UndoRenameFiles();
            MessageBox.Show("Umbenennung rückgängig gemacht");


        }

        static void allFiles(String path, String search, String suffix, String replace, String leadZeros)
        {
            files(path, search, suffix, replace, leadZeros);
            foreach (var subdirectorys in subDirects(path))
            {


                allFiles(subdirectorys.FullName, search, suffix, replace, leadZeros);
            }

        }

        static DirectoryInfo[] subDirects(String path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            return directoryInfo.GetDirectories();
        }

        static void files(String path, String search, String suffix, String replace, String leadZeros)
        {
            String sup = "";
            String sup2 = "";
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (var file in directory.GetFiles())
            {
                string fileName = file.Name;
                if (file.Name.Contains(search))
                {
                    fileName = fileName.Replace(search, replace);


                    string originalFilePath = file.FullName;
                    string newFilePath = System.IO.Path.Combine(directory.FullName, file.Name.Replace(search, replace));
                    sup = newFilePath;
                    renamedFiles.Add(new Tuple<string, string>(originalFilePath, newFilePath));
                    //File.Move(originalFilePath, newFilePath);
                }
                if (suffix != "")
                {
                    fileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + "." + suffix;
                }
                if (leadZeros != "")
                {
                    string fileNums = Regex.Match(fileName, @"\d+").Value;
                    if (fileNums != "")
                    {
                        fileName = fileName.Replace(fileNums, String.Format("{0:" + leadZeros + "0}", int.Parse(fileNums)));
                        sup2= System.IO.Path.Combine(directory.FullName, fileName);
                    }                  
                }
                File.Move(file.FullName, directory + "\\" + fileName);
                renamedFiles.Add(new Tuple<string, string>(sup, sup2));

            }
        }

        private void MainBtn_Click(object sender, RoutedEventArgs e)
        {
            if (directoryText.Text != "" && (searchVal.Text != "" || suffixVal.Text != "") && replaceVal.Text != "")
            {
                String dirText = directoryText.Text;
                String seaVal = searchVal.Text;
                String suffVal = suffixVal.Text;
                String replVal = replaceVal.Text;
                String leadZeros = leadingZeroes.Text;

                // alte Methode nur das jetzt auch in Unterordnern
                allFiles(dirText, seaVal,suffVal,replVal,leadZeros);
               
                MessageBox.Show("Dateien umbenannt");
            }
            else
            {
                MessageBox.Show("Textfelder nicht gefüllt");
            }
        }

        private void DirectoryBtn_Click(object sender, RoutedEventArgs e)
        {

            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();

            if (dlg.ShowDialog() ?? false)
            {
                directoryText.Text = dlg.SelectedPath;
            }
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        private void ConsoleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (directoryText.Text == "")
            {
                MessageBox.Show("Kein Ordner ausgewählt");
                return;
            }

            AllocConsole();
            string input = Console.ReadLine();

            if (input.StartsWith("count") && input.Contains(' '))
            {
                RenamerCount(input);
            }
        }

        private void RenamerCount(string input)
        {
            string orgFile = input.Split(' ')[1];
            string newFile = input.Split(' ')[2];

            DirectoryInfo directory = new DirectoryInfo(directoryText.Text);
            Console.WriteLine(Array.IndexOf(directory.GetFiles(), directoryText.Text + "\\" + orgFile));
            Console.WriteLine(directoryText.Text + "\\" + orgFile);
            Console.WriteLine(directory.GetFiles());
            for (int i = Array.IndexOf(directory.GetFiles(), directoryText.Text + "\\" + orgFile); i < directory.GetFiles().Length; i++)
            {
                Console.WriteLine(0);
            }
        }
        static int num = 1;
        public static void nameandcounter(String path, String praeffix, String newname)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (var file in directory.GetFiles())
            {
                String[] fileSplit = file.Name.Split('.');
                if (fileSplit[fileSplit.Length - 1].Equals(praeffix))
                {
                    
                   
                    string fileName = file.Name;
                    string search = "";
                    for (int i = 0; i < fileSplit.Length - 1; i++)
                    {
                        search = search + fileSplit[i];
                        if (fileSplit.Length - 2 - i > 0)
                        {
                            search = search + ".";
                        }
                    }

                   

                    fileName = fileName.Replace(search, newname);
                    string originalFilePath = file.FullName;
                    string newFilePath = System.IO.Path.Combine(directory.FullName, file.Name.Replace(file.Name, num+fileName));
                    renamedFiles.Add(new Tuple<string, string>(originalFilePath, newFilePath));
                    File.Move(file.FullName, directory + "\\" + num + fileName);
                    num++;

                }
            }

            foreach (var subdirectory in subDirects(path))
            {
                nameandcounter(subdirectory.FullName, praeffix, newname);
            }
        }

        
       
        
        
       
        
        private void datentyp_click(object sender, RoutedEventArgs e)
        {
            if (directoryText.Text != "" && suffixVal.Text != "" && replaceVal.Text != "")
            {
                nameandcounter(directoryText.Text, suffixVal.Text, replaceVal.Text);
               
            }

            else
            {
                MessageBox.Show("Textfelder nicht gefüllt");
            }
        }
        
    }
}
