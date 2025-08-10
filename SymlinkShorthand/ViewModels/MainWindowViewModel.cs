using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using SymlinkShorthand.Views;

namespace SymlinkShorthand.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public static string? ArgsTarget;
        public static TopLevel? TopLevel;
        
        [ObservableProperty]
        private bool _targetIsFile;
        [ObservableProperty]
        private bool _targetIsFileEnabled = true;
        [ObservableProperty]
        private bool _targetIsFileChecked = false;

        [ObservableProperty]
        private bool _targetIsDir;
        [ObservableProperty]
        private bool _targetIsDirEnabled = true;
        [ObservableProperty]
        private bool _targetIsDirChecked = false;

        [ObservableProperty]
        private string _targetPath = string.Empty;

        [ObservableProperty]
        private string _targetDestPath = string.Empty;

        [ObservableProperty]
        private string _targetDestName = string.Empty;

        [ObservableProperty]
        private string _status = string.Empty;

        public MainWindowViewModel()
        {
            if (ArgsTarget != null)
            {
                TargetPath = ArgsTarget;
                if (File.Exists(ArgsTarget))
                {
                    TargetIsFileChecked = true;
                    TargetIsFileEnabled = false;
                    TargetIsDirEnabled = false;
                }
                else if (Directory.Exists(ArgsTarget))
                {
                    TargetIsDirChecked = true;
                    TargetIsDirEnabled = false;
                    TargetIsFileEnabled = false;
                }
                else
                {
                    Status = "Wrong arguments";
                }
            }
        }


        public async Task PickTargetPath()
        {
            try
            {
                if (TargetIsFileChecked)
                {
                    IReadOnlyList<IStorageFile> files = await TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Select a file",
                        AllowMultiple = false
                    });

                    if (files.Count >= 1)
                    {
                        TargetPath = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    }
                    TargetIsDirEnabled = false;
                    TargetIsFileEnabled = false;
                }
                else if (TargetIsDirChecked)
                {
                    IReadOnlyList<IStorageFolder> files = await TopLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "Select target directory",
                        AllowMultiple = false
                    });
                    TargetPath = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    TargetIsDirEnabled = false;
                    TargetIsFileEnabled = false;
                }
                else
                {
                    Status = "You should select Symbolic Link type";
                }
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        }

        public async Task PickDestPath()
        {
            try
            {
                if (TargetIsDirChecked || TargetIsFileChecked)
                {
                    IReadOnlyList<IStorageFolder> files = await TopLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "Select target directory",
                        AllowMultiple = false
                    });
                    TargetDestPath = Uri.UnescapeDataString(files[0].Path.AbsolutePath);
                    TargetIsDirEnabled = false;
                    TargetIsFileEnabled = false;
                }
                else
                {
                    Status = "You should select Symbolic Link type";
                }
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        }

        public void FillDestPath()
        {
            if (TargetPath == "")
            {
                Status = "You should enter target path to use this";
                return;
            }

            if (TargetPath.EndsWith('/') || TargetPath.EndsWith('\\'))
            {
                TargetPath = TargetPath.Remove(TargetPath.Length - 1);
            }

            int slash = TargetPath.LastIndexOf('/');
            int backSlash = TargetPath.LastIndexOf('\\');
            int finalIndex = Math.Max(slash, backSlash);

            try
            {
                TargetDestPath = TargetPath.Remove(finalIndex, TargetPath.Length - finalIndex);
            }
            catch (Exception e)
            {
                Status = e.Message;
            }
        }

        public void LinkTargets()
        {
            try
            {
                if (!(TargetIsDirChecked || TargetIsFileChecked))
                {
                    Status = "Symbolic link type required";
                    return;
                }

                if (TargetPath == "" && TargetDestName == "")
                {
                    Status = "The destination name is required";
                }

                if (!TargetDestPath.EndsWith("/") && !TargetDestPath.EndsWith('\\'))
                    {
                    TargetDestPath += "/";
                }

                if (Directory.Exists(TargetPath))
                {
                    Directory.CreateSymbolicLink(TargetDestPath + TargetDestName, TargetPath);
                    Status = "Symlink successfully created";
                }
                else if (File.Exists(TargetPath))
                {
                    File.CreateSymbolicLink(TargetDestPath + TargetDestName, TargetPath);
                    Status = "Symlink successfully created";
                }
                else
                {
                    Status = "Target path doesn't exist";
                    return;
                }
                TargetIsDirEnabled = true;
                TargetIsFileEnabled = true;
            }
            catch (Exception ex)
            {
                Status = ex.Message;
            }
        }

        public void ClearAll()
        {
            TargetPath = "";
            TargetDestPath = "";
            TargetDestName = "";
            TargetIsDir = true;
            TargetIsFile = true;
            TargetIsDirEnabled = true;
            TargetIsFileEnabled = true;
            Status = "";
        }
    }
}
