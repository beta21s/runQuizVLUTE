using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace runQuickVLUTE
{
    public partial class frmMain : Form
    {
        string filename;
        public frmMain()
        {
            InitializeComponent();
   
            Show();
            this.WindowState = FormWindowState.Minimized;
            notifyIcon.Visible = true;

            // StartUp
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rk.SetValue(AppDomain.CurrentDomain.BaseDirectory, Application.ExecutablePath);
        }

        private void downloadPhanMem()
        {
            Uri uri = new Uri("http://113.176.83.98/vlute_packages/quiz/setup.exe");
            filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp/example.exe");
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                WebClient wc = new WebClient();
                wc.DownloadFileAsync(uri, filename);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (progressBar1.Value == progressBar1.Maximum)
            {
                progressBar1.Value = 0;
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Process.Start(filename);
                Close();
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Unable to download exe, please check your connection", "Download failed!");
            }
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            Timer_Check_Internet.Start();
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    showMess("Kết nối thành công với Internet", 0);
                return true;
            }
            catch
            {
                showMess("Kết nối Internet thất bại", 1);
                return false;
            }
        }

        public void showMess(string str, int err=0)
        {
            if(err == 1)
            {
                lblThongBao.ForeColor = Color.Red;
            }
            else
            {
                lblThongBao.ForeColor = Color.Green;
                lblThongBao.Text = str;
            }
        }

        private void Timer_Check_Internet_Tick(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {
                var cauHinh = FireBaseSetting.getData<FireBaseSetting>();
                if (cauHinh.start_date.Date <= DateTime.Now.Date && cauHinh.end_date.Date >= DateTime.Now.Date)
                {
                    downloadPhanMem();
                    Timer_Check_Internet.Stop();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
