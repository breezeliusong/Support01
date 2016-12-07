using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Html;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Support01
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
            //// Pointer event listeners.
            //TextBox.PointerReleased += touchRectangle_PointerReleased;
            //TextBox.PointerExited += touchRectangle_PointerExited;
            //TextBox.PointerPressed += touchRectangle_PointerPressed;
            // Listener for the ManipulationDelta event.
    public sealed partial class MainPage : Page
    {
        // Global translation transform used for changing the position of 
        // the TextBox based on input data from the touch contact.
        private TranslateTransform dragTranslation;
        public MainPage()
        {
            this.InitializeComponent();
            TextBox.ManipulationDelta += touchRectangle_ManipulationDelta;
            // New translation transform populated in 
            // the ManipulationDelta handler.
            dragTranslation = new TranslateTransform();
            // Apply the translation to the TextBox.
            TextBox.RenderTransform = this.dragTranslation;
            //set binding
            this.DataContext = this;
            Debug.WriteLine(MemoryManager.AppMemoryUsage);
            Debug.WriteLine(MemoryManager.AppMemoryUsageLevel);
            Debug.WriteLine(MemoryManager.AppMemoryUsageLimit);

        }

        // Handler for the ManipulationDelta event.
        // ManipulationDelta data is loaded into the
        // translation transform and applied to the TextBox.
        void touchRectangle_ManipulationDelta(object sender,
            ManipulationDeltaRoutedEventArgs e)
        {
            // Move the rectangle.
            dragTranslation.X += e.Delta.Translation.X;
            dragTranslation.Y += e.Delta.Translation.Y;
        }



        // Handler for pointer exited event.
        private void touchRectangle_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            TextBox rect = sender as TextBox;

            // Pointer moved outside Rectangle hit test area.
            // Reset the dimensions of the Rectangle.
            if (null != rect)
            {
                rect.Width = 200;
                rect.Height = 100;
            }
        }
        // Handler for pointer released event.
        private void touchRectangle_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            TextBox rect = sender as TextBox;

            // Reset the dimensions of the Rectangle.
            if (null != rect)
            {
                rect.Width = 200;
                rect.Height = 100;
            }
        }

        // Handler for pointer pressed event.
        private void touchRectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (null != textbox)
            {
                textbox.Width = 200;
                textbox.Height = 100;
            }
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    var text = await dataPackageView.GetTextAsync();
                }
                catch (Exception)
                {
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Set the content to DataPackage as html format
            string texts = text.Text;
            var dataPackage = new DataPackage();
            dataPackage.SetText(texts);

            // Set the DataPackage to clipboard.
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }

        public ICommand OpenDialogFile
        {
            get
            {
                return new DelegateCommand<RichEditBox>(OpenDialogToAttach);
            }
        }


        private async void OpenDialogToAttach(RichEditBox TweetEditBox)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile imgFile = await openPicker.PickSingleFileAsync();
            Windows.Storage.FileProperties.ImageProperties imgProperties = await imgFile.Properties.GetImagePropertiesAsync();


            using (Windows.Storage.Streams.IRandomAccessStream fileStream
                = await imgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                TweetEditBox.Document.Selection.InsertImage((int)imgProperties.Width, (int)imgProperties.Height, 0,
                    Windows.UI.Text.VerticalCharacterAlignment.Baseline, "img", fileStream);
            }
        }
    }
}
