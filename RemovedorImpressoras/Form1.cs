using System;
using System.Management;
using System.Windows.Forms;

namespace PrinterManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPrinters();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool check = chkSelectAll.Checked;
            for (int i = 0; i < clbPrinters.Items.Count; i++)
                clbPrinters.SetItemChecked(i, check);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPrinters();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (clbPrinters.CheckedItems.Count == 0)
            {
                MessageBox.Show("Nenhuma impressora selecionada.");
                return;
            }

            if (MessageBox.Show(
                $"Remover {clbPrinters.CheckedItems.Count} impressora(s)?",
                "Confirmar",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            string[] toRemove = new string[clbPrinters.CheckedItems.Count];
            clbPrinters.CheckedItems.CopyTo(toRemove, 0);

            foreach (string printerName in toRemove)
            {
                try
                {
                    if (!DeletePrinterByName(printerName))
                        MessageBox.Show("Falha ao remover: " + printerName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao remover '{printerName}': {ex.Message}");
                }
            }

            LoadPrinters();
        }

        private void LoadPrinters()
        {
            clbPrinters.Items.Clear();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer"))
                {
                    foreach (ManagementObject printer in searcher.Get())
                    {
                        string name = printer["Name"] != null ? printer["Name"].ToString() : "<sem nome>";
                        clbPrinters.Items.Add(name, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao listar impressoras: " + ex.Message);
            }
        }

        private bool DeletePrinterByName(string printerName)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_Printer WHERE Name = '" + EscapeWql(printerName) + "'"))
                {
                    foreach (ManagementObject printer in searcher.Get())
                    {
                        printer.Delete();
                        return true;
                    }
                }

                // fallback
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "rundll32.exe";
                p.StartInfo.Arguments = "printui.dll,PrintUIEntry /dl /n \"" + printerName + "\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit(5000);
                return p.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private string EscapeWql(string s)
        {
            return s == null ? "" : s.Replace("'", "''");
        }
    }
}
