using System.Collections.Generic;
using Windows.Storage;

namespace LandmarksLabeler.Models
{
    public class Model
    {
        public StorageFolder CurFolder { get; set; }
        public IReadOnlyList<StorageFolder> ImgFolders { get; set; } = new List<StorageFolder>();
        public IReadOnlyList<StorageFolder> FppFolders { get; set; } = new List<StorageFolder>();
        public IReadOnlyList<StorageFile> ImgList { get; set; } = new List<StorageFile>();
        public IReadOnlyList<StorageFile> FppList { get; set; } = new List<StorageFile>();
        public List<Landmarks> Landmarks { get; set; } = new List<Landmarks>();
    }

    public class Landmarks
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString()
        {
            return X.ToString() + " " + Y.ToString();
        }
    }
}
