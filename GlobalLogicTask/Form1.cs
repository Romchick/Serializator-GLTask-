using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalLogicTask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> listOfFilePath = new List<string>();
        List<string> listOfFiles = new List<string>();
        List<string> listOfFolders = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            // Open Folder Browser Dialog
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {

                string path = folderDlg.SelectedPath;
                // Selection of all subdirectories of selected folder
                var fold = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                string uStr = "";
                foreach (var u in fold)
                {
                    // Cut off path to selecned directory
                    uStr = u.Substring(path.Length);
                    //Add this path to collecton
                    listOfFolders.Add(uStr);

                }
                // Selection of all files in selected diectory and subdirectories
                var allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                string fStr = "";
                foreach (var f in allFiles)
                {
                    // The paths to files add to one collection
                    listOfFiles.Add(f);
                    fStr = f.Substring(path.Length);
                    //and paths without path to selected folder to other
                    listOfFilePath.Add(fStr);
                }

                // Sorting list. It is required for correct creation of direcrory structure.
                listOfFolders = listOfFolders.OrderBy(r => r).ToList();

                //All files are reading like array of bytes and this arrays are added to list
                List<byte[]> listOfBytes = new List<byte[]>();
                foreach (var myFile in listOfFiles)
                {
                    byte[] fileBytes = File.ReadAllBytes(myFile);
                    listOfBytes.Add(fileBytes);

                }

                // Binnary serialization. First list of folder structure is serialized, then list of bytes
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, listOfFolders);
                formatter.Serialize(stream, listOfFilePath);
                formatter.Serialize(stream, listOfBytes);
                stream.Close();
                Environment.SpecialFolder root = folderDlg.RootFolder;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //similarly Folder Browser Dialog is opening
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                string newPath = folderDlg.SelectedPath;
                Stream stream = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.None);

                //Deserealization 
                IFormatter formatter = new BinaryFormatter();
                List<string> outputListOfFolders = (List<string>)formatter.Deserialize(stream);
                List<string> outputListOfFilePath = (List<string>)formatter.Deserialize(stream);
                List<byte[]> outputListOfBytes = (List<byte[]>)formatter.Deserialize(stream);
                stream.Close();

                // Creating folder structure. 
                string outputPath = "";
                foreach (var m in outputListOfFolders)
                {
                    outputPath = newPath + m;
                    Directory.CreateDirectory(outputPath);
                }

                //Writing files from byte arrays. 
                int i = 0;
                foreach (var b in outputListOfBytes)
                {
                    File.WriteAllBytes(newPath + outputListOfFilePath.ElementAt(i), b);
                    i++;
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
