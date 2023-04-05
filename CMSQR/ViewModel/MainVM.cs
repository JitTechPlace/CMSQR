using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Haley.Abstractions;
using Haley.Enums;
using Haley.Models;
using Haley.MVVM;
using Haley.Utils;

namespace CMSQR.ViewModel
{
    public class MainVM : ChangeNotifier
    {
        //IDialogService _ds; 

        #region Properties

        private bool _isMonitoring;
        public bool IsMonotering
        {
            get { return _isMonitoring; }
            set { SetProp(ref _isMonitoring, value); }
        }

        private bool? _isDirPathValid;

        public bool? IsDirPathValid
        {
            get { return _isDirPathValid; }
            set { SetProp(ref _isDirPathValid , value); }
        }

        private bool? _isFileValid;

        public bool? IsFileValid
        {
            get { return _isFileValid; }
            set { SetProp(ref _isFileValid, value); }
        }

        private bool? _isQrValid;
        public bool? ISQRValid
        {
            get { return _isQrValid; }
            set { SetProp(ref _isQrValid, value); }
        }

        private string _selectedPosition;

        public string SelectedPosition
        {
            get { return _selectedPosition; }
            set {SetProp(ref  _selectedPosition, value); }
        }

        private string _baseDirectory;
        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set
            {
                SetProp(ref _baseDirectory, value);
                if (string.IsNullOrEmpty(value))
                {
                    IsDirPathValid = null;
                }
                else
                {
                    //Some value is Present
                    IsDirPathValid = Directory.Exists(value);
                }
            }
        }

        private string _fileSelected;
        public string FileSelected
        {
            get { return _fileSelected; }
            set 
            { 
               SetProp(ref _fileSelected, value);
                if (string.IsNullOrEmpty(value))
                {
                    IsFileValid = null;
                }
                else
                {
                    //Some value is Present
                    IsFileValid = File.Exists(value);
                }
            }
        }

        private string _qrfileSelected;
        public string QRFileSelected
        {
            get { return _qrfileSelected; }
            set 
            { 
                SetProp(ref _qrfileSelected, value);
                if (string.IsNullOrEmpty(value))
                {
                    ISQRValid = null;
                }
                else
                {
                    //Some valus is Present
                    ISQRValid = File.Exists(value);
                }
            }
        }




        private ObservableCollection<object> _selectedFileTypes;
        public ObservableCollection<object> SelectedFileTypes
        {
            get { return _selectedFileTypes; }
            set { SetProp(ref _selectedFileTypes, value); }
        }


        private ObservableCollection<object> _selectedDisplayFields;
        public ObservableCollection<object> SelectedDisplayFields
        {
            get { return _selectedDisplayFields; }
            set { SetProp(ref _selectedDisplayFields, value); }
        }

        #endregion


        #region Commands
        public ICommand BrowseFolderCommand => new DelegateCommand(_browseFolder);
        public ICommand BrowseFileCommand => new DelegateCommand(_browsFile);
        public ICommand BrowseQRFileCommand => new DelegateCommand(_browsQRFile);
        #endregion


        #region Command Methods

        void _browseFolder()
        {
            FolderBrowserDialog _fdialog = new FolderBrowserDialog()
            {
                Description = "Choose a folder to scan",
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar,
                //RootFolder = (Environment.SpecialFolder.MyDocuments),
                ShowNewFolderButton = true,
            };

            if (IsDirPathValid.HasValue && IsDirPathValid.Value)
            {
                _fdialog.SelectedPath = BaseDirectory;
            }

            if (_fdialog.ShowDialog() == DialogResult.OK)
            {
                BaseDirectory = _fdialog.SelectedPath;
            }

            //if (IsDirPathValid == null)
            //{
            //    IsDirPathValid = true;
            //}
            //{
            //    IsDirPathValid = !IsDirPathValid;
            //}
        }

        void _browsFile()
        {
            OpenFileDialog _fidialog = new OpenFileDialog()
            {
                Title = "Choose a File",
                Multiselect = true,
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),           
            };

            if (_fidialog.ShowDialog() == DialogResult.OK)
            {
                FileSelected = _fidialog.FileName;
            }

            //if (FileSelected.vsl)
            //{
            //    _fidialog.FileName = BaseDirectory;
            //}

            //if(FileSelected != null)
            //{
            //    _fidialog.CheckPathExists = FileSelected.Contains(string.Empty);
            //}
        }

        void _browsQRFile()
        {
            OpenFileDialog _QRfidialog = new OpenFileDialog()
            {
                Title = "Choose a File",
                Multiselect = true,
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            if (_QRfidialog.ShowDialog() == DialogResult.OK)
            {
                QRFileSelected = _QRfidialog.FileName;
            }

            //if (FileSelected.vsl)
            //{
            //    _fidialog.FileName = BaseDirectory;
            //}

            //if(FileSelected != null)
            //{
            //    _fidialog.CheckPathExists = FileSelected.Contains(string.Empty);
            //}
        }
        #endregion


        public static MainVM Singleton = new MainVM();
        public static MainVM getSingleton()
        {
            if (Singleton == null) Singleton = new MainVM();
            return Singleton;
        }

        public static void Clear()
        {
            Singleton = new MainVM();
        }
        private MainVM() 
        {
            IsDirPathValid = null;
            BaseDirectory = null;
            IsFileValid = null;
            FileSelected = null;
            ISQRValid = null;
            QRFileSelected = null;
        }

    }
}
