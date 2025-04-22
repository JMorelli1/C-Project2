using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zen.Barcode;
using System.Drawing.Imaging;
using System.IO;

namespace CpuSchedulingWinForms
{
    public partial class CpuScheduler : Form
    {
        public CpuScheduler()
        {
            InitializeComponent();
            LoadFilesIntoDropdown();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            //dashBoardTab.Show();
            this.tabSelection.SelectTab(0);
            this.sidePanel.Height = btnDashBoard.Height;
            this.sidePanel.Top = btnDashBoard.Top;

            //this.btnProductCode.BackColor = Color.Transparent;
            //this.btnCpuScheduler.BackColor = Color.Transparent;
            //this.btnDashBoard.BackColor = Color.DimGray;
        }

        private void btnCpuScheduler_Click(object sender, EventArgs e)
        {
            //this.dashBoardTab.Show();
            this.tabSelection.SelectTab(1);
            this.sidePanel.Height = btnCpuScheduler.Height;
            this.sidePanel.Top = btnCpuScheduler.Top;

            //this.btnProductCode.BackColor = Color.Transparent;         
            //this.btnDashBoard.BackColor = Color.Transparent;
            //this.btnCpuScheduler.BackColor = Color.DimGray;
        }

        private void processAlgorithmRun (object sender, EventArgs e){
            if(sender is not Button){
                throw new Exception("Unknown selection made for Algorithm run.");
            }

            Control clickedButton = (Button)sender; 
            List<ProcessControlBlock> pcbs = null;
            string selectedFile = fileSelector.SelectedItem?.ToString();
            Console.WriteLine($"FILE {selectedFile}");

            if(!string.IsNullOrEmpty(selectedFile)){
                pcbs = FileReader.LoadProcessesFromCsv($"data/{selectedFile}");
                Algorithms.runAlgorithm(pcbs, (string) clickedButton.Tag);
                updateProcessState(pcbs.Count);
            }
            else if (txtProcess.Text != "")
            {
                int np = Convert.ToInt16(txtProcess.Text);
                var optionsForm = new AlgorithmOptionsForm(clickedButton.Tag, np);
                optionsForm.ShowDialog();
                updateProcessState(short.Parse(txtProcess.Text));
            }
            else
            {
                MessageBox.Show("Enter number of processes or select a stored file", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }

        private void updateProcessState(int numberOfProcess) {
                if (numberOfProcess <= 10)
                {
                    progressBar1.Increment(4); //cpu progress bar
                    progressBar1.SetState(1);
                    progressBar2.Increment(13);
                    progressBar2.SetState(1);
                }
                else if (numberOfProcess > 10)
                {
                    progressBar1.Increment(15);
                    progressBar1.SetState(1);
                    progressBar2.Increment(38); //memory progress bar
                    progressBar2.SetState(3);
                }
                listView1.Clear();
                listView1.View = View.Details;
                
                listView1.Columns.Add("Process ID", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Quantum Time", 100, HorizontalAlignment.Center);

                for (int i = 0; i < numberOfProcess; i++)
                {
                    var item = new ListViewItem();
                    item.Text = "Process " + (i + 1);
                    item.SubItems.Add("-");
                    listView1.Items.Add(item);
                }
                listView1.Items.Add("\n");
                listView1.Items.Add("CPU handles: " + numberOfProcess);
        }

        private void txtProcess_TextChanged(object sender, EventArgs e)
        {

        }

        private void restartApp_Click(object sender, EventArgs e)
        {
            this.Hide();
            CpuScheduler cpuScheduler = new CpuScheduler();
            cpuScheduler.ShowDialog();
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            if (txtCodeInput.Text != "")
            {
                string barcode = txtCodeInput.Text;
                //Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                //pictureBoxCodeOutput.Image = barcode.Draw(barcodeInput, 30);
                //pictureBoxCodeOutput.Height = barcode.Draw(txtCodeInput.Text, 150).Height;
                //pictureBoxCodeOutput.Width = barcode.Draw(txtCodeInput.Text, 150).Width;

                Bitmap bitmap = new Bitmap(barcode.Length * 36, 109);   //40, 150
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Font font = new Font("IDAutomationHC39M Free Version", 25);
                    PointF point = new PointF(2f, 2f);
                    SolidBrush black = new SolidBrush(Color.Black);
                    SolidBrush white = new SolidBrush(Color.White);
                    graphics.FillRectangle(white, 0, 0, bitmap.Width, bitmap.Height);
                    graphics.DrawString(barcode, font, black, point);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    pictureBoxCodeOutput.Image = bitmap;
                    //pictureBoxCodeOutput.Height = bitmap.Height;
                    //pictureBoxCodeOutput.Width = bitmap.Width;
                }
            }
            else
            {
                MessageBox.Show("No Input", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodeInput.Focus();
            }
            
        }

        private void btnQrcode_Click(object sender, EventArgs e)
        {
            if (txtCodeInput.Text != "")
            {
                CodeQrBarcodeDraw codeQr = BarcodeDrawFactory.CodeQr;
                pictureBoxCodeOutput.Image = codeQr.Draw(txtCodeInput.Text, 100);
                //string barcode = txtCodeInput.Text;
                //Bitmap bitmap = new Bitmap(barcode.Length * 40, 150);
                //pictureBoxCodeOutput.Image = bitmap; 
            }
            else
            {
                MessageBox.Show("No Input", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodeInput.Focus();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pictureBoxCodeOutput.Image == null)
            {
                return;
            }
            else if (pictureBoxCodeOutput.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PNG|*.png|JPEG|*.jpeg|ICON|*.ico"})
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBoxCodeOutput.Image.Save(saveFileDialog.FileName);
                    }
                }
            }           
        }

        private void btnProductCode_Click(object sender, EventArgs e)
        {
            //this.dashBoardTab.Show();
            this.tabSelection.SelectTab(2);
            this.sidePanel.Height = btnProductCode.Height;
            this.sidePanel.Top = btnProductCode.Top;
            
            //this.btnCpuScheduler.BackColor = Color.Transparent;
            //this.btnDashBoard.BackColor = Color.Transparent;
            //this.btnProductCode.BackColor = Color.DimGray;
        }

        private void CpuScheduler_Load(object sender, EventArgs e)
        {
            this.sidePanel.Height = btnDashBoard.Height;
            this.sidePanel.Top = btnDashBoard.Top;
            this.progressBar1.Increment(5);
            this.progressBar2.Increment(17);
            listView1.View = View.Details;
            listView1.GridLines = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Application.Exit();
            timer1.Start();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(this.Opacity > 0.0)
            {
                this.Opacity -= 0.021;
            } else
            {
                timer1.Stop();
                Application.Exit();
            }
        }

        private void txtCodeInput_Click(object sender, EventArgs e)
        {
            this.txtCodeInput.Clear();
        }

        private void LoadFilesIntoDropdown(){
            string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory[..73], "data");

            if (Directory.Exists(dataFolder))
            {
                var files = Directory.GetFiles(dataFolder);

                fileSelector.Items.Clear();
                fileSelector.Items.Add(""); // None for clearing input
                foreach (string file in files)
                {
                    fileSelector.Items.Add(Path.GetFileName(file));
                }

            }
            else
            {
                MessageBox.Show($"Folder not found: {dataFolder}");
            }
        }
    }
}