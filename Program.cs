using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

public class ESParse : System.Windows.Forms.Form
{
    private FolderBrowserDialog folderBrowserDialog1;

    private RichTextBox richTextBox1;
    private ComboBox comboBox1;
    private TextBox textBox1, textBox2;

    private MainMenu mainMenu1;
    private MenuItem fileMenuItem;
    private MenuItem folderMenuItem;

    private string folderName,key,data;
    private string[] root, dataFiles;

    private List<string> dataLines = new List<string>();
    private List<string> temp = new List<string>();
    private Dictionary<string, List<string>> objects = new Dictionary<string, List<string>>();
    private Hashtable dataTypes = new Hashtable();

    // The main entry point for the application.
    [STAThreadAttribute]
    static void Main()
    {
        Application.Run(new ESParse());
    }
    // Constructor.
    public ESParse()
    {
        this.mainMenu1 = new System.Windows.Forms.MainMenu();
        this.fileMenuItem = new System.Windows.Forms.MenuItem();
        this.folderMenuItem = new System.Windows.Forms.MenuItem();

        this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

        this.richTextBox1 = new System.Windows.Forms.RichTextBox();
        this.comboBox1 = new System.Windows.Forms.ComboBox();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.textBox2 = new System.Windows.Forms.TextBox();

        this.mainMenu1.MenuItems.Add(this.fileMenuItem);
        this.fileMenuItem.MenuItems.AddRange(
                            new System.Windows.Forms.MenuItem[] {this.folderMenuItem});
        this.fileMenuItem.Text = "File";

        this.folderMenuItem.Text = "Select Game Directory...";
        this.folderMenuItem.Click += new System.EventHandler(this.folderMenuItem_Click);

        // Set the help text description for the FolderBrowserDialog.
        this.folderBrowserDialog1.Description =
            "Select the directory that you want to use as the default.";

        // Do not allow the user to create new files via the FolderBrowserDialog.
        this.folderBrowserDialog1.ShowNewFolderButton = false;

        // Default to the AppData/Local folder.
        this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.LocalApplicationData;

        this.textBox1.Location = new System.Drawing.Point(8, 8);
        this.textBox1.Size = new System.Drawing.Size(790, 8);
        this.textBox1.ReadOnly = true;
        this.textBox1.Text = "Please Select a Game Path via the File Dropdown";
        this.Controls.Add(this.textBox1);

        this.comboBox1.Location = new System.Drawing.Point(8, 58);
        this.Controls.Add(this.comboBox1);

        this.richTextBox1.AcceptsTab = true;
        this.richTextBox1.Location = new System.Drawing.Point(8, 108);
        this.richTextBox1.Size = new System.Drawing.Size(790, 740);
        this.richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left |
                                   AnchorStyles.Bottom | AnchorStyles.Right;

        this.ClientSize = new System.Drawing.Size(800, 800);
        this.Controls.Add(this.richTextBox1);


        this.Menu = this.mainMenu1;
        this.Text = "ESParse";
    }

    // Bring up a dialog to chose a folder path in which to open or save a file.
    private void folderMenuItem_Click(object sender, System.EventArgs e)
    {
        // Show the FolderBrowserDialog.
        DialogResult result = folderBrowserDialog1.ShowDialog();
        if (result == DialogResult.OK)
        {
            //save the path of the selected directory
           folderName = folderBrowserDialog1.SelectedPath;
            //get all folders in selected directory
            root = Directory.GetDirectories(folderName);
            textBox1.Text = folderName;
            foreach(string file in root)
            {
                if (Path.GetFileName(file) == "data")
                {
                    //Get all files from data
                    dataFiles = Directory.GetFiles(file,"*.txt",SearchOption.AllDirectories);
                } else
                {
                    
                }
            }
            foreach(string dataFile in dataFiles)
            {
                //iterate through files and add all text to list
                dataLines.AddRange(System.IO.File.ReadAllLines(dataFile));
            }
            //remove empty elements
            dataLines.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            //remove comments, lines starting with #
            dataLines.RemoveAll(dataLine => dataLine.Substring(0,1) == "#");

            for (int i = dataLines.Count; i == 0; i--)
            {
                //If there is data in the first "column" of the text line
                if (!String.IsNullOrEmpty(dataLines[i]) && Char.IsLetter(dataLines[i][0]))
                {
                    key = Regex.Replace(dataLines[i].Split()[0], @"[^0-9a-zA-Z\ ]+", "");
                    data = Regex.Replace(dataLines[i].Split()[1], @"[^0-9a-zA-Z\ ]+", "");
                    //then add that data to a new hash set which we use to populate the dropdown in the form
                    objects.Add(data, temp);
                    //finally add both the data and unique ID to a new dictionary list
                    dataTypes.Add(key, data);
                    temp.Clear();
                }
                else
                {
                    temp.Add(dataLines[i]);

                }
            }
            foreach (string dataID in dataTypes)
            {
                //populate combobox with hash set
                comboBox1.Items.Add(dataID);
            }
        }
    }
}