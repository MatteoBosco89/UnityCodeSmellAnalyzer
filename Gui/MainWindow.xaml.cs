using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using GuiModel;

namespace Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UnityCodeSmellAnalyzer : Window
    {
        protected string projectPath = "";

        public UnityCodeSmellAnalyzer()
        {
            InitializeComponent();
        }

        private void StartAnalyze(object sender, RoutedEventArgs e)
        {
            Program.AddParam("projectName", ProjectName.Text.Trim());
            Program.AddParam("projectFolder", ProjectFolder.Text.Trim());
            Program.AddParam("logLevel", ((ComboBoxItem)VerbosityLevel.SelectedValue).Uid);
            Program.AddParam("directory", FolderUnderAnalysis());
            Program.Init(this);  
        }

        private string FolderUnderAnalysis()
        {
            string folder = "";
            if (WholeProject.IsChecked.HasValue && WholeProject.IsChecked.Value) folder = WholeProject.Name;
            if (OnlyAssets.IsChecked.HasValue && OnlyAssets.IsChecked.Value) folder = "Assets";
            if (SubFolder.IsChecked.HasValue && SubFolder.IsChecked.Value)
            {
                if (FolderToAnalyze.Text.Trim() == "") folder = WholeProject.Name;
                else folder = FolderToAnalyze.Text.Trim();
            }
            return folder;
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShowDirectoryDialog(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result.ToString().Equals("OK")) ProjectFolder.Text = folderDialog.SelectedPath;
            }
        }

        public void WriteOutput(string? s)
        {
            if (s == null) return;
            LogDump.AppendText(s);
            LogDump.AppendText("\n");
            LogDumpScroll.ScrollToEnd();
        }

        private void ProjectFolderPlaceholder(object sender, RoutedEventArgs e)
        {
            if (ProjectFolder.Text == "")
            {
                SetPlaceholder(ProjectFolder, @"Assets/folderTextBg.gif");
            }
            else
            {
                ProjectFolder.Background = null;
                projectPath = ProjectFolder.Text;
                LoadProjectName();
            }
        }


        private void SetPlaceholder(System.Windows.Controls.TextBox t, string path)
        {
            ImageBrush textImageBrush = new ImageBrush();
            textImageBrush.ImageSource =
                new BitmapImage(
                    new Uri(path, UriKind.Relative)
                );
            textImageBrush.AlignmentX = AlignmentX.Left;
            textImageBrush.Stretch = Stretch.Uniform;
            t.Background = textImageBrush;
            ProjectName.Text = "Undefined";
        }

        protected void LoadProjectName()
        {
            string f = "ProjectSettings/ProjectSettings.asset";
            string fullPath = Path.Combine(projectPath, f);
            try
            {
                foreach (string l in File.ReadAllLines(fullPath))
                {
                    if (l.IndexOf("productName") > 0)
                    {
                        string name = l.Split(':')[1];
                        if (name.Trim() == "") throw new Exception();
                        ProjectName.Text = name;
                    }
                }
            }
            catch (Exception) { ProjectName.Text = "Undefined"; }
        }


    }
}
