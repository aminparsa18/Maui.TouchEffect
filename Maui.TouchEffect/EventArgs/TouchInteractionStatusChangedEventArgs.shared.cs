using MauiTouchEffect.Enums;

namespace MauiTouchEffect.EventArgs;

public class TouchInteractionStatusChangedEventArgs : System.EventArgs
{
    internal TouchInteractionStatusChangedEventArgs(TouchInteractionStatus touchInteractionStatus)
    {
        TouchInteractionStatus = touchInteractionStatus;
    }

    public TouchInteractionStatus TouchInteractionStatus { get; }
}