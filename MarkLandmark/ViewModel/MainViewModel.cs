using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MarkLandmark
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region [Members]

        private int _folderIndex;
        private int _fppIndex;
        private double _scale;
        private string _fppName;
        private double _imageWidth;
        private double _imageHeight;

        private double _imagePanelWidth;
        private double _imagePanelHeight;

        private bool isSaveEnabled;
        private bool isPreviousEnabled;
        private bool isNextEnabled;
        private bool isPreviousFolderEnabled;
        private bool isNextFolderEnabled;

        private BitmapImage imageSource;
        private String imagePath = (String)App.Current.FindResource("Prompt_OpenDatabase");

        private Model _model = new Model();

        private ObservableCollection<Landmark> renderedLandmarks = new ObservableCollection<Landmark>();

        public event PropertyChangedEventHandler PropertyChanged;



        #endregion

        #region [Properties]

        public ObservableCollection<Landmark> RenderedLandmarks
        {
            get { return renderedLandmarks; }
        }

        public BitmapImage ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                if (value != imageSource)
                {
                    imageSource = value;
                    RaisePropertyChanged("ImageSource");
                }
            }
        }

        public String ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    RaisePropertyChanged("ImagePath");
                }
            }
        }

        public ICommand OpenDatasetCmd
        {
            get
            {
                return new RelayCommand(OnOpenDataset);
            }
        }

        public ICommand SaveCmd
        {
            get
            {
                return new RelayCommand(OnSave);
            }
        }

        public ICommand PreviousCmd
        {
            get
            {
                return new RelayCommand(OnPrevious);
            }
        }

        public ICommand NextCmd
        {
            get
            {
                return new RelayCommand(OnNext);
            }
        }

        public ICommand PreviousFolderCmd
        {
            get
            {
                return new RelayCommand(OnPreviousFolderWrapper);
            }
        }

        public ICommand NextFolderCmd
        {
            get
            {
                return new RelayCommand(OnNextFolder);
            }
        }

        public ICommand KeyDownCmd
        {
            get
            {
                return new RelayCommand<System.Windows.Input.KeyEventArgs>(OnKeyDown);
            }
        }

        public ICommand CloseCmd
        {
            get
            {
                return new RelayCommand(OnClose);
            }
        }

        public bool IsSaveEnabled
        {
            get
            {
                return isSaveEnabled;
            }
            set
            {
                if (value != isSaveEnabled)
                {
                    isSaveEnabled = value;
                    RaisePropertyChanged("IsSaveEnabled");
                }
            }
        }

        public bool IsPreviousEnabled
        {
            get
            {
                return isPreviousEnabled;
            }
            set
            {
                if (value != isPreviousEnabled)
                {
                    isPreviousEnabled = value;
                    RaisePropertyChanged("IsPreviousEnabled");
                }
            }
        }

        public bool IsNextEnabled
        {
            get
            {
                return isNextEnabled;
            }
            set
            {
                if (value != isNextEnabled)
                {
                    isNextEnabled = value;
                    RaisePropertyChanged("IsNextEnabled");
                }
            }
        }

        public bool IsPreviousFolderEnabled
        {
            get
            {
                return isPreviousFolderEnabled;
            }
            set
            {
                if(value != isPreviousFolderEnabled)
                {
                    isPreviousFolderEnabled = value;
                    RaisePropertyChanged("IsPreviousFolderEnabled");
                }
            }
        }

        public bool IsNextFolderEnabled
        {
            get
            {
                return isNextFolderEnabled;
            }
            set
            {
                if(value != isNextFolderEnabled)
                {
                    isNextFolderEnabled = value;
                    RaisePropertyChanged("IsNextFolderEnabled");
                }
            }
        }

        public double ImagePanelWidth
        {
            set
            {
                if (value != _imagePanelWidth)
                {
                    _imagePanelWidth = value;
                    RaisePropertyChanged("ImagePanelWidth");
                }
            }
        }

        public double ImagePanelHeight
        {
            set
            {
                if (value != _imagePanelHeight)
                {
                    _imagePanelHeight = value;
                    RaisePropertyChanged("ImagePanelHeight");
                }
            }
        }

        #endregion

        #region [Functions]

        private void OnImagePanelSizeChanged(SizeChangedEventArgs e)
        {
            _imagePanelWidth = e.NewSize.Width;
            _imagePanelHeight = e.NewSize.Height;
        }

        //显示图像及landmarks
        private void OnOpenDataset()
        {
            try
            {
                var folderPicker = new FolderBrowserDialog();

                var sfr = folderPicker.ShowDialog();

                if (sfr != DialogResult.OK && sfr != DialogResult.Yes)
                    return;

                //选取各子文件夹
                DirectoryInfo di = new DirectoryInfo(folderPicker.SelectedPath);
                var subfolders = di.GetDirectories();

                foreach (var folder in subfolders)
                {
                    if (folder.Name == "img")
                    {
                        _model.ImgFolders.Clear();
                        foreach (var x in folder.GetDirectories())
                        {
                            if (x.GetFiles().Count() > 0)
                            {
                                _model.ImgFolders.Add(x);
                            }
                        }
                    }
                    else if (folder.Name == "fpp")
                    {
                        _model.FppFolders.Clear();
                        foreach (var x in folder.GetDirectories())
                        {
                            if (x.GetFiles().Count() > 0)
                            {
                                _model.FppFolders.Add(x);
                            }
                        }
                    }
                }
                _folderIndex = 0;

                IsNextEnabled = !(_fppIndex == (_model.ImgList.Count - 1) && (_folderIndex != (_model.ImgFolders.Count - 1)));
                IsPreviousEnabled = !(_fppIndex == 0 && _folderIndex == 0);

                //选取第一个子文件夹
                OpenSubFolder();
            }
            catch (Exception)
            {
                ClearVisualization();
            }
        }

        private void OpenSubFolder(bool pickFirst = true)
        {
            //选取图片
            _model.ImgList.Clear();
            foreach (var x in _model.ImgFolders[_folderIndex].GetFiles("*.jpg"))
            {
                _model.ImgList.Add(x);
            }

            //选取标签
            _model.FppList.Clear();
            foreach (var x in _model.FppFolders[_folderIndex].GetFiles("*.txt"))
            {
                _model.FppList.Add(x);
            }

            //选取第一张图
            _fppIndex = pickFirst ? 0 : (_model.ImgList.Count - 1);
            IsPreviousFolderEnabled = _folderIndex > 0;
            IsNextFolderEnabled = _folderIndex >= 0 && _folderIndex < (_model.FppFolders.Count - 1);
            DisplayImage();
        }

        private void DisplayImage()
        {
            ClearVisualization();

            //以landmark文件为索引
            if (_model.FppList.Count == 0) return;

            var fpp = _model.FppList[_fppIndex];
            String fppContent = String.Empty;
            using (var sr = new StreamReader(_model.FppList[_fppIndex].FullName))
            {
                fppContent = sr.ReadToEnd();
            }
            _fppName = fpp.Name.Substring(0, fpp.Name.IndexOf(".", StringComparison.Ordinal));

            var imgsNames = _model.ImgList.Select(o => o.Name.Substring(0, o.Name.IndexOf(".", StringComparison.Ordinal))).ToList();

            var image = _model.ImgList[imgsNames.IndexOf(_fppName)];

            var bitmapImage = new BitmapImage(new Uri(image.FullName, UriKind.Absolute));
            ImageSource = bitmapImage;
            ImagePath = _model.ImgFolders[_folderIndex].FullName + "\\" + _fppName + ".jpg";

            if (!string.IsNullOrEmpty(fppContent))
            {
                DisplayLandmarks(fppContent);
            }

            UpdateIsSaveEnabled();
        }

        private void DisplayLandmarks(string labelContent)
        {
            if (ImageSource == null)
            {
                throw new ArgumentNullException("ImageSource");
            }

            //获取缩放比例
            if (ImageSource.PixelWidth / _imagePanelWidth < ImageSource.PixelHeight / _imagePanelHeight)
            {
                _scale = ImageSource.PixelHeight / _imagePanelHeight;

                _imageHeight = _imagePanelHeight;
                _imageWidth = ImageSource.PixelWidth / _scale;
            }
            else
            {
                _scale = ImageSource.PixelWidth / _imagePanelWidth;

                _imageWidth = _imagePanelWidth;
                _imageHeight = ImageSource.PixelHeight / _scale;
            }

            using (var reader = new StringReader(labelContent))
            {
                string line;
                int idx = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    AddLandmark(line, idx);
                    ++idx;
                }
            }

        }

        private void AddLandmark(string input, int idx)
        {
            var splitString = input.Split();
            var x = Convert.ToDouble(splitString[0]);
            var y = Convert.ToDouble(splitString[1]);
            _model.Landmarks.Add(new Landmark { X = x, Y = y, Name = (Landmark.LandmarkName)(idx) });
            double renderedX, renderedY;
            Coor_ActualToRender(out renderedX, out renderedY, x, y);
            var name = (Landmark.LandmarkName) (idx);
            var visibility =
                (name == Landmark.LandmarkName.left_eye_center || name == Landmark.LandmarkName.right_eye_center)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            RenderedLandmarks.Add(new Landmark { X = renderedX, Y = renderedY, Name = name, Visibility = visibility});
        }

        private void ClearVisualization()
        {
            _model.Landmarks.Clear();
            RenderedLandmarks.Clear();
        }

        private void OnPrevious()
        {
            if (!IsPreviousEnabled)
            {
                return;
            }

            if (_fppIndex == 0)
            {
                OnPreviousFolder(false);
            }
            else
            {
                AutoSave();
                --_fppIndex;
                IsNextEnabled = true;
                IsPreviousEnabled = !(_fppIndex == 0 && _folderIndex == 0);
                DisplayImage();
            }
        }

        private void OnPreviousFolderWrapper()
        {
            OnPreviousFolder(true);
        }

        private void OnPreviousFolder(bool pickFirst=true)
        {
            if (_folderIndex != 0)
            {
                AutoSave();
                --_folderIndex;
                OpenSubFolder(pickFirst);
            }

            IsPreviousEnabled = !(_fppIndex == 0 && _folderIndex == 0);
            IsNextEnabled = !(_fppIndex == (_model.ImgList.Count - 1) && _folderIndex == (_model.ImgFolders.Count - 1));
        }

        private void OnNext()
        {
            if (!IsNextEnabled)
            {
                return;
            }

            if (_fppIndex == (_model.ImgList.Count - 1))
            {
                OnNextFolder();
            }
            else
            {
                AutoSave();
                ++_fppIndex;
                IsPreviousEnabled = true;
                IsNextEnabled = !(_fppIndex == 0 && _folderIndex == 0);
                DisplayImage();
            }
        }

        private void OnNextFolder()
        {
            if (_folderIndex != (_model.ImgFolders.Count - 1))
            {
                AutoSave();
                ++_folderIndex;
                OpenSubFolder();
            }

            IsNextEnabled = !(_fppIndex == (_model.ImgList.Count - 1) && _folderIndex == (_model.ImgFolders.Count - 1));
            IsPreviousEnabled = !(_fppIndex == 0 && _folderIndex == 0);
        }


        private void OnSave()
        {
            if (!IsSaveEnabled)
            {
                return;
            }

            UpdateEyeCenter();

            var label = (from Landmark pnt in renderedLandmarks
                         select Coor_RenderToActual(pnt.X, pnt.Y).ToString())
                         .Aggregate((x, y) => { return x + "\n" + y; });

            string dstFileName = _model.FppFolders[_folderIndex].FullName + "/" + _fppName + ".txt";

            using (StreamWriter labelFile = new StreamWriter(dstFileName))
            {
                labelFile.Write(label);
                labelFile.Flush();
            }
        }

        private void UpdateEyeCenter()
        {
            var lEyeCenter = RenderedLandmarks[(int) Landmark.LandmarkName.left_eye_center];
            var rEyeCenter = RenderedLandmarks[(int)Landmark.LandmarkName.right_eye_center];
            double lSumX = 0;
            double lSumY = 0;
            double rSumX = 0;
            double rSumY = 0;
            int lCount = 0;
            int rCount = 0;
            foreach (var landmark in RenderedLandmarks)
            {
                var name = landmark.Name.ToString();
                if (name.StartsWith("left_eye_") && !name.Equals("left_eye_center"))
                {
                    lSumX += landmark.X;
                    lSumY += landmark.Y;
                    lCount += 1;
                }
                else if (name.StartsWith("right_eye_") && !name.Equals("right_eye_center"))
                {
                    rSumX += landmark.X;
                    rSumY += landmark.Y;
                    rCount += 1;
                }
            }
            lEyeCenter.X = lSumX / lCount;
            lEyeCenter.Y = lSumY / lCount;
            rEyeCenter.X = rSumX / rCount;
            rEyeCenter.Y = rSumY / rCount;
        }

        private void UpdateIsSaveEnabled()
        {
            IsSaveEnabled = (null != ImageSource);
        }

        private void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            OnPreviousFolder(true);
                        else
                            OnPrevious();
                    }
                    break;
                case Key.Right:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            OnNextFolder();
                        else
                            OnNext();
                    }
                    break;
                case Key.S:
                    {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            OnSave();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        //图片与panel坐标转换
        private void Coor_ActualToRender(out double left, out double top, double x, double y)
        {
            left = _imagePanelWidth / 2 - _imageWidth / 2 + x / _scale;
            top = y / _scale;
        }

        private Landmark Coor_ActualToRender(double x, double y)
        {
            double left = _imagePanelWidth / 2 - _imageWidth / 2 + x / _scale;
            double top = y / _scale;

            return new Landmark() { X = left, Y = top };
        }

        private void Coor_RenderToActual(out double x, out double y, double left, double top)
        {
            x = (left - _imagePanelWidth / 2 + _imageWidth / 2) * _scale;
            y = top * _scale;
        }

        private Landmark Coor_RenderToActual(double left, double top)
        {
            double x = (left - _imagePanelWidth / 2 + _imageWidth / 2) * _scale;
            double y = top * _scale;

            return new Landmark() { X = x, Y = y };
        }

        private void RaisePropertyChanged(string p)
        {
            var h = this.PropertyChanged;

            if (null != h)
            {
                h(this, new PropertyChangedEventArgs(p));
            }
        }

        private void AutoSave()
        {
            if (renderedLandmarks.Count > 0 && renderedLandmarks.Any(x => x.IsDirty))
            {
                OnSave();
            }
        }

        private void OnWindowClosing()
        {
            if (renderedLandmarks.Count > 0 && renderedLandmarks.Any(x => x.IsDirty))
            {
                var result = System.Windows.MessageBox.Show(
                    (String)App.Current.FindResource("Prompt_OnClosingSave_Text"),
                    (String)App.Current.FindResource("Prompt_OnClosingSave_Caption"),
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    OnSave();
                }
            }
        }

        private void OnClose()
        {
            OnWindowClosing();

            foreach (var wnd in System.Windows.Application.Current.Windows)
            {
                if (wnd is Window)
                {
                    var x = (Window)wnd;
                    x.Close();
                }
            }
        }

        #endregion
    }
}
