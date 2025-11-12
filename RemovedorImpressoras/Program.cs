using System;
using System.Management;
using System.Windows.Forms;

namespace PrinterManager
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }

    public class MainForm : Form
    {
        private CheckedListBox clbPrinters;
        private CheckBox chkSelectAll;
        private Button btnRefresh;
        private Button btnRemove;

        public MainForm()
        {
            Text = "Removedor de Impressoras";
            Width = 600;
            Height = 500;

            clbPrinters = new CheckedListBox();
            clbPrinters.Dock = DockStyle.Top;
            clbPrinters.Height = 350;

            chkSelectAll = new CheckBox();
            chkSelectAll.Text = "Selecionar / Desselecionar Tudo";
            chkSelectAll.Dock = DockStyle.Top;
            chkSelectAll.Height = 24;

            btnRefresh = new Button();
            btnRefresh.Text = "Atualizar";
            btnRefresh.Width = 120;

            btnRemove = new Button();
            btnRemove.Text = "Remover Selecionadas";
            btnRemove.Width = 180;

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Bottom;
            panel.Height = 40;
            panel.Controls.Add(btnRefresh);
            panel.Controls.Add(btnRemove);

            Controls.Add(clbPrinters);
            Controls.Add(chkSelectAll);
            Controls.Add(panel);

            Load += (s, e) => LoadPrinters();
            btnRefresh.Click += (s, e) => LoadPrinters();
            chkSelectAll.CheckedChanged += (s, e) =>
            {
                bool check = chkSelectAll.Checked;
                for (int i = 0; i < clbPrinters.Items.Count; i++)
                    clbPrinters.SetItemChecked(i, check);
            };
            btnRemove.Click += BtnRemove_Click;
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (clbPrinters.CheckedItems.Count == 0)
            {
                MessageBox.Show("Nenhuma impressora selecionada.");
                return;
            }

            if (MessageBox.Show("Remover as impressoras selecionadas?", "Confirmar", MessageBoxButtons.YesNo)
                != DialogResult.Yes) return;

            btnRemove.Enabled = false;
            try
            {
                string[] toRemove = new string[clbPrinters.CheckedItems.Count];
                clbPrinters.CheckedItems.CopyTo(toRemove, 0);

                foreach (string printerName in toRemove)
                {
                    try
                    {
                        bool ok = DeletePrinterByName(printerName);
                        if (!ok)
                            MessageBox.Show("Falha ao remover: " + printerName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao remover '" + printerName + "': " + ex.Message);
                    }
                }
            }
            finally
            {
                btnRemove.Enabled = true;
                LoadPrinters();
            }
        }

        private void LoadPrinters()
        {
            clbPrinters.Items.Clear();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                foreach (ManagementObject printer in searcher.Get())
                {
                    string name = printer["Name"] != null ? printer["Name"].ToString() : "<sem nome>";
                    clbPrinters.Items.Add(name, false);
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
                        try
                        {
                            printer.Delete(); 
                            return true;
                        }
                        catch (ManagementException mex)
                        {
                           
                            if (mex.Message.ToLower().Contains("not implemented"))
                                break;
                            else
                                throw;
                        }
                    }
                }

      
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
