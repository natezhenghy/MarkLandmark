using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MarkLandmark
{
    public class Landmark : INotifyPropertyChanged
    {
        public enum LandmarkName
        {
            Unknown = -1,
            contour_chin,
            contour_left1,
            contour_left2,
            contour_left3,
            contour_left4,
            contour_left5,
            contour_left6,
            contour_left7,
            contour_left8,
            contour_left9,
            contour_right1,
            contour_right2,
            contour_right3,
            contour_right4,
            contour_right5,
            contour_right6,
            contour_right7,
            contour_right8,
            contour_right9,
            left_eye_bottom,
            left_eye_center,
            left_eye_left_corner,
            left_eye_lower_left_quarter,
            left_eye_lower_right_quarter,
            left_eye_pupil,
            left_eye_right_corner,
            left_eye_top,
            left_eye_upper_left_quarter,
            left_eye_upper_right_quarter,
            left_eyebrow_left_corner,
            left_eyebrow_lower_left_quarter,
            left_eyebrow_lower_middle,
            left_eyebrow_lower_right_quarter,
            left_eyebrow_right_corner,
            left_eyebrow_upper_left_quarter,
            left_eyebrow_upper_middle,
            left_eyebrow_upper_right_quarter,
            mouth_left_corner,
            mouth_lower_lip_bottom,
            mouth_lower_lip_left_contour1,
            mouth_lower_lip_left_contour2,
            mouth_lower_lip_left_contour3,
            mouth_lower_lip_right_contour1,
            mouth_lower_lip_right_contour2,
            mouth_lower_lip_right_contour3,
            mouth_lower_lip_top,
            mouth_right_corner,
            mouth_upper_lip_bottom,
            mouth_upper_lip_left_contour1,
            mouth_upper_lip_left_contour2,
            mouth_upper_lip_left_contour3,
            mouth_upper_lip_right_contour1,
            mouth_upper_lip_right_contour2,
            mouth_upper_lip_right_contour3,
            mouth_upper_lip_top,
            nose_contour_left1,
            nose_contour_left2,
            nose_contour_left3,
            nose_contour_lower_middle,
            nose_contour_right1,
            nose_contour_right2,
            nose_contour_right3,
            nose_left,
            nose_right,
            nose_tip,
            right_eye_bottom,
            right_eye_center,
            right_eye_left_corner,
            right_eye_lower_left_quarter,
            right_eye_lower_right_quarter,
            right_eye_pupil,
            right_eye_right_corner,
            right_eye_top,
            right_eye_upper_left_quarter,
            right_eye_upper_right_quarter,
            right_eyebrow_left_corner,
            right_eyebrow_lower_left_quarter,
            right_eyebrow_lower_middle,
            right_eyebrow_lower_right_quarter,
            right_eyebrow_right_corner,
            right_eyebrow_upper_left_quarter,
            right_eyebrow_upper_middle,
            right_eyebrow_upper_right_quarter
        }

        #region [Fields]

        private Visibility visibility = Visibility.Visible;

        private FrameworkElement visualObject;

        private Panel panel;

        private double x;

        private double y;

        private LandmarkName name;

        private bool isDirty = false;

        #endregion

        #region [Properties]

        public LandmarkName Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (value != x)
                {
                    x = value;
                    RaisePropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value != y)
                {
                    y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }

        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if (value != visibility)
                {
                    visibility = value;
                    RaisePropertyChanged("Visibility");
                }
            }
        }

        public ICommand MouseLeftButtonDownCmd
        {
            get
            {
                return new RelayCommand<MouseButtonEventArgs>(OnMouseLeftButtonDown);
            }
        }

        public ICommand LoadedCmd
        {
            get
            {
                return new RelayCommand<FrameworkElement>(OnLoaded);
            }
        }

        public FrameworkElement VisualObject
        {
            set
            {
                if (value != visualObject)
                {
                    visualObject = value;
                    RaisePropertyChanged("VisualObject");
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            private set
            {
                if(value != isDirty)
                {
                    isDirty = value;
                    RaisePropertyChanged("IsDirty");
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

        private void OnMouseMoveInPanel(object obj, MouseEventArgs e)
        {
            var pnt = e.GetPosition(this.visualObject);
            X += (pnt.X - visualObject.ActualWidth / 2);
            Y += (pnt.Y - visualObject.ActualHeight / 2);
            IsDirty = true;
        }

        private void OnMouseLeavePanel(object obj, MouseEventArgs e)
        {
            Mouse.RemoveMouseMoveHandler(this.panel, OnMouseMoveInPanel);
            Mouse.RemoveMouseLeaveHandler(this.panel, OnMouseLeavePanel);
            Mouse.RemoveMouseUpHandler(this.panel, OnMouseUpPanel);
        }

        private void OnMouseUpPanel(object obj, MouseButtonEventArgs e)
        {
            Mouse.RemoveMouseMoveHandler(this.panel, OnMouseMoveInPanel);
            Mouse.RemoveMouseLeaveHandler(this.panel, OnMouseLeavePanel);
            Mouse.RemoveMouseUpHandler(this.panel, OnMouseUpPanel);
        }
        
        private void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.AddMouseMoveHandler(this.panel, OnMouseMoveInPanel);
            Mouse.AddMouseLeaveHandler(this.panel, OnMouseLeavePanel);
            Mouse.AddMouseUpHandler(this.panel, OnMouseUpPanel);
        }

        private void OnLoaded(FrameworkElement e)
        {
            VisualObject = (FrameworkElement)e;

            DependencyObject obj = visualObject;

            while (!(obj is Grid))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            this.panel = (Panel)obj;

        }

        public override string ToString()
        {
            return X.ToString() + " " + Y.ToString();
        }

        public void ResetDirtyState()
        {
            IsDirty = false;
        }

        #endregion
    }
}
