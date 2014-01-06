/*
Project: Triptych (http://triptych.codeplex.com)
Filename: Triptych.Demo.Silverlight.Webcams\App.xaml.cs
Version: 20140106
*/

using System;
using System.Windows;
using System.Windows.Threading;

namespace Triptych.Demo.Silverlight.Webcams
{
  public partial class App : Application
  {

    #region --- Fields ---

    bool showingMessages = false; //not showing messages with modal dialogboxes, since we target exchibitions

    #endregion

    #region --- Methods ---

    public App()
    {
      this.Startup += this.Application_Startup;
      this.Exit += this.Application_Exit;
      this.UnhandledException += this.Application_UnhandledException;

      InitializeComponent();
    }

    private void UpdateOOB()
    {
      if (!IsRunningOutOfBrowser) return;

      CheckAndDownloadUpdateCompleted += new CheckAndDownloadUpdateCompletedEventHandler(OnCheckAndDownloadUpdateCompleted); //attach event handler
      try
      {
        CheckAndDownloadUpdateAsync();
      }
      catch
      {
        //Ignore any exceptions (e.g. when offline)
      }
    }

    private void InfoMessage(string msg)
    {
      if (showingMessages)
        MessageBox.Show(msg);
    }

    private void ErrorMessage(string msg, Exception err)
    {
      if (showingMessages)
        MessageBox.Show(msg + ": " + err.Message + "\n\n" + err.StackTrace);
    }

    #endregion

    #region --- Events ---

    private void Application_Startup(object sender, StartupEventArgs e)
    {
      UpdateOOB(); //TODO: run this from background thread, seems to take some time //CALLING THIS FIRST, SINCE THE REST OF THE CODE COULD THROW AN EXCEPTION WHICH WOULD BLOCK UPDATES (AND ALSO TO MAKE USE OF THE TIME TO SET UP THE APP, SINCE UPDATING OCCURS IN THE BACKGROUND)
      
      this.RootVisual = new MainPage();
    }

    private void Application_Exit(object sender, EventArgs e)
    {
      //NOP
    }

    private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
      // If the app is running outside of the debugger then report the exception using
      // the browser's exception mechanism. On IE this will display it a yellow alert 
      // icon in the status bar and Firefox will display a script error.
      //if (!System.Diagnostics.Debugger.IsAttached) //THIS DOESN'T SEEM TO WORK, FREEZES
      {

        // NOTE: This will allow the application to continue running after an exception has been thrown
        // but not handled. 
        // For production applications this error handling should be replaced with something that will 
        // report the error to the website and stop the application.
        e.Handled = true;
        Dispatcher dispatcher = Deployment.Current.Dispatcher;
        if (dispatcher!=null)
          dispatcher.BeginInvoke(
            () => {
              ErrorMessage("Unhandled exception", e.ExceptionObject);
          });
      }
    }

    private void OnCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
    {
      CheckAndDownloadUpdateCompleted -= new CheckAndDownloadUpdateCompletedEventHandler(OnCheckAndDownloadUpdateCompleted); //detach event handler

      if (e.UpdateAvailable) //update was found and downloaded
        InfoMessage("Update downloaded, will use at next launch");
      else if (e.Error != null) //error during update process
        ErrorMessage("Update failed", e.Error);
    }
    
    #endregion

  }
}
