using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Takkhal_Panel
{
    public partial class Form5 : Form
    {
        private string configPath;
        public Form5()
        {
            InitializeComponent();
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "TakkhalPanel");
            Directory.CreateDirectory(appFolder);
            configPath = Path.Combine(appFolder, "configTK.txt");

            LoadSavedPath();
        }
        private void LoadSavedPath()
        {
            if (File.Exists(configPath))
            {
                string savedPath = File.ReadAllText(configPath);
                if (Directory.Exists(savedPath))
                    button3.Text = savedPath;
            }
        }
        private void Form5_Load(object sender, EventArgs e)
        {
            if (File.Exists(configPath))
            {
                string savedPath = File.ReadAllText(configPath);
                if (Directory.Exists(savedPath))
                {
                    button3.Text = savedPath;

                    string exePath = Path.Combine(savedPath, "DayZ_x64.exe");
                    if (File.Exists(exePath))
                    {
                        label2.Text = "OK";
                        label2.ForeColor = Color.Green;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the DayZ installation folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string exePath = Path.Combine(selectedPath, "DayZ_x64.exe");

                    if (File.Exists(exePath))
                    {
                        button3.Text = selectedPath;

                        try
                        {
                            File.WriteAllText(configPath, selectedPath);
                            label2.Text = "OK";
                            label2.ForeColor = Color.Green;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error saving path: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("DayZ_x64.exe was not found in the selected folder.", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
        
        
            string username = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appData, "TakkhalPanel");
            Directory.CreateDirectory(appFolder);

            string usernamePath = Path.Combine(appFolder, "usernameTK.txt");

            try
            {
                File.WriteAllText(usernamePath, username, new UTF8Encoding(true));
                MessageBox.Show("Username saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving username: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
    

