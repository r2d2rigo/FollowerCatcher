using CommonDX;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace FollowerCatcher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        DeviceManager deviceManager;
        SwapChainBackgroundPanelTarget target;
        Scene scene;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content == null)
            {
                deviceManager = new DeviceManager();

                scene = new Scene();

                // Place the frame in the current Window and ensure that it is active
                var swapchainPanel = new MainPage();
                swapchainPanel.SetScene(scene);
                Window.Current.Content = swapchainPanel;
                Window.Current.Activate();

                // Use CoreWindowTarget as the rendering target (Initialize SwapChain, RenderTargetView, DepthStencilView, BitmapTarget)
                target = new SwapChainBackgroundPanelTarget(swapchainPanel);

                // Add Initializer to device manager
                deviceManager.OnInitialize += target.Initialize;
                deviceManager.OnInitialize += scene.Initialize;

                // Render the cube within the CoreWindow
                target.OnRender += scene.Render;

                // Initialize the device manager and all registered deviceManager.OnInitialize 
                deviceManager.Initialize(DisplayProperties.LogicalDpi);

                // Setup rendering callback
                CompositionTarget.Rendering += CompositionTarget_Rendering;

                // Callback on DpiChanged
                DisplayProperties.LogicalDpiChanged += DisplayProperties_LogicalDpiChanged;


                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        void DisplayProperties_LogicalDpiChanged(object sender)
        {
            deviceManager.Dpi = DisplayProperties.LogicalDpi;
        }

        void CompositionTarget_Rendering(object sender, object e)
        {
            target.RenderAll();
            target.Present();
        }
    }
}
