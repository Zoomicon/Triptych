/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Triptych.Demo.Silverlight.Webcam\MainPage.xaml.cs
Version: 20140102
*/

using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Triptych.Demo.WPF.ImageFolders
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
  
    #region --- Fields ---

    bool initialized = false;
    List<ImageSource> imageSources = new List<ImageSource>();
    ImageBrush imageBrushLeft, imageBrushCenter, imageBrushRight;
    int imageSourceLeftIndex, imageSourceCenterIndex, imageSourceRightIndex;
  
    #endregion

    #region --- Methods ---

    public MainWindow()
    {
      InitializeComponent();
      App.Current.MainWindow.SizeChanged += (s, e) =>
      {
        Init();
      };
    }
    
    public void Init()
    {
      try
      {
        var imageFilenames = Directory.EnumerateFiles(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,"images")).Where(file => Regex.IsMatch(file, @"^.+\.(jpg|png|bmp)$"));
        foreach (string filename in imageFilenames)
          imageSources.Add(new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute)));
      }
      catch {
        Stop();
        return;
      }

      imageSourceLeftIndex = 0 % imageSources.Count;
      imageSourceCenterIndex = 1 % imageSources.Count;
      imageSourceRightIndex = 2 % imageSources.Count;

      imageBrushLeft = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushCenter = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushRight = new ImageBrush() { Stretch = Stretch.UniformToFill };

      Vision v = new Vision();
      v.Start();

      if (!initialized)
      {
        Start();
        initialized = true;
      }
    }

    public void Start() {
      try
      {
        imageBrushLeft.ImageSource = imageSources[imageSourceLeftIndex];
        ViewportLeft.Fill = imageBrushLeft;

        imageBrushCenter.ImageSource = imageSources[imageSourceCenterIndex];
        ViewportCenter.Fill = imageBrushCenter;

        imageBrushRight.ImageSource = imageSources[imageSourceRightIndex];
        ViewportRight.Fill = imageBrushRight;
      }
      catch {
        //NOP
      }
    }

    public void Stop()
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

    public void Restart()
    {
      Stop();
      Start();
    }

    #endregion

    #region --- Events ---

    private void ViewportLeft_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = imageSources.Count;
      if (count > 0) 
        imageSourceLeftIndex = (imageSourceLeftIndex + 1) % count;
      Restart();
    }

    private void ViewportCenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = imageSources.Count;
      if (count > 0)
        imageSourceCenterIndex = (imageSourceCenterIndex + 1) % count;
      Restart();
    }

    private void ViewportRight_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      int count = imageSources.Count;
      if (count > 0)
        imageSourceRightIndex = (imageSourceRightIndex + 1) % count;
      Restart();
    }

    #endregion
 
  }
}

