using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Takkhal_Panel
{
    public class AppUpdater
    {
        private readonly string tempFolder;
        private readonly string updateZipPath;

        public AppUpdater()
        {
            tempFolder = Path.Combine(Path.GetTempPath(), "TakkhalPanelUpdate");
            updateZipPath = Path.Combine(tempFolder, "update.zip");
        }

      
        public string GetUpdateZipPath() => updateZipPath;

    
        public async Task<string> GetUpdateLinkAsync()
        {
            Logger.Log("Fetching update link from server...");
            string response = await Networking.NetworkManager.SendReceiveAsync("GET_Link_Launcher", expectedPrefix: "LINK:");
            if (!string.IsNullOrEmpty(response) && response.StartsWith("LINK:"))
            {
                string link = response.Substring("LINK:".Length).Trim();
                Logger.Log("Update link received: " + link);
                return link;
            }

            Logger.Log("Failed to get update link.");
            return null;
        }

     
        public async Task<bool> DownloadUpdateAsync(string url, IProgress<int> progress = null)
        {
            try
            {
                Logger.Log("Starting update file download...");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = File.Create(updateZipPath))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int read;
                        Logger.Log("Download progress: 0%");

                        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            if (canReportProgress)
                            {
                                int percent = (int)((totalRead * 100) / totalBytes);
                                Logger.UpdateLastLine($"Download progress: {percent}%");
                                progress?.Report(percent);
                            }
                        }
                    }
                }

                Logger.Log("Update download completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error downloading update: " + ex.Message);
                MessageBox.Show("Error downloading update: " + ex.Message);
                return false;
            }
        }
        public void LaunchUpdaterAndExit(string zipPath, string installPath, string baseDirectory)
        {
            string updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");

            if (!File.Exists(updaterPath))
            {
                Logger.Log("Updater executable not found at: " + updaterPath);
                MessageBox.Show("Updater executable not found.");
                return;
            }

            if (!File.Exists(zipPath))
            {
                Logger.Log("Zip file not found at: " + zipPath);
                MessageBox.Show("Zip file not found.");
                return;
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = updaterPath,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                Logger.Log("Launching updater...");
                Process.Start(psi);

                Logger.Log("Updater launched successfully.");
                Application.Exit();
            }
            catch (Exception ex)
            {
                Logger.Log("Error launching updater: " + ex.Message);
                MessageBox.Show("Error launching updater: " + ex.Message);
            }
        }


        private void CreateBatchFile()
        {
         
        }

        public void StartUpdaterAndExit()
        {
           
        }
    }
}
