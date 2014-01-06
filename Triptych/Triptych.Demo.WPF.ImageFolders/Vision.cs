//Version: 20140106
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

      resultPoints = new List<ResultPoint>();
    }

    private void Decode(Bitmap image, bool tryMultipleBarcodes, IList<BarcodeFormat> possibleFormats)
    {
      resultPoints.Clear();
  
      var timerStart = DateTime.Now.Ticks;
      Result[] results = null;
      barcodeReader.Options.PossibleFormats = possibleFormats;
      if (tryMultipleBarcodes)
        results = barcodeReader.DecodeMultiple(image);
      else
      {
        var result = barcodeReader.Decode(image);
        if (result != null)
        {
          results = new[] { result };
        }
      }
      var timerStop = DateTime.Now.Ticks;

      if (results == null)
      {
        //txtContent.Text = "No barcode recognized";
      }
      //labDuration.Text = new TimeSpan(timerStop - timerStart).Milliseconds.ToString("0 ms");

      if (results != null)
      {
        foreach (var result in results)
        {
          if (result.ResultPoints.Length > 0)
          {
            var rect = new Rectangle((int)result.ResultPoints[0].X, (int)result.ResultPoints[0].Y, 1, 1);
            foreach (var point in result.ResultPoints)
            {
              if (point.X < rect.Left)
                rect = new Rectangle((int)point.X, rect.Y, rect.Width + rect.X - (int)point.X, rect.Height);
              if (point.X > rect.Right)
                rect = new Rectangle(rect.X, rect.Y, rect.Width + (int)point.X - rect.X, rect.Height);
              if (point.Y < rect.Top)
                rect = new Rectangle(rect.X, (int)point.Y, rect.Width, rect.Height + rect.Y - (int)point.Y);
              if (point.Y > rect.Bottom)
                rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)point.Y - rect.Y);
            }
            /*using (var g = picBarcode.CreateGraphics())
            {
              g.DrawRectangle(Pens.Green, rect);
            }*/
          }
        }
      }
    }

    public void Start()
    {
      if (wCam == null)
      {
        wCam = new WebCam { Container = new PictureBox() };

        wCam.OpenConnection();

        webCamTimer = new Timer();
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
      if (bitmap == null)
        return;
      var reader = new BarcodeReader();
      var result = reader.Decode(bitmap);
      if (result != null)
      {
        MessageBox.Show("3 - " + result.BarcodeFormat.ToString() + " - " + result.Text);
      }
      webCamTimer.Start();
    }

 }
}
