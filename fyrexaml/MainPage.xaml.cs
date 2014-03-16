using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using fyrexaml.Services;

namespace fyrexaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FyreVMService gameService = null;
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            gameService.SendCommand(CommandLine.Text);
            CommandLine.Text = "";
            UpdateDisplay();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FyreVMService.ViewModelLocator = new Common.WinRTDispatcher();

            gameService = new FyreVMService();

            await gameService.LoadGame(this);

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            this.MainChannel.Text = gameService.ChannelData["MAIN"];
        }
    }
}
