using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MarkLandmark
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region [Members]

        private int _folderIndex;
        private int _fileIndex;
        private double _scale;
        private string _fileName;
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
        private String logPathRelative = "\\log";

        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };
        private static string password = "13104036";

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

        public ICommand WndLoaded
        {
            get
            {
                return new RelayCommand(OnWndLoaded);
            }
        }

        public ICommand WndClosed
        {
            get
            {
                return new RelayCommand(OnWndClosed);
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

        private void OnWndLoaded()
        {
            Log.LogInfo(Log.EventName.Launch, 
                String.Format("{0} {1} {2}",
                Environment.OSVersion, 
                Environment.MachineName, 
                Environment.UserName));
        }

        private void OnWndClosed()
        {
            Log.LogInfo(Log.EventName.Close,
                String.Format("{0} {1} {2}",
                Environment.OSVersion,
                Environment.MachineName,
                Environment.UserName));
        }

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

                Log.LogInfo(Log.EventName.OpenDataset, folderPicker.SelectedPath);
                _model.DsetFolder = new DirectoryInfo(folderPicker.SelectedPath);
                var subfolders = _model.DsetFolder.GetDirectories();

                //判断是否存在img/fpp文件夹
                if (!subfolders.Select(o => o.Name).ToList().Contains("img") ||
                    !subfolders.Select(o => o.Name).ToList().Contains("img"))
                    throw new FileNotFoundException("ParentFolder");

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

                if (_model.ImgFolders.Count == 0 || _model.FppFolders.Count == 0)
                    throw new FileNotFoundException("SubFolder");

                //加载当前工作进度
                LoadProgress();

                IsNextEnabled = !(_fileIndex == (_model.ImgList.Count - 1) &&
                                  (_folderIndex != (_model.ImgFolders.Count - 1)));
                IsPreviousEnabled = !(_fileIndex == 0 && _folderIndex == 0);

                //选取当前工作子文件夹
                OpenSubFolder(null);
            }
            catch (Exception ex)
            {
                ClearVisualization();

                MessageBoxResult result;
                if (ex.Message == "ParentFolder")
                {
                    result = System.Windows.MessageBox.Show(
                        (String) App.Current.FindResource("Promp_DatasetMisingImgFpp"),
                        "",
                        MessageBoxButton.YesNo);
                }
                else if (ex.Message == "SubFolder")
                {
                    result = System.Windows.MessageBox.Show(
                        (String)App.Current.FindResource("Promp_DatasetMisingSubfolders"),
                        "",
                        MessageBoxButton.YesNo);
                }
                else
                {
                    result = MessageBoxResult.No;
                }

                if (result == MessageBoxResult.Yes)
                {
                    OnOpenDataset();
                }
            }
        }
        
        private void OpenSubFolder(bool? pickFirst = true)
        {
            //选取图片
            _model.ImgList.Clear();
            foreach (var x in _model.ImgFolders[_folderIndex].GetFiles("*.jpg"))
            {
                _model.ImgList.Add(x);
            }
            if (_model.ImgList.Count == 0)
                OnNextFolder();

            //选取标签
            _model.FppList.Clear();
            foreach (var x in _model.FppFolders[_folderIndex].GetFiles("*.txt"))
            {
                _model.FppList.Add(x);
            }

            //选取第一张图
            if (pickFirst == true)
                _fileIndex = 0;
            else if (pickFirst == false)
                _fileIndex = (_model.ImgList.Count - 1);

            UpdateIsPreviousNextFolderEnabled();
            DisplayImage();
        }

        private void UpdateIsPreviousNextFolderEnabled()
        {
            IsPreviousFolderEnabled = _folderIndex > 0;
            IsNextFolderEnabled = _folderIndex >= 0 && _folderIndex < (_model.FppFolders.Count - 1);
        }

        private void DisplayImage()
        {
            ClearVisualization();

            ////以landmark文件为索引
            //if (_model.FppList.Count == 0) return;

            //var fpp = _model.FppList[_fileIndex];
            //String fppContent = String.Empty;
            //using (var sr = new StreamReader(fpp.FullName))
            //{
            //    fppContent = sr.ReadToEnd();
            //}
            //_fileName = fpp.Name.Substring(0, fpp.Name.IndexOf(".", StringComparison.Ordinal));

            //var imgsNames = _model.ImgList.Select(o => o.Name.Substring(0, o.Name.IndexOf(".", StringComparison.Ordinal))).ToList();

            //var image = _model.ImgList[imgsNames.IndexOf(_fileName)];

            //var bitmapImage = new BitmapImage(new Uri(image.FullName, UriKind.Absolute));
            //ImageSource = bitmapImage;
            //ImagePath = _model.ImgFolders[_folderIndex].FullName + "\\" + _fileName + ".jpg";

            //以jpg文件为索引
            if (_model.ImgList.Count == 0) return;


            var img = _model.ImgList[_fileIndex];

            var bitmapImage = new BitmapImage(new Uri(img.FullName, UriKind.Absolute));
            ImageSource = bitmapImage;
            ImagePath = _model.ImgFolders[_folderIndex].FullName + "\\" + _fileName + ".jpg";
            Log.LogInfo(Log.EventName.OpenImage, ImagePath);

            _fileName = img.Name.Substring(0, img.Name.IndexOf(".", StringComparison.Ordinal));
            var fppsNames = _model.FppList.Select(o => o.Name.Substring(0, o.Name.IndexOf(".", StringComparison.Ordinal))).ToList();

            var fpp = _model.FppList[fppsNames.IndexOf(_fileName)];
            String fppContent = String.Empty;
            using (var sr = new StreamReader(fpp.FullName))
            {
                fppContent = sr.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(fppContent))
            {
                DisplayLandmarks(fppContent);
            }

            UpdateIsSaveEnabled();

            UpdateIsPreviousNextEnabled();
        }

        private void UpdateIsPreviousNextEnabled()
        {
            IsPreviousEnabled = !(_fileIndex == 0 && _folderIndex == 0);
            IsNextEnabled = !(_fileIndex == (_model.ImgList.Count - 1) && _folderIndex == (_model.ImgFolders.Count - 1));
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
            var landmark = new Landmark { X = renderedX, Y = renderedY, Name = name, Visibility = visibility };
            landmark.LandmarkMoveStart += OnLandmarkMoveStart;
            landmark.LandmarkMoveEnd += OnLandmarkMoveEnd;
            RenderedLandmarks.Add(landmark);
        }

        private void OnLandmarkMoveStart(object sender, EventArgs e)
        {
            Landmark landmark = (Landmark)sender;
            double x, y;
            Coor_RenderToActual(out x, out y, landmark.X, landmark.Y);
            Log.LogInfo(Log.EventName.MoveLandmarkStart, String.Format("{0} {1} {2} {3}", imagePath, landmark.Name, x, y));
        }

        private void OnLandmarkMoveEnd(object sender, EventArgs e)
        {
            Landmark landmark = (Landmark)sender;
            double x, y;
            Coor_RenderToActual(out x, out y, landmark.X, landmark.Y);
            Log.LogInfo(Log.EventName.MoveLandmarkEnd, String.Format("{0} {1} {2} {3}", imagePath, landmark.Name, x, y));
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

            if (_fileIndex == 0)
            {
                OnPreviousFolder(false);
            }
            else
            {
                AutoSave();
                --_fileIndex;

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


        }

        private void OnNext()
        {
            if (!IsNextEnabled)
            {
                return;
            }

            if (_fileIndex == (_model.ImgList.Count - 1))
            {
                OnNextFolder();
            }
            else
            {
                AutoSave();
                ++_fileIndex;

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
            else
            {
                System.Windows.MessageBox.Show(
                    (String)App.Current.FindResource("Prompt_LastFolder"), null);
            }
        }
        
        private void OnSave()
        {
            if (!IsSaveEnabled)
            {
                return;
            }
            SaveProgress();

            UpdateEyeCenter();

            var label = (from Landmark pnt in renderedLandmarks
                         select Coor_RenderToActual(pnt.X, pnt.Y).ToString())
                         .Aggregate((x, y) => { return x + "\n" + y; });

            string dstFileName = _model.FppFolders[_folderIndex].FullName + "/" + _fileName + ".txt";

            using (StreamWriter labelFile = new StreamWriter(dstFileName))
            {
                labelFile.Write(label);
                labelFile.Flush();
            }

            Log.LogInfo(Log.EventName.Save, dstFileName);
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
            else
            {
                SaveProgress();
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

        private void LoadProgress()
        {
            var logPath = _model.DsetFolder + logPathRelative;
            String logContent;
            try
            {
                byte[] logContentEncrypted = System.IO.File.ReadAllBytes(logPath);

                using (Rijndael myRijndael = Rijndael.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
                    myRijndael.Key = pdb.GetBytes(32);
                    myRijndael.IV = pdb.GetBytes(16);

                    // Decrypt the bytes to a string.
                    logContent = DecryptStringFromBytes(logContentEncrypted, myRijndael.Key, myRijndael.IV);

                }
                

                var tokens = logContent.Split('-');
                int folderProgress, fppProgress;
                if (tokens[0] == "GODNESSOFLABELLING" && tokens[1] == "DIR" && tokens[3] == "FPP" //格式
                    && Int32.TryParse(tokens[2], out folderProgress) &&
                    Int32.TryParse(tokens[4], out fppProgress) //合法数字
                    && folderProgress >= 0 && folderProgress < _model.FppFolders.Count //合法folder index
                    && fppProgress >= 0 &&
                    fppProgress < _model.FppFolders[folderProgress].GetFiles().Length) //合法fpp index
                {
                    if (folderProgress == _model.FppFolders.Count - 1
                        && fppProgress == _model.FppFolders[folderProgress].GetFiles().Length - 1)
                    {
                        var result = System.Windows.MessageBox.Show((String)App.Current.FindResource("Prompt_LabellingFinish"), (String)App.Current.FindResource("Prompt_LabellingProgress"));

                        _folderIndex = 0;
                        _fileIndex = 0;
                    }
                    else
                    {
                        var result =
                            System.Windows.MessageBox.Show((String)App.Current.FindResource("Prompt_LabellingResume"), (String)App.Current.FindResource("Prompt_LabellingProgress"), MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (fppProgress < _model.FppFolders[folderProgress].GetFiles().Length - 1)
                            {
                                _folderIndex = folderProgress;
                                _fileIndex = fppProgress + 1;
                            }
                            else
                            {
                                _folderIndex = folderProgress + 1;
                                _fileIndex = 0;
                            }

                        }
                        else
                        {
                            _folderIndex = 0;
                            _fileIndex = 0;
                        }
                    }

                }
                else
                    throw new FileFormatException();

            }
            catch (Exception ex)
            {
                if (ex is FileFormatException)
                {
                    var result = System.Windows.MessageBox.Show((String)App.Current.FindResource("Prompt_LabellingCorrupt"), (String)App.Current.FindResource("Prompt_LabellingProgress"));

                    while (true)
                    {
                        var filePicker = new OpenFileDialog();

                        var sfr = filePicker.ShowDialog();

                        if (sfr != DialogResult.OK && sfr != DialogResult.Yes)
                            return;

                        var file = new FileInfo(filePicker.FileName);
                        var folder = new DirectoryInfo(file.DirectoryName);
                        var folderIndex = _model.ImgFolders.Select(o => o.Name).ToList().IndexOf(folder.Name);
                        if (folderIndex != -1)
                        {
                            var fppIndex = _model.ImgFolders[folderIndex].GetFiles().Select(o => o.Name).ToList()
                                .IndexOf(file.Name);
                            if (fppIndex != -1)
                            {
                                _fileIndex = fppIndex;
                                _folderIndex = folderIndex;
                                return;
                            }
                        }
                        System.Windows.MessageBox.Show((String)App.Current.FindResource("Prompt_LabellingSelectError"), (String)App.Current.FindResource("Prompt_LabellingProgress"));
                    }
                }
                else if (ex is FileNotFoundException)
                {
                    _folderIndex = 0;
                    _fileIndex = 0;
                }

            }

        }

        private void SaveProgress()
        {
            var logContent = "GODNESSOFLABELLING-DIR-" + _folderIndex + "-FPP-" + _fileIndex;
            var logPath = _model.DsetFolder + logPathRelative;

            using (Rijndael myRijndael = Rijndael.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
                myRijndael.Key = pdb.GetBytes(32);
                myRijndael.IV = pdb.GetBytes(16);

                // Encrypt the string to an array of bytes.
                byte[] logContentEncripted = EncryptStringToBytes(logContent, myRijndael.Key, myRijndael.IV);
                using (BinaryWriter bw = new BinaryWriter(new FileStream(logPath, FileMode.Create)))
                {
                    bw.Write(logContentEncripted);
                    bw.Flush();
                }
            }
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

        #endregion
    }
}
