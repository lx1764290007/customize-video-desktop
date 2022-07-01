using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;//necessary

namespace MediaIndexMark
{
    class Mark : DependencyObject
    {
        public static int GetMarkIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(VCIndex);
        }

        public static void SetMarkIndex(DependencyObject obj, int value)
        {
            obj.SetValue(VCIndex, value);
        }

        public static readonly DependencyProperty VCIndex =
            DependencyProperty.RegisterAttached("VCIndex", typeof(int), typeof(Mark), new PropertyMetadata(0, OnIndexChanged));

        private static void OnIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as UIElement;
            if (element != null)
            {
                //element.RenderTransformOrigin = new Point(0.5, 0.5);
                //element.RenderTransform = new RotateTransform((double)e.NewValue);
            }
        }
    }
}