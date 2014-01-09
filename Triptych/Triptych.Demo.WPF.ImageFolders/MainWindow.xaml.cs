/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Triptych.Demo.Silverlight.Webcam\MainPage.xaml.cs
Version: 20140109
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZXing.Client.Result;

namespace Triptych.Demo.WPF.ImageFolders
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    #region --- Constants ---

    const string BASE_FOLDER = "Photos";
    const string DEFAULT_FOLDER = "FruitBowl";
    const int DELAY_CENTER = 10; //sec
    const int DELAY_SIDES = 20; //sec

    #endregion
    
    #region --- Fields ---

    bool initialized = false;
    List<ImageSource> imageSources = new List<ImageSource>();
    ImageBrush imageBrushLeft, imageBrushCenter, imageBrushRight;
    int imageSourceLeftIndex, imageSourceCenterIndex, imageSourceRightIndex;
    DispatcherTimer timerCenter, timerSides;
    Vision vision = new Vision();
    
    #endregion

    #region --- Methods ---

    public MainWindow()
    {
      InitializeComponent();
      App.Current.MainWindow.SizeChanged += (s, e) =>
      {
        if (!initialized)
        {
          Init();
          Start();
        }
      };

    }
     
    public void Init()
    {
      InitVision();
      InitImages();
      InitAnimation();

      if (!initialized)
      {
        UpdateImages();
        initialized = true;
      }
    }

    #region Media


    private void InitImages()
    {
      LoadImages(DEFAULT_FOLDER);
      InitBrushes();
    }

    private void LoadImages(string folder)
    {
      try
      {
        var imageFilenames = Directory.EnumerateFiles(Path.Combine(new String[]{System.AppDomain.CurrentDomain.BaseDirectory, BASE_FOLDER, folder})).Where(file => Regex.IsMatch(file, @"^.+\.(jpg|png|bmp)$"));
        foreach (string filename in imageFilenames)
          imageSources.Add(new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute)));
      }
      catch
      {
        ClearImages();
        return;
      }
    }

    private void InitBrushes()
    {
      imageSourceLeftIndex = imageSourceRightIndex = imageSourceCenterIndex;

      imageBrushLeft = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushCenter = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushRight = imageBrushLeft; //new ImageBrush() { Stretch = Stretch.UniformToFill };

      ViewportCenter.Fill = imageBrushCenter;
      ViewportLeft.Fill = imageBrushLeft;
      ViewportRight.Fill = imageBrushRight;
    }

    public void UpdateImages()
    {
      try
      {
        imageBrushLeft.ImageSource = imageSources[imageSourceLeftIndex];
        imageBrushCenter.ImageSource = imageSources[imageSourceCenterIndex];
        imageBrushRight.ImageSource = imageSources[imageSourceRightIndex];
      }
      catch
      {
        //NOP
      }
    }

    public void ClearImages()
    {
      try
      {
        ViewportLeft.Fill = new SolidColorBrush(Colors.Red);
        ViewportCenter.Fill = new SolidColorBrush(Colors.Green);
        ViewportRight.Fill = new SolidColorBrush(Colors.Blue);
      }
      catch
      {
        //NOP
      }
    }

    #endregion

    #region Animation

    private void InitAnimation()
    {
      timerCenter = new System.Windows.Threading.DispatcherTimer();
      timerCenter.Tick += new EventHandler(TimerCenter_Tick);
      timerCenter.Interval = new TimeSpan(0, 0, DELAY_CENTER);

      timerSides = new System.Windows.Threading.DispatcherTimer();
      timerSides.Tick += new EventHandler(TimerSides_Tick);
      timerSides.Interval = new TimeSpan(0, 0, DELAY_SIDES);
    }

    private void StartAnimation()
    {
      timerCenter.Start();
      timerSides.Start();
    }

    private void StopAnimation()
    {
      timerCenter.Stop();
      timerSides.Stop();
    }

    public void PreviousLeft()
    {
      imageSourceLeftIndex = CoerceIndex(imageSourceLeftIndex - 1);
      UpdateImages();
    }

    public void NextLeft()
    {
      imageSourceLeftIndex = CoerceIndex(imageSourceLeftIndex + 1);
      UpdateImages();
    }

    public void PreviousCenter()
    {
      imageSourceCenterIndex = CoerceIndex(imageSourceCenterIndex - 1);
      UpdateImages();
    }

    public void NextCenter()
    {
      imageSourceCenterIndex = CoerceIndex(imageSourceCenterIndex + 1);
      UpdateImages();
    }

    public void CenterToLeftRight()
    {
      imageSourceRightIndex = imageSourceLeftIndex = imageSourceCenterIndex;
      UpdateImages();
    }

    public void PreviousRight()
    {
      imageSourceRightIndex = CoerceIndex(imageSourceRightIndex - 1);
      UpdateImages();
    }

    public void NextRight()
    {
      imageSourceRightIndex = CoerceIndex(imageSourceRightIndex + 1);
      UpdateImages();
    }
    
    public void NextLeftRight()
    {
      imageSourceLeftIndex = CoerceIndex(imageSourceLeftIndex + 1);
      imageSourceRightIndex = CoerceIndex(imageSourceRightIndex + 1);
      UpdateImages();
    }

    #endregion

    #region Vision

    public void InitVision()
    {
      vision.Recognized += result =>
      {
        /*
        MessageBox.Show("1 - " + result.BarcodeFormat.ToString() + " - " + result.Text);
        var parsedResult = ResultParser.parseResult(result);
        if (parsedResult != null)
          MessageBox.Show("2 - Parsed result:" + parsedResult.DisplayResult);
        */
        LoadImages(result.Text); //TODO: get last part of URL here instead (e.g. using tinyurl.com with names)
      };
    }

    public void StartVision()
    {
      vision.Start(new WindowInteropHelper(this).Handle, 64, 48); //(int)Width, (int)Height
    }

    public void StopVision()
    {
      vision.Stop();
    }

    #endregion

    public void Start()
    {
      UpdateImages();
      StartAnimation();
      StartVision();
    }

    public void Stop()
    {
      StopAnimation();
      StopVision();
      ClearImages();
    }

    #region Helpers

    public int CoerceIndex(int index)
    {
      return CoerceIndex(index, imageSources.Count);
    }

    public static int CoerceIndex(int index, int count)
    {
      if (index < 0)
        return count - 1;
      else if (index >= count)
        return 0;
      else
        return index;
    }

    #endregion

    #endregion

    #region --- Events ---

    #region Timers

    private void TimerCenter_Tick(object sender, EventArgs e)
    {
      NextCenter();
    }

    private void TimerSides_Tick(object sender, EventArgs e)
    {
      NextLeftRight(); //do in a single step
    }
    
    #endregion

    #region Mouse

    private void ViewportLeft_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      NextLeft();
    }

    private void ViewportLeft_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      PreviousLeft();
    }

    private void ViewportCenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      NextCenter();
    }

    private void ViewportCenter_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      PreviousCenter();
    }

    private void ViewportCenter_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      CenterToLeftRight();
    }

    private void ViewportRight_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      NextRight();
    }

    private void ViewportRight_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      PreviousRight();
    }

    #endregion

    #endregion

  }
}

