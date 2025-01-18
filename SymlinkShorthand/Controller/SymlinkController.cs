using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SymlinkShorthand.Model;

namespace SymlinkShorthand.Controller
{
    internal class SymlinkController
    {
        private readonly SymlinkModel _model;

        public SymlinkController(SymlinkModel model)
        {
            _model = model;
        }

        public void SetTargetPath(string path) => _model.TargetPath = path;
        public void SetDestinationPath(string path) => _model.DestinationPath = path;
        public void SetDestinationName(string name) => _model.DestinationName = name;
        public string CreateSymlink() => _model.CreateSymlink();

        public void ClearFields()
        {
            _model.TargetPath = null;
            _model.DestinationPath = null;
            _model.DestinationName = null;
        }
    }
}
