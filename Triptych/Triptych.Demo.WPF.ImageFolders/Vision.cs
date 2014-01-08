//Version: 20140108
//Based on ZXing.net's WindowsFormsDemo

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ZXing;
using ZXing.Client.Result;
using ZXing.Common;

namespace Triptych.Demo.WPF.ImageFolders
{
  public class Vision
  {
    private WebCam wCam;
    private Timer webCamTimer;
    private readonly BarcodeReader barcodeReader;
    private readonly IList<ResultPoint> resultPoints;
    private EncodingOptions EncodingOptions { get; set; }
    private bool TryMultipleBarcodes { get; set; }

    public Vision()
    {
      resultPoints = new List<ResultPoint>();

      barcodeReader = new BarcodeReader
      {
        AutoRotate = true,
        TryInverted = true,
        Options = new DecodingOptions { TryHarder = true}
      };

      barcodeReader.ResultPointFound += point =>
      {
        if (point == null)
          resultPoints.Clear();
        else
          resultPoints.Add(point);
      };

      barcodeReader.ResultFound += result =>
      {
        MessageBox.Show("1 - " + result.BarcodeFormat.ToString() + " - " + result.Text);
        var parsedResult = ResultParser.parseResult(result);
        if (parsedResult != null)
          MessageBox.Show("2 - Parsed result:" + parsedResult.DisplayResult);
      };
    }
    
    public void Start(IntPtr parentHandle, int width, int height)
    {
      if (wCam == null)
      {
        wCam = new WebCam();

        wCam.OpenConnection(parentHandle, width, height);

        webCamTimer = new Timer(); //Note: for WPF maybe should use a DispatchTimer instead
        webCamTimer.Tick += webCamTimer_Tick;
        webCamTimer.Interval = 200;
        webCamTimer.Start();
      }
      else
        Stop();
    }

    public void Stop()
    {
      webCamTimer.Stop();
      webCamTimer = null;
      wCam.Dispose();
      wCam = null;
    }

    void webCamTimer_Tick(object sender, EventArgs e)
    {
      webCamTimer.Stop();
      var bitmap = wCam.GetCurrentImage();
      if (bitmap == null) return;

      var reader = new BarcodeReader();
      var result = reader.Decode(bitmap);
      if (result != null)
        MessageBox.Show("3 - " + result.BarcodeFormat.ToString() + " - " + result.Text); //TODO: raise some event here or call some callback proc

      webCamTimer.Start();
    }

 }
}
