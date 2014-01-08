/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Triptych.Demo.Silverlight.Webcam\MainPage.xaml.cs
Version: 20140109
*/

using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

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
        Clear();
        return;
      }

      imageSourceLeftIndex = 0 % imageSources.Count;
      imageSourceCenterIndex = 1 % imageSources.Count;
      imageSourceRightIndex = 2 % imageSources.Count;

      imageBrushLeft = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushCenter = new ImageBrush() { Stretch = Stretch.UniformToFill };
      imageBrushRight = new ImageBrush() { Stretch = Stretch.UniformToFill };

      Vision v = new Vision();
      v.Start(new WindowInteropHelper(this).Handle, 100, 100); //(int)Width, (int)Height

      if (!initialized)
      {
        Update();
        initialized = true;
      }
    }

    public void Update()
    {
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

    public void Clear()
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
    
    public void PreviousLeft()
    {
      imageSourceLeftIndex = CoerceIndex(imageSourceLeftIndex - 1);
      Update();
    }

    public void NextLeft()
    {
      imageSourceLeftIndex = CoerceIndex(imageSourceLeftIndex + 1);
      Update();
    }

    public void PreviousCenter()
    {
      imageSourceCenterIndex = CoerceIndex(imageSourceCenterIndex - 1);
      Update();
    }

    public void NextCenter()
    {
      imageSourceCenterIndex = CoerceIndex(imageSourceCenterIndex + 1);
      Update();
    }

    public void CenterToLeftRight()
    {
      imageSourceRightIndex = imageSourceLeftIndex = imageSourceCenterIndex;
      Update();
    }
    
    public void PreviousRight()
    {
      imageSourceRightIndex = CoerceIndex(imageSourceRightIndex - 1);
      Update();
    }

    public void NextRight()
    {
      imageSourceRightIndex = CoerceIndex(imageSourceRightIndex + 1);
      Update();
    }

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

    #region --- Events ---

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
 
  }
}

