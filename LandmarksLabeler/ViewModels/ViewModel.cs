using System;
using System.Linq;
using System.Threading.Tasks;
using LandmarksLabeler.Models;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.Storage.Search;
using Windows.Storage.Pickers;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using System.IO;
using System.Collections.Specialized;

namespace LandmarksLabeler.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int _folderIndex;
        private int _fppIndex;
        private double _scale;
        private string _fppName;
        private double _imageWidth;
        private double _imageHeight;

        private double _imagePanelWidth;
        private double _imagePanelHeight;
        public void ImagePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is Panel panel)) return;
            _imagePanelWidth = panel.ActualWidth;
            _imagePanelHeight = panel.ActualHeight;
        }

        private readonly Model _model = new Model();

        public ObservableCollection<LandmarksViewModel> RenderedLandmarks { get; set; } = new ObservableCollection<LandmarksViewModel>();
        public void OnRenderedLandmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
                foreach (LandmarksViewModel item in e.NewItems)
                    item.PropertyChanged += Landmark_PropertyChanged;

            if (e.OldItems != null)
                foreach (LandmarksViewModel item in e.OldItems)
                    item.PropertyChanged -= Landmark_PropertyChanged;

        }

        private void Landmark_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "LandmarksViewModel") return;
            var lm = sender as LandmarksViewModel;
            var index = RenderedLandmarks.IndexOf(lm);
            Coor_RenderToActual(out var X, out var Y, lm.X, lm.Y);
            _model.Landmarks[index] = new Landmarks { X = X, Y = Y };
        }

        private BitmapImage imageSource;
        public BitmapImage ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }

        public ViewModel()
        {
            RenderedLandmarks.CollectionChanged += OnRenderedLandmarks_CollectionChanged;
        }





        //显示图像及landmarks
        public async void OpenDataset(object sender, RoutedEventArgs e)
        {
            const CommonFileQuery query = CommonFileQuery.DefaultQuery;

            try
            {
                var folderPicker = new FolderPicker()
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                folderPicker.FileTypeFilter.Add("*");

                var parentFolder = await folderPicker.PickSingleFolderAsync();

                if (parentFolder == null) return;

                //选取各子文件夹
                var queryOptions = new QueryOptions(query, new[] { "*" }) { FolderDepth = FolderDepth.Shallow };
                var subfolders = await parentFolder.GetFoldersAsync();
                foreach (var folder in subfolders)
                {
                    if (folder.Name == "img")
                        _model.ImgFolders = await folder.GetFoldersAsync();
                    else if (folder.Name == "fpp")
                        _model.FppFolders = await folder.GetFoldersAsync();
                }
                _folderIndex = 0;

                //选取第一个子文件夹
                await OpenSubFolder();
            }
            catch (Exception)
            {
                ClearVisualization();
            }
        }

        private async Task OpenSubFolder()
        {
            const CommonFileQuery query = CommonFileQuery.DefaultQuery;

            //选取图片
            var queryOptions = new QueryOptions(query, new[] { ".jpg" }) { FolderDepth = FolderDepth.Shallow };
            var queryResult = _model.ImgFolders[_folderIndex].CreateFileQueryWithOptions(queryOptions);
            _model.ImgList = await queryResult.GetFilesAsync();

            //选取标签
            queryOptions = new QueryOptions(query, new[] { ".txt" }) { FolderDepth = FolderDepth.Shallow };
            queryResult = _model.FppFolders[_folderIndex].CreateFileQueryWithOptions(queryOptions);
            _model.FppList = await queryResult.GetFilesAsync();

            //选取第一张图
            _fppIndex = 0;
            DisplayImage();
        }

        private async void DisplayImage()
        {
            ClearVisualization();

            //以landmark文件为索引
            if (_model.FppList.Count == 0) return;

            var fpp = _model.FppList[_fppIndex];
            var fppContent = await FileIO.ReadTextAsync(_model.FppList[_fppIndex]);

            _fppName = fpp.Name.Substring(0, fpp.Name.IndexOf(".", StringComparison.Ordinal));

            var imgsNames = _model.ImgList.Select(o => o.Name.Substring(0, o.Name.IndexOf(".", StringComparison.Ordinal))).ToList();

            var image = _model.ImgList[imgsNames.IndexOf(_fppName)];

            var bitmapImage = new BitmapImage();
            var stream = (FileRandomAccessStream)await image.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            ImageSource = bitmapImage;

            if (!string.IsNullOrEmpty(fppContent))
            {
                DisplayLandmarks(fppContent);
            }
        }

        private void DisplayLandmarks(string labelContent)
        {
            if (ImageSource == null) throw new ArgumentNullException(nameof(ImageSource));

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
                while ((line = reader.ReadLine()) != null)
                {
                    AddLandmark(line);
                }
            }

        }

        private void AddLandmark(string input)
        {
            var splitString = input.Split();
            var x = Convert.ToDouble(splitString[0]);
            var y = Convert.ToDouble(splitString[1]);
            _model.Landmarks.Add(new Landmarks { X = x, Y = y });
            Coor_ActualToRender(out double renderedX, out double renderedY, x, y);
            RenderedLandmarks.Add(new LandmarksViewModel { X = renderedX, Y = renderedY });
        }

        private void ClearVisualization()
        {
            _model.Landmarks.Clear();
            RenderedLandmarks.Clear();
        }

        //图片与panel坐标转换
        private void Coor_ActualToRender(out double left, out double top, double x, double y)
        {
            left = _imagePanelWidth / 2 - _imageWidth / 2 + x / _scale - 5;
            top = y / _scale - 5;
        }

        private void Coor_RenderToActual(out double x, out double y, double left, double top)
        {
            x = (left + 5 - _imagePanelWidth / 2 + _imageWidth / 2) * _scale;
            y = (top + 5) * _scale;
        }





        //保存
        public async void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var label = "";

            foreach (var item in _model.Landmarks)
            {
                label += (item.ToString() + "\n");
            }

            var labelFile =
                await _model.FppFolders[_folderIndex].CreateFileAsync(_fppName + ".txt",
                    CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(labelFile, label);
        }





        //浏览控制
        public async Task PreviousOrNextClick(object sender, RoutedEventArgs e)
        {
            if (_model.ImgList == null) return;
            if (sender != null)
            {
                if (!(sender is AppBarButton button)) return;

                switch (button.Tag)
                {
                    case "0":
                        if (_fppIndex == 0 &&
                            _folderIndex == 0)
                            return;
                        else if (_fppIndex == 0)
                        {
                            _folderIndex--;
                            await OpenSubFolder();
                            return;
                        }
                        else
                        {
                            _fppIndex--;
                        }
                        break;
                    default:
                        if (_fppIndex == _model.ImgList.Count - 1 &&
                            _folderIndex == _model.ImgFolders.Count - 1)
                            return;
                        else if (_fppIndex == _model.ImgList.Count - 1)
                        {
                            _folderIndex++;
                            await OpenSubFolder();
                            return;
                        }
                        else
                        {
                            _fppIndex++;
                        }
                        break;
                }
            }

            try
            {
                if (_fppIndex == _model.ImgList.Count)
                    _fppIndex--;
                DisplayImage();
            }
            catch (Exception)
            {
                ClearVisualization();
            }
        }
    }

    public class LandmarksViewModel : INotifyPropertyChanged

    {
        private double x;
        public double X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChanged("LandmarksViewModel");
            }
        }

        private double y;
        public double Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChanged("LandmarksViewModel");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
