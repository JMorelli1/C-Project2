using System;
using System.IO;
using System.Windows.Forms;

public class FileSelectorForm : Form
{
    private ComboBox fileDropdown;

    public FileSelectorForm()
    {
        this.Text = "File Selector";
        this.Width = 400;
        this.Height = 150;

        InitializeFileDropdown();
    }

    private void InitializeFileDropdown()
    {
        Console.WriteLine("HI");
        fileDropdown = new ComboBox
        {
            Left = 20,
            Top = 20,
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

MessageBox.Show("HI");
        string dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        if (Directory.Exists(dataFolder))
        {
            MessageBox.Show("HI");
            Console.WriteLine("HELLO");
            string[] files = Directory.GetFiles(dataFolder);

            foreach (var file in files)
            {
                fileDropdown.Items.Add(Path.GetFileName(file)); // just filename, not full path
            }

            if (fileDropdown.Items.Count > 0)
                fileDropdown.SelectedIndex = 0;
        }
        else
        {
            MessageBox.Show($"Directory '{dataFolder}' does not exist.");
        }

        this.Controls.Add(fileDropdown);
    }

    public string GetSelectedFilePath()
    {
        if (fileDropdown.SelectedItem == null)
            return string.Empty;

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileDropdown.SelectedItem.ToString());
    }
}
