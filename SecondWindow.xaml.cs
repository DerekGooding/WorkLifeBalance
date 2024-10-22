using System.Windows.Input;

namespace WorkLifeBalance;

/// <summary>
/// Interaction logic for SecondWindow.xaml
/// </summary>
/// //use it in dependency injection, make searate method for req windows
public partial class SecondWindow : Window
{
    public SecondWindow() 
    {
        InitializeComponent();
    }

    private void MoveWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
