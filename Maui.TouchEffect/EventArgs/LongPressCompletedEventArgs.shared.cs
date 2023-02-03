namespace MauiTouchEffect.EventArgs;

public class LongPressCompletedEventArgs : System.EventArgs
{
    internal LongPressCompletedEventArgs(object parameter)
    {
        Parameter = parameter;
    }

    public object Parameter { get; }
}