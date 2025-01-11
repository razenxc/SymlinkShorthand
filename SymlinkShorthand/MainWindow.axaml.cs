using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SymlinkShorthand
{
    public partial class MainWindow : Window
    {
        public static string? argsTarget;
        public MainWindow()
        {
            InitializeComponent();

            if (argsTarget != null)
            {
                xamlTargetPath.Text = argsTarget;
                if (File.Exists(argsTarget))
                {
                    xamlTargetIsFile.IsChecked = true;
                    xamlTargetIsDir.IsEnabled = false;
                    xamlTargetIsFile.IsEnabled = false;
                }
                else if(Directory.Exists(argsTarget))
                {
                    xamlTargetIsDir.IsChecked = true;
                    xamlTargetIsDir.IsEnabled = false;
                    xamlTargetIsFile.IsEnabled = false;
                }
                else
                {
                    StatusUpdate("Wrong arguments");
                }
            }
        }

        private async void xamlPickTargetPath_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                TopLevel? topLevel = TopLevel.GetTopLevel(this);

                if (xamlTargetIsFile.IsChecked.Value)
                {
                    IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Select a file",
                        AllowMultiple = false
                    });

                    if (files.Count >= 1)
                    {
                        xamlTargetPath.Text = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    }
                    xamlTargetIsDir.IsEnabled = false;
                    xamlTargetIsFile.IsEnabled = false;
                }
                else if (xamlTargetIsDir.IsChecked.Value)
                {
                    IReadOnlyList<IStorageFolder> files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title="Select target directory",
                        AllowMultiple = false
                    });
                    xamlTargetPath.Text = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    xamlTargetIsDir.IsEnabled = false;
                    xamlTargetIsFile.IsEnabled = false;
                }
                else
                {
                    StatusUpdate("You should select Symbolic Link type");
                }
                
            } catch (Exception e)
            {
                StatusUpdate(e.Message);
            }
        }

        private async void xamlPickDestPath_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (xamlTargetIsDir.IsChecked.Value || xamlTargetIsFile.IsChecked.Value)
                {
                    TopLevel? topLevel = TopLevel.GetTopLevel(this);

                    IReadOnlyList<IStorageFolder> files = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "Select target directory",
                        AllowMultiple = false
                    });
                    xamlTargetDestPath.Text = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    xamlTargetIsDir.IsEnabled = false;
                    xamlTargetIsFile.IsEnabled = false;
                }
                else 
                {
                    StatusUpdate("You should select Symbolic Link type");
                }
            }
            catch (Exception e)
            {
                StatusUpdate(e.Message);
            }
        }

        private void xamlFillDestPath_Clicked(object sender, RoutedEventArgs args)
        {
            if (xamlTargetPath.Text == "")
            {
                StatusUpdate("You should enter target path to use this");
                return;
            }

            if (xamlTargetPath.Text.EndsWith('/') || xamlTargetPath.Text.EndsWith('\\'))
            {
                xamlTargetPath.Text = xamlTargetPath.Text.Remove(xamlTargetPath.Text.Length - 1);
            }

            int slash = xamlTargetPath.Text.LastIndexOf('/');
            int backSlash = xamlTargetPath.Text.LastIndexOf('\\');
            int finalIndex = Math.Max(slash, backSlash);
            
            try
            {
                xamlTargetDestPath.Text = xamlTargetPath.Text.Remove(finalIndex, xamlTargetPath.Text.Length - finalIndex);
            } 
            catch (Exception e)
            {
                StatusUpdate(e.Message);
            }
        }

        private void xamlLinkTargets_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (!(xamlTargetIsDir.IsChecked.Value || xamlTargetIsFile.IsChecked.Value))
                {
                    StatusUpdate("Symbolic link type required");
                    return;
                }

                if (xamlTargetPath.Text == "" && xamlTargetDestName.Text == "")
                {
                    StatusUpdate("The destination name is required");
                    return;
                }

                if (!xamlTargetDestPath.Text.EndsWith('/') && !xamlTargetDestPath.Text.EndsWith('\\'))
                {
                    xamlTargetDestPath.Text += '/';
                }

                if (Directory.Exists(xamlTargetPath.Text))
                {
                    Directory.CreateSymbolicLink(xamlTargetDestPath.Text + xamlTargetDestName.Text, xamlTargetPath.Text);
                    StatusUpdate("Symlink successfully created");
                }
                else if (File.Exists(xamlTargetPath.Text))
                {
                    File.CreateSymbolicLink(xamlTargetDestPath.Text + xamlTargetDestName.Text, xamlTargetPath.Text);
                    StatusUpdate("Symlink successfully created");
                }
                else
                {
                    StatusUpdate("Target path does not exist");
                    return;
                }
                xamlTargetIsDir.IsEnabled = true;
                xamlTargetIsFile.IsEnabled = true;
            }
            catch (Exception e)
            {
                StatusUpdate(e.Message);
            }
        }

        private void xamlClearAll_Clicked(object sender, RoutedEventArgs args)
        {
            xamlTargetPath.Clear();
            xamlTargetDestPath.Clear();
            xamlTargetDestName.Clear();
            xamlTargetIsDir.IsEnabled = true;
            xamlTargetIsFile.IsEnabled = true;
        }

        private void StatusUpdate(string message)
        {
            xamlStatus.Text = message;
        }
    }
}