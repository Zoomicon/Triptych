/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Vision.cs
Version: 20140113
*/

//Based on ZXing.net's WindowsFormsDemo

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;

using ZXing;
using ZXing.Common;

namespace Triptych.Demo.WPF.ImageFolders
{
  public class Vision
  {

    #region --- Fields ---

    private WebCam wCam;
    //private DispatcherTimer visionTimer; //avoid using a Timer with WPF
    private Timer visionTimer; //if we use a DispatchTimer it gets in same queue as animation timers and may not respond fast enough
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

    public void Start(IntPtr parentHandle, int captureWidth, int captureHeight, int posx, int posy, int displayWidth, int displayHeight, int deviceID = 0)
    {
      if (wCam == null)
      {
        wCam = new WebCam();

        wCam.OpenConnection(parentHandle, captureWidth, captureHeight, posx, posy, displayWidth, displayHeight, deviceID);

        //visionTimer = new DispatcherTimer(); //Note: for WPF maybe should use a DispatchTimer instead
        //visionTimer.Interval = new TimeSpan(0,0,0,0,200); //200 msec

        visionTimer = new Timer();
        visionTimer.Interval = 200; //msec
        
        visionTimer.Tick += visionTimer_Tick;
        visionTimer.Start();
      }
      else
        Stop();
    }

    public void Stop()
    {
      if (visionTimer != null)
      {
        visionTimer.Stop();
        visionTimer = null;
      }
      if (wCam != null)
      {
        wCam.Dispose();
        wCam = null;
      }
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
