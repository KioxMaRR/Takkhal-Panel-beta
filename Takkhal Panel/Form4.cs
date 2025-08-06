using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace Takkhal_Panel
{
    public partial class Form4 : Form
    {
        WebClient client;

        public Form4()
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        private void progressBar1_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            string downloadUrl = "https://dl2.soft98.ir/soft/r/Rufus.4.7.2231.0.zip?1748879094";
            string extractPath = @"C:\Users\Parham\Downloads\HJSplit.3.0";

            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;

            client.DownloadDataCompleted += (s, ev) =>
            {
                button1.Enabled = true;

                if (ev.Error != null)
                {
                    MessageBox.Show("Download error: " + ev.Error.Message);
                    return;
                }

                try
                {
                    byte[] zipBytes = ev.Result;

                    using (MemoryStream zipStream = new MemoryStream(zipBytes))
                    using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullPath = Path.Combine(extractPath, entry.FullName);

                            if (string.IsNullOrEmpty(entry.Name))
                            {
                                Directory.CreateDirectory(fullPath);
                                continue;
                            }

                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                            using (var entryStream = entry.Open())
                            using (var outputFileStream = File.Create(fullPath))
                            {
                                entryStream.CopyTo(outputFileStream);
                            }
                        }
                    }

                    label1.Text = "Download and extract completed.";
                    MessageBox.Show("Success", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Extract error: " + ex.Message);
                }
            };

            try
            {
                button1.Enabled = false;
                label1.Text = "Downloading...";
                client.DownloadDataAsync(new Uri(downloadUrl));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = $"Downloading: {e.ProgressPercentage}%";
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}