using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MarkLandmark
{
    class LandmarkNameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Landmark.LandmarkName)
            {
                Landmark.LandmarkName name = (Landmark.LandmarkName)value;

                switch (name)
                {
                    case Landmark.LandmarkName.contour_chin:
                    case Landmark.LandmarkName.contour_left1:
                    case Landmark.LandmarkName.contour_left2:
                    case Landmark.LandmarkName.contour_left3:
                    case Landmark.LandmarkName.contour_left4:
                    case Landmark.LandmarkName.contour_left5:
                    case Landmark.LandmarkName.contour_left6:
                    case Landmark.LandmarkName.contour_left7:
                    case Landmark.LandmarkName.contour_left8:
                    case Landmark.LandmarkName.contour_left9:
                    case Landmark.LandmarkName.contour_right1:
                    case Landmark.LandmarkName.contour_right2:
                    case Landmark.LandmarkName.contour_right3:
                    case Landmark.LandmarkName.contour_right4:
                    case Landmark.LandmarkName.contour_right5:
                    case Landmark.LandmarkName.contour_right6:
                    case Landmark.LandmarkName.contour_right7:
                    case Landmark.LandmarkName.contour_right8:
                    case Landmark.LandmarkName.contour_right9:
                        return FaceContourColor;
                    case Landmark.LandmarkName.left_eye_bottom:
                    case Landmark.LandmarkName.left_eye_center:
                    case Landmark.LandmarkName.left_eye_left_corner:
                    case Landmark.LandmarkName.left_eye_lower_left_quarter:
                    case Landmark.LandmarkName.left_eye_lower_right_quarter:
                    case Landmark.LandmarkName.left_eye_pupil:
                    case Landmark.LandmarkName.left_eye_right_corner:
                    case Landmark.LandmarkName.left_eye_top:
                    case Landmark.LandmarkName.left_eye_upper_left_quarter:
                    case Landmark.LandmarkName.left_eye_upper_right_quarter:
                        return EyeColor;
                    case Landmark.LandmarkName.left_eyebrow_left_corner:
                    case Landmark.LandmarkName.left_eyebrow_lower_left_quarter:
                    case Landmark.LandmarkName.left_eyebrow_lower_middle:
                    case Landmark.LandmarkName.left_eyebrow_lower_right_quarter:
                    case Landmark.LandmarkName.left_eyebrow_right_corner:
                    case Landmark.LandmarkName.left_eyebrow_upper_left_quarter:
                    case Landmark.LandmarkName.left_eyebrow_upper_middle:
                    case Landmark.LandmarkName.left_eyebrow_upper_right_quarter:
                        return EyebrowColor;
                    case Landmark.LandmarkName.mouth_left_corner:
                    case Landmark.LandmarkName.mouth_lower_lip_bottom:
                    case Landmark.LandmarkName.mouth_lower_lip_left_contour1:
                    case Landmark.LandmarkName.mouth_lower_lip_left_contour2:
                    case Landmark.LandmarkName.mouth_lower_lip_left_contour3:
                    case Landmark.LandmarkName.mouth_lower_lip_right_contour1:
                    case Landmark.LandmarkName.mouth_lower_lip_right_contour2:
                    case Landmark.LandmarkName.mouth_lower_lip_right_contour3:
                    case Landmark.LandmarkName.mouth_lower_lip_top:
                    case Landmark.LandmarkName.mouth_right_corner:
                    case Landmark.LandmarkName.mouth_upper_lip_bottom:
                    case Landmark.LandmarkName.mouth_upper_lip_left_contour1:
                    case Landmark.LandmarkName.mouth_upper_lip_left_contour2:
                    case Landmark.LandmarkName.mouth_upper_lip_left_contour3:
                    case Landmark.LandmarkName.mouth_upper_lip_right_contour1:
                    case Landmark.LandmarkName.mouth_upper_lip_right_contour2:
                    case Landmark.LandmarkName.mouth_upper_lip_right_contour3:
                    case Landmark.LandmarkName.mouth_upper_lip_top:
                        return MouthColor;
                    case Landmark.LandmarkName.nose_contour_left1:
                    case Landmark.LandmarkName.nose_contour_left2:
                    case Landmark.LandmarkName.nose_contour_left3:
                    case Landmark.LandmarkName.nose_contour_lower_middle:
                    case Landmark.LandmarkName.nose_contour_right1:
                    case Landmark.LandmarkName.nose_contour_right2:
                    case Landmark.LandmarkName.nose_contour_right3:
                    case Landmark.LandmarkName.nose_left:
                    case Landmark.LandmarkName.nose_right:
                    case Landmark.LandmarkName.nose_tip:
                        return NoseColor;
                    case Landmark.LandmarkName.right_eye_bottom:
                    case Landmark.LandmarkName.right_eye_center:
                    case Landmark.LandmarkName.right_eye_left_corner:
                    case Landmark.LandmarkName.right_eye_lower_left_quarter:
                    case Landmark.LandmarkName.right_eye_lower_right_quarter:
                    case Landmark.LandmarkName.right_eye_pupil:
                    case Landmark.LandmarkName.right_eye_right_corner:
                    case Landmark.LandmarkName.right_eye_top:
                    case Landmark.LandmarkName.right_eye_upper_left_quarter:
                    case Landmark.LandmarkName.right_eye_upper_right_quarter:
                        return EyeColor;
                    case Landmark.LandmarkName.right_eyebrow_left_corner:
                    case Landmark.LandmarkName.right_eyebrow_lower_left_quarter:
                    case Landmark.LandmarkName.right_eyebrow_lower_middle:
                    case Landmark.LandmarkName.right_eyebrow_lower_right_quarter:
                    case Landmark.LandmarkName.right_eyebrow_right_corner:
                    case Landmark.LandmarkName.right_eyebrow_upper_left_quarter:
                    case Landmark.LandmarkName.right_eyebrow_upper_middle:
                    case Landmark.LandmarkName.right_eyebrow_upper_right_quarter:
                        return EyebrowColor;
                    default:
                        return DefaultColor;
                }
            }
            else
            {
                return DefaultColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Brush DefaultColor
        {
            get;
            set;
        }

        public Brush EyeColor
        {
            get;
            set;
        }

        public Brush EyebrowColor
        {
            get;
            set;
        }

        public Brush FaceContourColor
        {
            get;
            set;
        }

        public Brush NoseColor
        {
            get;
            set;
        }

        public Brush MouthColor
        {
            get;
            set;
        }
    }
}
