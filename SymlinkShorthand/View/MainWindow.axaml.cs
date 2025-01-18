using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SymlinkShorthand.Controller;
using SymlinkShorthand.Model;

namespace SymlinkShorthand
{
    public partial class MainWindow : Window
    {
        private readonly SymlinkController _controller;
        public static string? argsTarget;

        public MainWindow()
        {
            InitializeComponent();

            SymlinkModel model = new SymlinkModel();
            _controller = new SymlinkController(model);

            if (argsTarget != null)
            {
                if(File.Exists(argsTarget) || Directory.Exists(argsTarget))
                {
                    if (File.Exists(argsTarget))
                        TargetIsFile.IsChecked = true;
                    else if (Directory.Exists(argsTarget))
                        TargetIsFile.IsChecked = false;

                    TargetIsFile.IsEnabled = false;
                    TargetIsDir.IsEnabled = false;

                    _controller.SetTargetPath(argsTarget);
                }
                else
                {
                    StatusUpdate("Wrong arguments");
                }
            }
        }

        private async void PickTargetPath_Clicked(object sender, RoutedEventArgs args)
        {
            TargetPath.Text = await SelectPath();
        }

        private async void PickDestPath_Clicked(object sender, RoutedEventArgs args)
        {
            TargetDestPath.Text = await SelectPath(true);
        }

        private void FillDestPath_Clicked(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(TargetPath.Text))
            {
                StatusUpdate("You should enter target path to use it");
                return;
            }

            if (TargetPath.Text.EndsWith('/') || TargetPath.Text.EndsWith('\\'))
            {
                TargetPath.Text = TargetPath.Text.Remove(TargetPath.Text.Length - 1);
            }

            int slash = TargetPath.Text.LastIndexOf('/');
            int backSlash = TargetPath.Text.LastIndexOf('\\');
            int finalIndex = Math.Max(slash, backSlash);

            try
            {
                TargetDestPath.Text = TargetPath.Text.Remove(finalIndex, TargetPath.Text.Length - finalIndex);
            }
            catch (Exception e)
            {
                StatusUpdate(e.Message);
            }
        }

        private void LinkTargets_Clicked(object sender, RoutedEventArgs args)
        {
            _controller.SetTargetPath(TargetPath.Text);
            _controller.SetDestinationPath(TargetDestPath.Text);
            _controller.SetDestinationName(TargetDestName.Text);
            Status.Text = _controller.CreateSymlink();

            TargetIsDir.IsEnabled = true;
            TargetIsFile.IsEnabled = true;
        }

        private void ClearAll_Clicked(object sender, RoutedEventArgs args)
        {
            _controller.ClearFields();
            ClearFields();
        }

        private async Task<string> SelectPath(bool dir = false)
        {
            try
            {
                TopLevel? topLevel = TopLevel.GetTopLevel(this);
                IReadOnlyList<IStorageItem> items;

                if (TargetIsDir.IsChecked.Value || dir)
                {
                    items = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "Select target directory",
                        AllowMultiple = false
                    });
                }
                else if (TargetIsFile.IsChecked.Value)
                {

                    items = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Select target file",
                        AllowMultiple = false
                    });
                }
                else
                {
                    StatusUpdate("You should select Symbolic Link type");
                    return "";
                }
                return Uri.UnescapeDataString(items[0].Path.AbsolutePath);
            }
            catch (Exception e)
            {
                StatusUpdate(e.Message);
                return "";
            }
        }

        private void ClearFields()
        {
            TargetPath.Clear();
            TargetDestPath.Clear();
            TargetDestName.Clear();
            TargetIsDir.IsEnabled = true;
            TargetIsFile.IsEnabled = true;
        }

        private void StatusUpdate(string message)
        {
            Status.Text = message;
        }
    }
}