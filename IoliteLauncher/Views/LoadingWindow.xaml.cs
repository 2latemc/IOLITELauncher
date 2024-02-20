using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualBasic.CompilerServices;
using Utils = IoLiteLauncher.Utils.Utils;

namespace IoliteLauncher.Views;

public partial class LoadingWindow : Window {
    //we need an artificial delay to give the load operation time
    public Action LoadingComplete;

    public LoadingWindow() {
        InitializeComponent();
    }

    public void ShowLoadingWindow(Action loadingComplete, int delayInSeconds) {
        Show();
        LoadingComplete = loadingComplete;
        Task wait = Utils.WaitTask(delayInSeconds);
        wait.GetAwaiter().OnCompleted(() => {
            LoadingComplete?.Invoke();
            Close();
        });
    }
}