using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttachedPropertyLeaks {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();
      }

      public static readonly DependencyProperty PopupIndexProperty = DependencyProperty.RegisterAttached("PopupIndex", typeof(long), typeof(MainWindow),
         new FrameworkPropertyMetadata(PopupIndexChanged));
      public static void SetPopupIndex(UIElement element, long value) {
         element.SetValue(PopupIndexProperty, value);
      }

      public static long GetPopupIndex(UIElement element) {
         return (long)element.GetValue(PopupIndexProperty);
      }
      private static void PopupIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
         var popup = d as Popup;
         if (popup != null) {
            popup.Opened += OnPopup;
         }
      }
      private static long MaxPopup = 0;
      private static void OnPopup(object sender, EventArgs e) {
         MaxPopup = Math.Max(MaxPopup, MainWindow.GetPopupIndex((Popup)sender));
      }

      private void AttachedPropertyTest_Click(object sender, RoutedEventArgs e) {
         for (long i = 0; i < 5 * 1000 * 1000; i++) {
            Popup p = new Popup();
            MainWindow.SetPopupIndex(p, i);
         }
      }
      private void MemoryLeakTest_Click(object sender, RoutedEventArgs e) {
         for (long i = 0; i < 5 * 1000 * 1000; i++) {
            MyPopup p = new MyPopup(this);
         }
      }
   }

   public class MyPopup : Popup {
      public MyPopup(Window parentWindow) {
         parentWindow.Closed += parentWindoClosed;
      }

      private void parentWindoClosed(object sender, EventArgs e) { }
   }

}
