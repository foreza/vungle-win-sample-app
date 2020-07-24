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

        private VungleAd inmobiProvider;
        private VungleAd vartyrInstance;

        public MainPage()
        {
            this.InitializeComponent();
            initializeSDK();
            registerHandlers();
        }

        public void initializeSDK()
        {
            inmobiProvider = AdFactory.GetInstance("5f1a11808ffe460001a441cf");

            try
            {
                vartyrInstance = AdFactory.GetInstance("5f1a17226694ec0001781d16");

            } catch (InvalidOperationException e)
            {
                Debug.WriteLine($"caught {e}");

            }
        }




        public void registerHandlers()
        {
            inmobiProvider.OnAdPlayableChanged += InMobiHandler_OnAdPlayableChanged;
            inmobiProvider.OnInitCompleted += InMobiHandler_OnInitCompleted;

            if (vartyrInstance != null)
            {
                vartyrInstance.OnAdPlayableChanged += VartyrInstance_OnAdPlayableChanged;
                vartyrInstance.OnInitCompleted += VartyrInstance_OnInitCompleted;
            }

        }

        private void VartyrInstance_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            Debug.WriteLine("VartyrInstance_OnInitCompleted");
        }

        private void InMobiHandler_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            Debug.WriteLine("InMobiHandler_OnInitCompleted");
            doAdRequestForInstance(inmobiProvider, "5f1a11808ffe460001a441cf");
        }

        public void doAdRequestForInstance(VungleAd instance, String placement)
        {
            instance.LoadAd(placement);
        }

        public async void playAdForInMobiInstance(String placement){
            AdConfig adConfig = new AdConfig();
            adConfig.Orientation = DisplayOrientations.Portrait;
            adConfig.SoundEnabled = false;
            await inmobiProvider.PlayAdAsync(adConfig, placement);
        }

        public async void playAdForInstanceVartyr(String placement)
        {
            AdConfig adConfig = new AdConfig();
            adConfig.Orientation = DisplayOrientations.Landscape;
            adConfig.SoundEnabled = true;
            await inmobiProvider.PlayAdAsync(adConfig, placement);
        }

        private async void InMobiHandler_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            if (e.AdPlayable == true)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  new DispatchedHandler(() => playAdForInMobiInstance(e.Placement)));
            }
        }


        private async void VartyrInstance_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            if (e.AdPlayable == true)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                  new DispatchedHandler(() => playAdForInstanceVartyr(e.Placement)));
            }
        }

        // TODO: handle disposal
    }
}
