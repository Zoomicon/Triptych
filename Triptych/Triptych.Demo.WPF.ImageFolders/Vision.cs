//Version: 20140109
//Based on ZXing.net's WindowsFormsDemo

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ZXing;
using ZXing.Common;

namespace Triptych.Demo.WPF.ImageFolders
{
  public class Vision
  {

    #region --- Fields ---

    private WebCam wCam;
    private Timer visionTimer;
    private readonly BarcodeReader barcodeReader;
    private readonly IList<ResultPoint> resultPoints;
    private EncodingOptions EncodingOptions { get; set; }
    private bool TryMultipleBarcodes { get; set; }

    #endregion

    #region --- Methods ---

    public Vision()
    {
      resultPoints = new List<ResultPoint>();

      barcodeReader = new BarcodeReader
      {
        AutoRotate = true,
        TryInverted = true,
        Options = new DecodingOptions { TryHarder = true }
      };
      
      /*
      barcodeReader.ResultPointFound += point =>
      {
        if (point == null)
          resultPoints.Clear();
        else
          resultPoints.Add(point);
      };

      barcodeReader.ResultFound += result =>
      {
        if (Recognized != null)
          Recognized(result);
      };
      */
    }

    public void Start(IntPtr parentHandle, int width, int height)
    {
      if (wCam == null)
      {
        wCam = new WebCam();

        wCam.OpenConnection(parentHandle, width, height);

        visionTimer = new Timer(); //Note: for WPF maybe should use a DispatchTimer instead
        visionTimer.Tick += visionTimer_Tick;
        visionTimer.Interval = 200;
        visionTimer.Start();
      }
      else
        Stop();
    }

    public void Stop()
    {
      visionTimer.Stop();
      visionTimer = null;
      wCam.Dispose();
      wCam = null;
    }

    #endregion

    #region --- Events ---

    public event Action<ZXing.Result> Recognized;

    void visionTimer_Tick(object sender, EventArgs e)
    {
      if (Recognized != null)
      {
        visionTimer.Stop();
        var bitmap = wCam.GetCurrentImage();
        if (bitmap == null) return;

        var reader = new BarcodeReader();
        var result = reader.Decode(bitmap);
        if (result!=null)
          Recognized(result);

        visionTimer.Start();
      }
    }

    #endregion

  }
}
