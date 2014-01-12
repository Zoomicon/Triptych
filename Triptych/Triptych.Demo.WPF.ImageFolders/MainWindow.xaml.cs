/*
Project: Triptych (http://triptych.codeplex.com)
Filename: MainWindow.xaml.cs
Version: 20140112
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

    const int DEVICE_ID = 0; //if you want to use an external camera with a laptop, better disable the inner camera (or uninstall its driver). Make sure you run the app as an administrator (right click, Properties, Compatibility settings, check the Run as Administrator option). Can disable UAC (User Access Control) to avoid Windows Vista+ security prompts
    const string URI_PREFIX = "http://bit.ly/GB-";
    const string IMAGE_EXTENSIONS = "jpg|png|bmp"; //GIF not supported //TODO: check if BMP are supported
    const string BASE_FOLDER = "Photos";
    const string STARTUP_FOLDER = "Startup";
    const int DELAY_CENTER = 3; //sec
    const int DELAY_SIDES = 5; //sec

    #endregion
    
    #region --- Fields ---

    string lastFolder = "";
    bool loadingImages = false;
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
      LoadImages(STARTUP_FOLDER);
      InitBrushes();
    }
    
    private void LoadImages(string folder, bool startAnimation = false)
    {
      if (loadingImages || String.Equals(folder, lastFolder, StringComparison.OrdinalIgnoreCase))
        return; //do not try to load the same image set again, looks strange (like going fast, since we may get multiple recognition events and it keeps trying to reload the same stuff)

      loadingImages = true;
      try
      {
        var imageFilenames = Directory.EnumerateFiles(Path.Combine(new String[]{System.AppDomain.CurrentDomain.BaseDirectory, BASE_FOLDER, folder})).Where(file => Regex.IsMatch(file, @"^.+\.(" + IMAGE_EXTENSIONS + ")$")).Shuffle(new Random());
        imageSources.Clear();
        foreach (string filename in imageFilenames)
          imageSources.Add(new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute)));

        lastFolder = folder;

        ShowFirstImage();
      }
      catch
      {
        //NOP //Dispatcher.BeginInvoke((Action)(() => ClearImages()));
      }
      finally
      {
        loadingImages=false;
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

    public void ShowFirstImage()
    {
      imageSourceLeftIndex = imageSourceCenterIndex = imageSourceRightIndex = 0;
      UpdateImages();
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
        //MessageBox.Show(result.BarcodeFormat.ToString());
        var parsedResult = ResultParser.parseResult(result);
        if (parsedResult != null)
        {
          //MessageBox.Show(parsedResult.DisplayResult);
          ParsedResultType resultType = parsedResult.Type;
          switch (resultType)
          {
            case ParsedResultType.URI:
              LoadImages(result.Text.TrimLeft(URI_PREFIX), startAnimation:true); //remove URI prefix to get the value
              break;
            default: //handling ParsedResultType.TEXT and all other ParsedResultType values as text
              LoadImages(result.Text, startAnimation:true);
              break;
          }
        }

      };
    }

    public void StartVision()
    {
      vision.Start(new WindowInteropHelper(this).Handle, 640, 640, 64, 64, DEVICE_ID); //(int)Width, (int)Height
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
      return Utils.CoerceIndex(index, imageSources.Count);
    }

    #endregion

    #endregion

    #region --- Events ---

    #region Timers

    private void TimerCenter_Tick(object sender, EventArgs e)
    {
      if (!loadingImages)
        NextCenter();
    }

    private void TimerSides_Tick(object sender, EventArgs e)
    {
      if (!loadingImages)
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

