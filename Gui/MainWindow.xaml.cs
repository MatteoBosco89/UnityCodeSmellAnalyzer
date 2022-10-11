﻿using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UnityCodeSmellAnalyzer : Window
    {
        protected Dictionary<string, string> parameters = new Dictionary<string, string>();
        protected string projectPath = "";

        public UnityCodeSmellAnalyzer()
        {
            InitializeComponent();
        }
        private void StartAnalyze(object sender, RoutedEventArgs e)
        {

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
            }

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
                        projectName.Text = name;
                    }
                }
            }
            catch (Exception) { projectName.Text = "Undefined"; }
        }

        protected void UpdateProjectName(object sender, RoutedEventArgs e)
        {
            parameters["name"] = projectName.Text;
        }

    }
}