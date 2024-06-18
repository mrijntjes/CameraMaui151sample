using Camera.MAUI;
using Camera.MAUI.ZXing;

namespace CameraMaui151sample
{
    public partial class MainPage : ContentPage
    {
        bool playing = false;

        public MainPage()
        {
            InitializeComponent();

            cameraView.CamerasLoaded += CameraView_CamerasLoaded;

            // barcode detection
            cameraView.BarcodeDetected += CameraView_BarcodeDetected;
            cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
            cameraView.BarCodeOptions = new BarcodeDecodeOptions
            {
                AutoRotate = true,
                PossibleFormats = { BarcodeFormat.QR_CODE },
                ReadMultipleCodes = false,
                TryHarder = true,
                TryInverted = true
            };
            cameraView.BarCodeDetectionFrameRate = 10;
            cameraView.BarCodeDetectionMaxThreads = 5;
            cameraView.ControlBarcodeResultDuplicate = true;
            cameraView.BarCodeDetectionEnabled = true;
        }
        private async void CameraView_CamerasLoaded(object? sender, EventArgs e)
        {
            await Task.Delay(1000);
            if (cameraView.NumCamerasDetected > 0)
            {
                controlButton.IsEnabled = true;
            }
            else
                await DisplayAlert("Warning", "No cameras detected.", "OK");

        }

        private async void StartStopCamera()
        {
            if (playing)
            {
                if (await cameraView.StopCameraAsync() == CameraResult.Success)
                {
                    playing = false;
                    controlButton.Text = "Start";
                }
            }
            else
            {
                if (cameraView.NumCamerasDetected > 0)
                {
                    //if (cameraView.NumMicrophonesDetected > 0)
                    //   cameraView.Microphone = cameraView.Microphones.First();
                    cameraView.Camera = cameraView.Cameras.First();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (await cameraView.StartCameraAsync() == CameraResult.Success)
                        {
                            controlButton.Text = "Stop";
                            playing = true;
                        }
                    });
                }
                else
                    await DisplayAlert("Warning", "No cameras detected.", "OK");
            }
        }

        private void controlButton_Clicked(object sender, EventArgs e)
        {
            StartStopCamera();
        }

        private void CameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                barcodeResult.Text = string.Format("BarcodeText on {0:HH:mm:ss}: {1}", DateTime.Now, args.Result[0].Text);
            });
        }
    }
}