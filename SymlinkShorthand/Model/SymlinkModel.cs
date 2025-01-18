using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymlinkShorthand.Model
{
    internal class SymlinkModel
    {
        public string? TargetPath { get; set; }
        public string? DestinationPath { get; set; }
        public string? DestinationName { get; set; }
        public string CreateSymlink()
        {
            if(string.IsNullOrEmpty(DestinationPath) || string.IsNullOrEmpty(DestinationPath) || string.IsNullOrEmpty(DestinationName))
            {
                return "All fields should be filled";
            }

            if (!(DestinationPath.EndsWith('/') || DestinationPath.EndsWith('\\')))
            {
                DestinationPath += '/';
            }

            string fullDestinationPath = DestinationPath + DestinationName;

            try
            {
                if (Directory.Exists(TargetPath))
                {
                    Directory.CreateSymbolicLink(fullDestinationPath, TargetPath);
                }
                else if (File.Exists(TargetPath))
                {
                    File.CreateSymbolicLink(fullDestinationPath, TargetPath);
                }

                return "Symlink successfully create";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
