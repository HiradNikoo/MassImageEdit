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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MassImageEdit
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void tbQuality_Scroll(object sender, EventArgs e)
        {
            lblQuality.Text = $"{tbQuality.Value}%";
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                fbd.SelectedPath = txtOutput.Text;
            }

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtOutput.Text = fbd.SelectedPath;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutput.Text))
            {
                txtOutput.Text = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        private string getExportOutput(string directory) => Path.Combine(directory, "Exported");

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lstImages.Items.AddRange(ofd.FileNames);
                if (lstImages.Items.Count > 0)
                {
                    lstImages.SelectedIndex = 0;
                    var path = Path.GetDirectoryName(lstImages.SelectedItem.ToString());
                    txtOutput.Text = getExportOutput(path);
                    btnRemove.Enabled = true;
                }
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            var index = lstImages.SelectedIndex;
            if (index <= 0) return;

            var path = lstImages.SelectedItem.ToString();
            lstImages.Items.RemoveAt(index);
            lstImages.SelectedIndex = index - 1;
            lstImages.Items.Insert(index - 1, path);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            var index = lstImages.SelectedIndex;

            if (index < 0 || index >= lstImages.Items.Count - 1) return;

            var path = lstImages.SelectedItem.ToString();
            lstImages.Items.RemoveAt(index);
            lstImages.SelectedIndex = index;
            lstImages.Items.Insert(index + 1, path);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            progressbar.Maximum = lstImages.Items.Count;
            progressbar.Value = 0;
            progressbar.Visible = true;

            var quality = tbQuality.Value;
            var output = txtOutput.Text;
            var width = 2000;
            var dpi = 96;

            int.TryParse(txtWidth.Text, out width);
            int.TryParse(txtDPI.Text, out dpi);

            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            var counter = 0;

            foreach (string imagePath in lstImages.Items)
            {
                progressbar.Value += 1;
                counter++;

                var fileName = Path.GetFileName(imagePath);
                var targetFileName = $"{fileName}_optimized_{counter}.jpg";


                ImageUtils.SaveImage(imagePath, Path.Combine(output, targetFileName), width, quality, dpi);
            }
            progressbar.Visible = false;
        }

        private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstImages.SelectedItem == null)
            {

                pbShow.Image = null;

            }
            else
            {

                pbShow.Image = new Bitmap(lstImages.SelectedItem.ToString());
            }
        }

        private void chbImageAutoSize_CheckedChanged(object sender, EventArgs e)
        {
            if (chbImageAutoSize.Checked)
            {
                pnlImage.AutoScroll = true;
                pbShow.SizeMode = PictureBoxSizeMode.AutoSize;
                pbShow.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            }
            else
            {
                pnlImage.AutoScroll = false;
                pbShow.SizeMode = PictureBoxSizeMode.Zoom;
                pbShow.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var index = lstImages.SelectedIndex;
            if (index < 0) return;

            var paths = lstImages.Items.Cast<string>().ToList();
           
            paths.RemoveAt(index);
            index--;
            if (index < 0 && paths.Count > 0) index++;

            lstImages.Items.Clear();
            lstImages.Items.AddRange(paths.ToArray());
            lstImages.SelectedIndex = index;

            if (lstImages.SelectedIndex < 0)
            {
                btnRemove.Enabled = false;
                pbShow.Image = null;
            }
        }

        private void lstImages_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var lastFileDirectory = "";
                foreach (string file in files)
                {
                    lstImages.Items.Add(file);
                    lastFileDirectory = System.IO.Path.GetDirectoryName(file);
                }
                txtOutput.Text = getExportOutput(lastFileDirectory);
            }
        }

        private void lstImages_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
