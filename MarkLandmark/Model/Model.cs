using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MarkLandmark
{
    public class Model : INotifyPropertyChanged
    {
        #region [Field]

        private DirectoryInfo dsetFolder;

        private ObservableCollection<DirectoryInfo> imgFolders = new ObservableCollection<DirectoryInfo>();

        private ObservableCollection<DirectoryInfo> fppFolders = new ObservableCollection<DirectoryInfo>();

        private ObservableCollection<FileInfo> imgList = new ObservableCollection<FileInfo>();

        private ObservableCollection<FileInfo> fppList = new ObservableCollection<FileInfo>();

        private ObservableCollection<Landmark> landmarks = new ObservableCollection<Landmark>();

        #endregion

        #region [Properties]

        public DirectoryInfo DsetFolder
        {
            get
            {
                return this.dsetFolder;
            }
            set
            {
                if (value != this.dsetFolder)
                {
                    this.dsetFolder = value;
                    RaisePropertyChanged("DsetFolder");
                }
            }
        }

        public ObservableCollection<DirectoryInfo> ImgFolders
        {
            get { return this.imgFolders; }
        }

        public ObservableCollection<DirectoryInfo> FppFolders
        {
            get { return this.fppFolders; }
        }



        public ObservableCollection<FileInfo> ImgList
        {
            get { return this.imgList; }
        }

        public ObservableCollection<FileInfo> FppList
        {
            get { return this.fppList; }
        }

        public ObservableCollection<Landmark> Landmarks
        {
            get
            {
                return this.landmarks;
            }
            set
            {
                if (value != this.landmarks)
                {
                    this.landmarks = value;
                    RaisePropertyChanged("Landmarks");
                }
            }
        }

        #endregion

        #region [Event]

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [Functions]

        private void RaisePropertyChanged(string p)
        {
            var h = this.PropertyChanged;

            if (null != h)
            {
                h(this, new PropertyChangedEventArgs(p));
            }
        }

        #endregion
    }
}
