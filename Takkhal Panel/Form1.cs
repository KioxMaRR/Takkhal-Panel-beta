using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using Microsoft.Win32;
using System.Security.Cryptography;
using Newtonsoft.Json.Schema;
using iTextSharp.tool.xml.html;
using Takkhal_Panel.Networking;

namespace Takkhal_Panel
{
    public partial class Form1 : Form
    {
        private TcpClient persistentClient = null;
        private NetworkStream persistentStream = null;
        private readonly object streamLock = new object();
        private const ulong SteamBase = 76561197960265728;
        private static readonly string MyVersion = "1.1.0";
        public Form1()
        {
            InitializeComponent();
            customizeDesing();
            Logger.Initialize(richTextBoxLog);
        }

        private void customizeDesing()
        {
            // Your customization code here
        }

        private void hideSubMenu()
        {
            // Your code here
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }

        private void btnMedia_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                activeForm.Close();
                activeForm = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openChildForm(new Form2());
            hideSubMenu();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void btnPlaylist_Click(object sender, EventArgs e)
        {
        }

        private void button9_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void btnEqualizer_Click(object sender, EventArgs e)
        {
     
            hideSubMenu();
        }

        private void btnTools_Click(object sender, EventArgs e)
        {
        }

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void btnPhotoAccount_Click(object sender, EventArgs e)
        {
    
    
        }

        private void lblactive_Click(object sender, EventArgs e)
        {
        }

        private void panelSideMenu_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnwifi_Click(object sender, EventArgs e)
        {
        }

        private void lblonline_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void btndown_Click(object sender, EventArgs e)
        {
            openChildForm(new Form4());
        }

        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            openChildForm(new Form5());
        }

        private async Task ConnectToServerWithModsFolderAsync(string modsFromServer, string gamePath, string username)
        {
            Logger.Log("Starting game...");

            string exePath = Path.Combine(gamePath, "DayZ_x64.exe");
            if (!File.Exists(exePath))
            {
                string msg = "DayZ_x64.exe not found at: " + exePath;
                Logger.Log(msg);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string modsFolder = Path.Combine(gamePath, "mods");
            if (!Directory.Exists(modsFolder))
            {
                string msg = "Mods folder not found at: " + modsFolder;
                Logger.Log(msg);
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string modsArgument = "";

            if (string.IsNullOrWhiteSpace(modsFromServer))
            {
                Logger.Log("No mods specified by server. Using local mods folder.");

                var mods = Directory.GetDirectories(modsFolder)
                    .Where(dir => Path.GetFileName(dir).StartsWith("@"))
                    .Select(dir => $"mods\\{Path.GetFileName(dir)}")
                    .ToList();

                if (!mods.Any())
                {
                    string msg = "No valid mods found in local mods folder.";
                    Logger.Log(msg);
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                modsArgument = $"\"-mod={string.Join(";", mods)}\"";
            }
            else
            {
                Logger.Log("Server provided mod list: " + modsFromServer);

                var modNames = modsFromServer.Split(';').Select(m => m.Trim()).ToList();
                List<string> missingMods = new List<string>();

                foreach (var mod in modNames)
                {
                    string modPath = Path.Combine(modsFolder, mod);
                    if (!Directory.Exists(modPath))
                    {
                        missingMods.Add(mod);
                        Logger.Log($"Missing mod: {mod}");
                    }
                }

                if (missingMods.Any())
                {
                    string missingList = string.Join(", ", missingMods);
                    string msg = "Missing mods: " + missingList;
                    Logger.Log(msg);
                    MessageBox.Show("The following mods are missing:\n" + string.Join("\n", missingMods),
                        "Missing Mods Update requred ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                var modPaths = modNames.Select(name => $"mods\\{name}");
                modsArgument = $"\"-mod={string.Join(";", modPaths)}\"";
            }

            string arguments = $"{modsArgument} -connect=5.42.223.48 -port=2302 -name={username}";


            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TakkhalPanel", "log.txt");
            try
            {
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {exePath} {arguments}{Environment.NewLine}");
            }
            catch { }

            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                WorkingDirectory = gamePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                
            };

            try
            {
                Process.Start(psi);

                if (button2.InvokeRequired)
                {
                    button2.Invoke((Action)(() =>
                    {
                        button2.Text = "Joining";
                        button2.Enabled = false;
                    }));
                }
                else
                {
                    button2.Text = "Joining";
                    button2.Enabled = false;
                }

                await Task.Delay(15000); 

                string processName = "DayZ_x64";
                Process[] processes = Process.GetProcessesByName(processName);

                if (button2.InvokeRequired)
                {
                    button2.Invoke((Action)(() =>
                    {
                        button2.Text = processes.Length > 0 ? "Running" : "Join";
                        button2.Enabled = true;
                    }));
                }
                else
                {
                    button2.Text = processes.Length > 0 ? "Running" : "Join";
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Launch failed: {ex.Message}{Environment.NewLine}");
                }
                catch { }

                MessageBox.Show("Error launching game:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static string GetHWID()
        {
            try
            {
                RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

                if (rk != null)
                {
                    object guid = rk.GetValue("MachineGuid");
                    if (guid != null)
                    {
                        string rawGuid = guid.ToString();

                        using (SHA256 sha = SHA256.Create())
                        {
                            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(rawGuid));
                            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
                        }
                    }
                }

                return "UNKNOWN";
            }
            catch
            {
                return "ERROR";
            }
        }




        private async Task SendSteamInfoPersistentAsync()
        {
            if (!await NetworkManager.ConnectPesistentClientAsync())
            {
                Logger.Log("Failed to connect persistent client.");
                return;
            }

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gamePathFile = Path.Combine(appData, "TakkhalPanel", "configTK.txt");

            if (!File.Exists(gamePathFile))
            {
                Logger.Log("configTK.txt not found");
                return;
            }

            string gamePath = File.ReadAllText(gamePathFile).Trim();

            string steamId = SteamSettingsHandler.LoadOrCreateSteamId(gamePath);
            if (!string.IsNullOrEmpty(steamId))
            {
                string boardSerial = GetHWID();
                string username = File.ReadAllText(Path.Combine(appData, "TakkhalPanel", "usernameTK.txt")).Trim();

                string payload = $"{steamId}|{boardSerial}|{username}";

                Logger.Log("Sending steam info...");
                string response = await NetworkManager.SendReceiveAsync(payload, expectedPrefix: "OK:");

                if (response == "OK: Access granted")
                {
                    Logger.Log("Authorized. Waiting before requesting mods list...");
                    await Task.Delay(100); 

                    Logger.Log("Requesting mods list...");
                    string modsResponse = await NetworkManager.SendReceiveAsync("GET_MODS", expectedPrefix: "MODS:");
                    string modsList = "";
                    if (!string.IsNullOrEmpty(modsResponse) && modsResponse.StartsWith("MODS:"))
                        modsList = modsResponse.Substring("MODS:".Length).Trim();

                    await ConnectToServerWithModsFolderAsync(modsList, gamePath, username);
                }
                else
                {
                    Logger.Log("Not authorized");
                    MessageBox.Show(response);
                }
            }
            else
            {
                Logger.Log("Invalid Steamid !" + steamId);
            }
        }


        private async void button2_Click_1(object sender, EventArgs e)
        {
            await SendSteamInfoPersistentAsync();
        }

        public static string GetMotherboardSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj["SerialNumber"].ToString().Trim();
                    }
                }
            }
            catch { }
            return "UNKNOWN";
        }


        private async void Form1_Load_1(object sender, EventArgs e)
        {
            try
            {
                Logger.Log("Launcher Loaded");

                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string panelPath = Path.Combine(appDataPath, "TakkhalPanel");
                string usernameFile = Path.Combine(panelPath, "usernameTK.txt");

                if (File.Exists(usernameFile))
                {
                    string username = File.ReadAllText(usernameFile, Encoding.UTF8);
                    lblnoaccount.Text = username;
                }

                bool connected = await NetworkManager.ConnectPesistentClientAsync();
                if (!connected)
                {
                    Logger.Log("Failed to connect Persistent client.");
                    return;
                }

                int connectedCount = await NetworkManager.GetConnectedClientCountAsync();
                if (connectedCount == -1)
                {
                    Logger.Log("Connection failed. No response received");
                    return;
                }

                string versionResponse = await NetworkManager.SendReceiveAsync($"VERSION:{MyVersion}");

                if (versionResponse == "UPDATE_REQUIRED")
                {
                    Logger.Log("Update required");

                    var updater = new AppUpdater();
                    string updateLink = await updater.GetUpdateLinkAsync();

                    Logger.Log($"Please Update Last Version: {updateLink}");

                    DialogResult result = MessageBox.Show(
                        "نسخه جدید در دسترس است. آیا مایل به دانلود آن هستید؟",
                        "آپدیت ضروری",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.OK)
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = updateLink,
                                UseShellExecute = true 
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("خطا در باز کردن لینک: " + ex.Message);
                        }
                    }

                    Application.Exit();
                    return;
                }
                else if (versionResponse == "VERSION_OK")
                {
                    Logger.Log("The Last Update");
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in Form1_Load_1: " + ex.Message);
                MessageBox.Show("Unexpected error: " + ex.Message);
                Application.Exit();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click_1(object sender, EventArgs e)
        {


           Environment.Exit(0);
            hideSubMenu();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
           
            ReturnRestart();


            string roaimn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            string panelPath = Path.Combine(roaimn, ("TakkhalPanel"));
            string usernameFile = Path.Combine(panelPath, ("usernameTK.txt"));
            if (File.Exists(usernameFile))
            {
                string username = File.ReadAllText(usernameFile, Encoding.UTF8);
                lblnoaccount.Text = username;
            }

            Task.Run(async () =>
            {
                int count = await NetworkManager.GetConnectedClientCountAsync();

                if (count != -1)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        label2.Text = $"{count}/40";
                    }));
                }
            });

            string processName = "DayZ_x64";
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                button2.Text = "Running";
            }
            else
            {
                button2.Text = "Join";
                button2.Enabled = true;
            }
        }

       
        public async void ReturnRestart()
        {
            string RestartTime = await NetworkManager.SendReceiveAsync("GET_RESTART_TIME");

            if (!string.IsNullOrEmpty(RestartTime) && RestartTime.StartsWith("RestartTime:"))
            {
           
                string timeOnly = RestartTime.Substring("RestartTime:".Length).Trim();

                lblRestartTime.Text = timeOnly;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (persistentStream != null)
                persistentStream.Close();
            if (persistentClient != null)
                persistentClient.Close();
        }


    

       

        private void button3_Click_1(object sender, EventArgs e)
        {
            openChildForm(new Form6());
        }
    }
}
