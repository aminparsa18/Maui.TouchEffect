namespace MauiTouchEffect.EventArgs;

public class TouchCompletedEventArgs : System.EventArgs
{
    internal TouchCompletedEventArgs(object parameter)
    {
        Parameter = parameter;
    }

    public object Parameter { get; }
}