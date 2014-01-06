/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Triptych.Demo.Silverlight.Webcams\MainPage.xaml.cs
Version: 20140106
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Triptych.Demo.Silverlight.Webcams
{
  /// <summary>
  /// MainPage of the Triptych Demo
  /// </summary>
  public partial class MainPage : UserControl
  {
    #region --- Fields ---

    bool initialized = false;
    List<CaptureSource> captureSources = new List<CaptureSource>();
    VideoBrush videoBrushLeft, videoBrushCenter, videoBrushRight;
    int captureSourceLeftIndex, captureSourceCenterIndex, captureSourceRightIndex;
  
    #endregion

    #region --- Methods ---

    public MainPage()
    {
      InitializeComponent();
      App.Current.Host.Content.Resized += (s, e) =>
      {
        /* //Not needed
        Width = App.Current.Host.Content.ActualWidth;
        Height = App.Current.Host.Content.ActualHeight;
        */
        Init();
      };
    }
    
    public void Init()
    {
      // Initialize the webcams
      ReadOnlyCollection<VideoCaptureDevice> videoCams = CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices();
      try
      {
        foreach (VideoCaptureDevice videoCam in videoCams)
          captureSources.Add(new CaptureSource() { VideoCaptureDevice = videoCam });

        captureSourceLeftIndex = 0 % captureSources.Count;
        captureSourceCenterIndex = 1 % captureSources.Count;
        captureSourceRightIndex = 2 % captureSources.Count;
      }
      catch {
        Stop();
        return;
      }

      videoBrushLeft = new VideoBrush();
      videoBrushCenter = new VideoBrush();
      videoBrushRight = new VideoBrush();

      if (!initialized)
      {
        Start();
        initialized = true;
      }
    }

    public void Start() {
      try
      {
        // Request webcam access and start the capturing
        if (CaptureDeviceConfiguration.RequestDeviceAccess())
        {
          videoBrushLeft.SetSource(captureSources[captureSourceLeftIndex]);
          captureSources[captureSourceLeftIndex].Start();
          ViewportLeft.Fill = videoBrushLeft;
 
          videoBrushCenter.SetSource(captureSources[captureSourceCenterIndex]);
          captureSources[captureSourceCenterIndex].Start();
          ViewportCenter.Fill = videoBrushCenter;
          
          videoBrushRight.SetSource(captureSources[captureSourceRightIndex]);
          captureSources[captureSourceRightIndex].Start();
          ViewportRight.Fill = videoBrushRight;
        }
      }
      catch {
        //NOP
      }
    }

    public void Stop()
    {
      ViewportLeft.Fill = new SolidColorBrush(Colors.Red);
      ViewportCenter.Fill = new SolidColorBrush(Colors.Green);
      ViewportRight.Fill = new SolidColorBrush(Colors.Blue);

      try
      {
        captureSources[captureSourceLeftIndex].Stop();
        captureSources[captureSourceCenterIndex].Stop();
        captureSources[captureSourceRightIndex].Stop();
      }
      catch
      {
        //NOP
      }
    }

    public void Restart()
    {
      Stop();
      Start();
    }

    #endregion

    #region --- Events ---

    private void ViewportLeft_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = captureSources.Count;
      if (count > 0) 
        captureSourceLeftIndex = (captureSourceLeftIndex + 1) % count;
      Restart();
    }

    private void ViewportCenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = captureSources.Count;
      if (count > 0)
        captureSourceCenterIndex = (captureSourceCenterIndex + 1) % count;
      Restart();
    }

    private void ViewportRight_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = captureSources.Count;
      if (count > 0)
        captureSourceRightIndex = (captureSourceRightIndex + 1) % count;
      Restart();
    }

    #endregion
 
  }
}
