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
using VungleSDK;
using Windows.UI.Core;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Vungle_Exploratory
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private VungleAd vungleProvider;
        private VungleAd inmobiProvider;

        String appID = "5f1a11808ffe460001a441cf";
        String plcA = "LANDING-0232123";        // Gets automatically loaded
        String plcB = "INFO_PAGE-7076639";


        Button playA;
        Button playB;


        public MainPage()
        {
            this.InitializeComponent();
            configureUI();
            initializeSDK();
            registerHandlers();
        }

        public void configureUI()
        {

            playA = FindName("PlayPlcA") as Button;
            playB = FindName("PlayPlcB") as Button;
            playA.IsEnabled = playB.IsEnabled = false;
        }

        public async void togglePlayButtonStatusForPlacement(string placement, Boolean enabled)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {

                 if (placement == plcA)
                 {
                     playA.IsEnabled = enabled;

                 }

                 if (placement == plcB)
                 {
                     playB.IsEnabled = enabled;
                 }
             });
        }


        public void initializeSDK() {
            vungleProvider = AdFactory.GetInstance(appID); // From a core integration
            inmobiProvider = AdFactory.GetInstance(appID); // Simulate the handler that InMobi will reflect and attach itself to
        }


        public void registerHandlers()
        {
            inmobiProvider.OnAdPlayableChanged += InMobiHandler_OnAdPlayableChanged;
            inmobiProvider.OnInitCompleted += InMobiHandler_OnInitCompleted;
            inmobiProvider.OnAdEnd += InmobiProvider_OnAdEnd;
        }


        private void InmobiProvider_OnAdEnd(object sender, AdEndEventArgs e)
        {
            Debug.WriteLine($"InmobiProvider_OnAdEnd for {e.Placement}");
            togglePlayButtonStatusForPlacement(e.Placement, false);
        }


        private void InMobiHandler_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            // Ad request to the instance (plcA) is triggered by Vungle on preload

            var initString = $"InMobiHandler_OnInitCompleted, InMobi Handler Init: {e.Initialized}";

            if (e.Initialized == true)
            {
                for (var i = 0; i < e.Placements.Length; ++i)
                {
                    initString += $"\n\t PLC: {e.Placements[i].ReferenceId}" +
                        $" isAutoCached: {e.Placements[i].IsAutoCached}";
                }
            }

            Debug.WriteLine(initString);
        }


        private void LoadPlacementA(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Loading Placement A: {plcA}");
            inmobiProvider.LoadAd(plcA);
        }

        private void LoadPlacementB(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Loading Placement B: {plcB}");
            inmobiProvider.LoadAd(plcB);
        }

        private async void playAdForPlacementAsync(string placement)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
new DispatchedHandler(() => playAdForInMobiInstance(placement)));
        }

        private void PlayPlacementA(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Playing Placement A: {plcA}");
            playAdForPlacementAsync(plcA);

        }

        private void PlayPlacementB(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Playing Placement B: {plcB}");
            playAdForPlacementAsync(plcB);
        }


        public async void playAdForInMobiInstance(String placement)
        {
            AdConfig adConfig = new AdConfig();
            adConfig.Orientation = DisplayOrientations.Landscape;
            adConfig.SoundEnabled = true;
            await inmobiProvider.PlayAdAsync(adConfig, placement);
        }

        private void InMobiHandler_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            if (e.AdPlayable == true)
            {
                Debug.WriteLine($"Ad loaded for placement {e.Placement}");
                togglePlayButtonStatusForPlacement(e.Placement, true);
            }

        }


        // TODO: handle disposal
    }
}
