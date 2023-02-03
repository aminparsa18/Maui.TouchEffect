﻿using MauiTouchEffect.Enums;
using MauiTouchEffect.EventArgs;
using MauiTouchEffect.Extentions;
using System.Windows.Input;
using Mcolor = Microsoft.Maui.Graphics.Color;
using Mview = Microsoft.Maui.Controls.View;
#if __ANDROID__
using MauiTouchEffect.Platforms.Android;
#endif

namespace MauiTouchEffect;

public class TouchEffect : RoutingEffect
{
    public const string UnpressedVisualState = "Unpressed";

    public const string PressedVisualState = "Pressed";

    public const string HoveredVisualState = "Hovered";

    public event EventHandler<TouchStatusChangedEventArgs> StatusChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<TouchStateChangedEventArgs> StateChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<TouchInteractionStatusChangedEventArgs> InteractionStatusChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<HoverStatusChangedEventArgs> HoverStatusChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<HoverStateChangedEventArgs> HoverStateChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<TouchCompletedEventArgs> Completed
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public event EventHandler<LongPressCompletedEventArgs> LongPressCompleted
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    public static readonly BindableProperty IsAvailableProperty = BindableProperty.CreateAttached(
        nameof(IsAvailable),
        typeof(bool),
        typeof(TouchEffect),
        true,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty ShouldMakeChildrenInputTransparentProperty = BindableProperty.CreateAttached(
        nameof(ShouldMakeChildrenInputTransparent),
        typeof(bool),
        typeof(TouchEffect),
        true,
        propertyChanged: SetChildrenInputTransparentAndTryGenerateEffect);

    public static readonly BindableProperty CommandProperty = BindableProperty.CreateAttached(
        nameof(Command),
        typeof(ICommand),
        typeof(TouchEffect),
        default(ICommand),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty LongPressCommandProperty = BindableProperty.CreateAttached(
        nameof(LongPressCommand),
        typeof(ICommand),
        typeof(TouchEffect),
        default(ICommand),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
        nameof(CommandParameter),
        typeof(object),
        typeof(TouchEffect),
        default,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty LongPressCommandParameterProperty = BindableProperty.CreateAttached(
        nameof(LongPressCommandParameter),
        typeof(object),
        typeof(TouchEffect),
        default,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty LongPressDurationProperty = BindableProperty.CreateAttached(
        nameof(LongPressDuration),
        typeof(int),
        typeof(TouchEffect),
        500,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty StatusProperty = BindableProperty.CreateAttached(
        nameof(Status),
        typeof(TouchStatus),
        typeof(TouchEffect),
        TouchStatus.Completed,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty StateProperty = BindableProperty.CreateAttached(
        nameof(State),
        typeof(TouchState),
        typeof(TouchEffect),
        TouchState.Normal,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty InteractionStatusProperty = BindableProperty.CreateAttached(
        nameof(InteractionStatus),
        typeof(TouchInteractionStatus),
        typeof(TouchEffect),
        TouchInteractionStatus.Completed,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty HoverStatusProperty = BindableProperty.CreateAttached(
        nameof(HoverStatus),
        typeof(HoverStatus),
        typeof(TouchEffect),
        HoverStatus.Exited,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty HoverStateProperty = BindableProperty.CreateAttached(
        nameof(HoverState),
        typeof(HoverState),
        typeof(TouchEffect),
        HoverState.Normal,
        BindingMode.OneWayToSource);

    public static readonly BindableProperty NormalBackgroundColorProperty = BindableProperty.CreateAttached(
        nameof(NormalBackgroundColor),
        typeof(Color),
        typeof(TouchEffect),
        Colors.Transparent,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredBackgroundColorProperty = BindableProperty.CreateAttached(
        nameof(HoveredBackgroundColor),
        typeof(Color),
        typeof(TouchEffect),
        Colors.Transparent,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedBackgroundColorProperty = BindableProperty.CreateAttached(
        nameof(PressedBackgroundColor),
        typeof(Color),
        typeof(TouchEffect),
        Colors.Transparent,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalOpacityProperty = BindableProperty.CreateAttached(
        nameof(NormalOpacity),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredOpacityProperty = BindableProperty.CreateAttached(
        nameof(HoveredOpacity),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedOpacityProperty = BindableProperty.CreateAttached(
        nameof(PressedOpacity),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalScaleProperty = BindableProperty.CreateAttached(
        nameof(NormalScale),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredScaleProperty = BindableProperty.CreateAttached(
        nameof(HoveredScale),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedScaleProperty = BindableProperty.CreateAttached(
        nameof(PressedScale),
        typeof(double),
        typeof(TouchEffect),
        1.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalTranslationXProperty = BindableProperty.CreateAttached(
        nameof(NormalTranslationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredTranslationXProperty = BindableProperty.CreateAttached(
        nameof(HoveredTranslationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedTranslationXProperty = BindableProperty.CreateAttached(
        nameof(PressedTranslationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalTranslationYProperty = BindableProperty.CreateAttached(
        nameof(NormalTranslationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredTranslationYProperty = BindableProperty.CreateAttached(
        nameof(HoveredTranslationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedTranslationYProperty = BindableProperty.CreateAttached(
        nameof(PressedTranslationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalRotationProperty = BindableProperty.CreateAttached(
        nameof(NormalRotation),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredRotationProperty = BindableProperty.CreateAttached(
        nameof(HoveredRotation),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedRotationProperty = BindableProperty.CreateAttached(
        nameof(PressedRotation),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalRotationXProperty = BindableProperty.CreateAttached(
        nameof(NormalRotationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredRotationXProperty = BindableProperty.CreateAttached(
        nameof(HoveredRotationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedRotationXProperty = BindableProperty.CreateAttached(
        nameof(PressedRotationX),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalRotationYProperty = BindableProperty.CreateAttached(
        nameof(NormalRotationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredRotationYProperty = BindableProperty.CreateAttached(
        nameof(HoveredRotationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedRotationYProperty = BindableProperty.CreateAttached(
        nameof(PressedRotationY),
        typeof(double),
        typeof(TouchEffect),
        0.0,
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.CreateAttached(
        nameof(AnimationDuration),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty AnimationEasingProperty = BindableProperty.CreateAttached(
        nameof(AnimationEasing),
        typeof(Easing),
        typeof(TouchEffect),
        null,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty PressedAnimationDurationProperty = BindableProperty.CreateAttached(
        nameof(PressedAnimationDuration),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty PressedAnimationEasingProperty = BindableProperty.CreateAttached(
        nameof(PressedAnimationEasing),
        typeof(Easing),
        typeof(TouchEffect),
        null,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NormalAnimationDurationProperty = BindableProperty.CreateAttached(
        nameof(NormalAnimationDuration),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NormalAnimationEasingProperty = BindableProperty.CreateAttached(
        nameof(NormalAnimationEasing),
        typeof(Easing),
        typeof(TouchEffect),
        null,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty HoveredAnimationDurationProperty = BindableProperty.CreateAttached(
        nameof(HoveredAnimationDuration),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty HoveredAnimationEasingProperty = BindableProperty.CreateAttached(
        nameof(HoveredAnimationEasing),
        typeof(Easing),
        typeof(TouchEffect),
        null,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty PulseCountProperty = BindableProperty.CreateAttached(
        nameof(PulseCount),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty IsToggledProperty = BindableProperty.CreateAttached(
        nameof(IsToggled),
        typeof(bool?),
        typeof(TouchEffect),
        default(bool?),
        BindingMode.TwoWay,
        propertyChanged: ForceUpdateStateWithoutAnimationAndTryGenerateEffect);

    public static readonly BindableProperty DisallowTouchThresholdProperty = BindableProperty.CreateAttached(
        nameof(DisallowTouchThreshold),
        typeof(int),
        typeof(TouchEffect),
        default(int),
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NativeAnimationProperty = BindableProperty.CreateAttached(
        nameof(NativeAnimation),
        typeof(bool),
        typeof(TouchEffect),
        false,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NativeAnimationColorProperty = BindableProperty.CreateAttached(
        nameof(NativeAnimationColor),
        typeof(Color),
        typeof(TouchEffect),
        Colors.Transparent,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NativeAnimationRadiusProperty = BindableProperty.CreateAttached(
        nameof(NativeAnimationRadius),
        typeof(int),
        typeof(TouchEffect),
        -1,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NativeAnimationShadowRadiusProperty = BindableProperty.CreateAttached(
        nameof(NativeAnimationShadowRadius),
        typeof(int),
        typeof(TouchEffect),
        -1,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NativeAnimationBorderlessProperty = BindableProperty.CreateAttached(
        nameof(NativeAnimationBorderless),
        typeof(bool),
        typeof(TouchEffect),
        false,
        propertyChanged: TryGenerateEffect);

    public static readonly BindableProperty NormalBackgroundImageSourceProperty = BindableProperty.CreateAttached(
        nameof(NormalBackgroundImageSource),
        typeof(ImageSource),
        typeof(TouchEffect),
        default(ImageSource),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredBackgroundImageSourceProperty = BindableProperty.CreateAttached(
        nameof(HoveredBackgroundImageSource),
        typeof(ImageSource),
        typeof(TouchEffect),
        default(ImageSource),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedBackgroundImageSourceProperty = BindableProperty.CreateAttached(
        nameof(PressedBackgroundImageSource),
        typeof(ImageSource),
        typeof(TouchEffect),
        default(ImageSource),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty BackgroundImageAspectProperty = BindableProperty.CreateAttached(
        nameof(BackgroundImageAspect),
        typeof(Aspect),
        typeof(TouchEffect),
        default(Aspect),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty NormalBackgroundImageAspectProperty = BindableProperty.CreateAttached(
        nameof(NormalBackgroundImageAspect),
        typeof(Aspect),
        typeof(TouchEffect),
        default(Aspect),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty HoveredBackgroundImageAspectProperty = BindableProperty.CreateAttached(
        nameof(HoveredBackgroundImageAspect),
        typeof(Aspect),
        typeof(TouchEffect),
        default(Aspect),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty PressedBackgroundImageAspectProperty = BindableProperty.CreateAttached(
        nameof(PressedBackgroundImageAspect),
        typeof(Aspect),
        typeof(TouchEffect),
        default(Aspect),
        propertyChanged: ForceUpdateStateAndTryGenerateEffect);

    public static readonly BindableProperty ShouldSetImageOnAnimationEndProperty = BindableProperty.CreateAttached(
        nameof(ShouldSetImageOnAnimationEnd),
        typeof(bool),
        typeof(TouchEffect),
        default(bool),
        propertyChanged: TryGenerateEffect);
    private readonly GestureManager gestureManager = new();
    private readonly WeakEventManager weakEventManager = new();
    private VisualElement? element;

    public TouchEffect()
        : base()
    {
        #region Required work-around to prevent linker from removing the platform-specific implementation
#if __ANDROID__
        			if (System.DateTime.Now.Ticks < 0)
        				_ = new PlatformTouchEffect();
#endif
        #endregion
    }

    public static bool GetIsAvailable(BindableObject? bindable)
        => (bool)(bindable?.GetValue(IsAvailableProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetIsAvailable(BindableObject? bindable, bool value)
        => bindable?.SetValue(IsAvailableProperty, value);

    public static bool GetShouldMakeChildrenInputTransparent(BindableObject? bindable)
        => (bool)(bindable?.GetValue(ShouldMakeChildrenInputTransparentProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetShouldMakeChildrenInputTransparent(BindableObject? bindable, bool value)
        => bindable?.SetValue(ShouldMakeChildrenInputTransparentProperty, value);

    public static ICommand GetCommand(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : (ICommand)bindable.GetValue(CommandProperty);

    public static void SetCommand(BindableObject? bindable, ICommand value)
        => bindable?.SetValue(CommandProperty, value);

    public static ICommand GetLongPressCommand(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : (ICommand)bindable.GetValue(LongPressCommandProperty);

    public static void SetLongPressCommand(BindableObject? bindable, ICommand value)
        => bindable?.SetValue(LongPressCommandProperty, value);

    public static object? GetCommandParameter(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : bindable.GetValue(CommandParameterProperty);

    public static void SetCommandParameter(BindableObject? bindable, object value)
        => bindable?.SetValue(CommandParameterProperty, value);

    public static object? GetLongPressCommandParameter(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : bindable.GetValue(LongPressCommandParameterProperty);

    public static void SetLongPressCommandParameter(BindableObject? bindable, object value)
        => bindable?.SetValue(LongPressCommandParameterProperty, value);

    public static int GetLongPressDuration(BindableObject? bindable)
        => (int)(bindable?.GetValue(LongPressDurationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetLongPressDuration(BindableObject? bindable, int value)
        => bindable?.SetValue(LongPressDurationProperty, value);

    public static TouchStatus GetStatus(BindableObject? bindable)
        => (TouchStatus)(bindable?.GetValue(StatusProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetStatus(BindableObject? bindable, TouchStatus value)
        => bindable?.SetValue(StatusProperty, value);

    public static TouchState GetState(BindableObject? bindable)
        => (TouchState)(bindable?.GetValue(StateProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetState(BindableObject? bindable, TouchState value)
        => bindable?.SetValue(StateProperty, value);

    public static TouchInteractionStatus GetInteractionStatus(BindableObject? bindable)
        => (TouchInteractionStatus)(bindable?.GetValue(InteractionStatusProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetInteractionStatus(BindableObject? bindable, TouchInteractionStatus value)
        => bindable?.SetValue(InteractionStatusProperty, value);

    public static HoverStatus GetHoverStatus(BindableObject? bindable)
        => (HoverStatus)(bindable?.GetValue(HoverStatusProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoverStatus(BindableObject? bindable, HoverStatus value)
        => bindable?.SetValue(HoverStatusProperty, value);

    public static HoverState GetHoverState(BindableObject? bindable)
        => (HoverState)(bindable?.GetValue(HoverStateProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoverState(BindableObject? bindable, HoverState value)
        => bindable?.SetValue(HoverStateProperty, value);

    public static Mcolor GetNormalBackgroundColor(BindableObject? bindable)
        => (Mcolor)(bindable?.GetValue(NormalBackgroundColorProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalBackgroundColor(BindableObject? bindable, Color value)
        => bindable?.SetValue(NormalBackgroundColorProperty, value);

    public static Mcolor GetHoveredBackgroundColor(BindableObject? bindable)
        => (Mcolor)(bindable?.GetValue(HoveredBackgroundColorProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredBackgroundColor(BindableObject? bindable, Color value)
        => bindable?.SetValue(HoveredBackgroundColorProperty, value);

    public static Mcolor GetPressedBackgroundColor(BindableObject? bindable)
        => (Mcolor)(bindable?.GetValue(PressedBackgroundColorProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedBackgroundColor(BindableObject? bindable, Color value)
        => bindable?.SetValue(PressedBackgroundColorProperty, value);

    public static double GetNormalOpacity(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalOpacityProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalOpacity(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalOpacityProperty, value);

    public static double GetHoveredOpacity(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredOpacityProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredOpacity(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredOpacityProperty, value);

    public static double GetPressedOpacity(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedOpacityProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedOpacity(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedOpacityProperty, value);

    public static double GetNormalScale(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalScaleProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalScale(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalScaleProperty, value);

    public static double GetHoveredScale(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredScaleProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredScale(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredScaleProperty, value);

    public static double GetPressedScale(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedScaleProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedScale(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedScaleProperty, value);

    public static double GetNormalTranslationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalTranslationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalTranslationX(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalTranslationXProperty, value);

    public static double GetHoveredTranslationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredTranslationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredTranslationX(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredTranslationXProperty, value);

    public static double GetPressedTranslationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedTranslationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedTranslationX(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedTranslationXProperty, value);

    public static double GetNormalTranslationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalTranslationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalTranslationY(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalTranslationYProperty, value);

    public static double GetHoveredTranslationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredTranslationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredTranslationY(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredTranslationYProperty, value);

    public static double GetPressedTranslationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedTranslationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedTranslationY(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedTranslationYProperty, value);

    public static double GetNormalRotation(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalRotationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalRotation(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalRotationProperty, value);

    public static double GetHoveredRotation(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredRotationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredRotation(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredRotationProperty, value);

    public static double GetPressedRotation(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedRotationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedRotation(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedRotationProperty, value);

    public static double GetNormalRotationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalRotationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalRotationX(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalRotationXProperty, value);

    public static double GetHoveredRotationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredRotationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredRotationX(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredRotationXProperty, value);

    public static double GetPressedRotationX(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedRotationXProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedRotationX(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedRotationXProperty, value);

    public static double GetNormalRotationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(NormalRotationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalRotationY(BindableObject? bindable, double value)
        => bindable?.SetValue(NormalRotationYProperty, value);

    public static double GetHoveredRotationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(HoveredRotationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredRotationY(BindableObject? bindable, double value)
        => bindable?.SetValue(HoveredRotationYProperty, value);

    public static double GetPressedRotationY(BindableObject? bindable)
        => (double)(bindable?.GetValue(PressedRotationYProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedRotationY(BindableObject? bindable, double value)
        => bindable?.SetValue(PressedRotationYProperty, value);

    public static int GetAnimationDuration(BindableObject? bindable)
        => (int)(bindable?.GetValue(AnimationDurationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetAnimationDuration(BindableObject? bindable, int value)
        => bindable?.SetValue(AnimationDurationProperty, value);

    public static Easing? GetAnimationEasing(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : (Easing?)bindable.GetValue(AnimationEasingProperty);

    public static void SetAnimationEasing(BindableObject? bindable, Easing? value)
        => bindable?.SetValue(AnimationEasingProperty, value);

    public static int GetPressedAnimationDuration(BindableObject? bindable)
       => (int)(bindable?.GetValue(PressedAnimationDurationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedAnimationDuration(BindableObject? bindable, int value)
        => bindable?.SetValue(PressedAnimationDurationProperty, value);

    public static Easing? GetPressedAnimationEasing(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (Easing?)bindable.GetValue(PressedAnimationEasingProperty);
    }

    public static void SetPressedAnimationEasing(BindableObject? bindable, Easing? value)
        => bindable?.SetValue(PressedAnimationEasingProperty, value);

    public static int GetNormalAnimationDuration(BindableObject? bindable)
        => (int)(bindable?.GetValue(NormalAnimationDurationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalAnimationDuration(BindableObject? bindable, int value)
        => bindable?.SetValue(NormalAnimationDurationProperty, value);

    public static Easing? GetNormalAnimationEasing(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (Easing?)bindable.GetValue(NormalAnimationEasingProperty);
    }

    public static void SetNormalAnimationEasing(BindableObject? bindable, Easing? value)
        => bindable?.SetValue(NormalAnimationEasingProperty, value);

    public static int GetHoveredAnimationDuration(BindableObject? bindable)
        => (int)(bindable?.GetValue(HoveredAnimationDurationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredAnimationDuration(BindableObject? bindable, int value)
        => bindable?.SetValue(HoveredAnimationDurationProperty, value);

    public static Easing? GetHoveredAnimationEasing(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (Easing?)bindable.GetValue(HoveredAnimationEasingProperty);
    }

    public static void SetHoveredAnimationEasing(BindableObject? bindable, Easing? value)
        => bindable?.SetValue(HoveredAnimationEasingProperty, value);

    public static int GetPulseCount(BindableObject? bindable)
        => (int)(bindable?.GetValue(PulseCountProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPulseCount(BindableObject? bindable, int value)
        => bindable?.SetValue(PulseCountProperty, value);

    public static bool? GetIsToggled(BindableObject? bindable) => bindable == null ? throw new ArgumentNullException(nameof(bindable)) : (bool?)bindable.GetValue(IsToggledProperty);

    public static void SetIsToggled(BindableObject? bindable, bool? value)
        => bindable?.SetValue(IsToggledProperty, value);

    public static int GetDisallowTouchThreshold(BindableObject? bindable)
        => (int)(bindable?.GetValue(DisallowTouchThresholdProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetDisallowTouchThreshold(BindableObject? bindable, int value)
        => bindable?.SetValue(DisallowTouchThresholdProperty, value);

    public static bool GetNativeAnimation(BindableObject? bindable)
        => (bool)(bindable?.GetValue(NativeAnimationProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNativeAnimation(BindableObject? bindable, bool value)
        => bindable?.SetValue(NativeAnimationProperty, value);

    public static Color GetNativeAnimationColor(BindableObject? bindable)
        => (Color)(bindable?.GetValue(NativeAnimationColorProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNativeAnimationColor(BindableObject? bindable, Color value)
        => bindable?.SetValue(NativeAnimationColorProperty, value);

    public static int GetNativeAnimationRadius(BindableObject? bindable)
        => (int)(bindable?.GetValue(NativeAnimationRadiusProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNativeAnimationRadius(BindableObject? bindable, int value)
        => bindable?.SetValue(NativeAnimationRadiusProperty, value);

    public static int GetNativeAnimationShadowRadius(BindableObject? bindable)
        => (int)(bindable?.GetValue(NativeAnimationShadowRadiusProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNativeAnimationShadowRadius(BindableObject? bindable, int value)
        => bindable?.SetValue(NativeAnimationShadowRadiusProperty, value);

    public static bool GetNativeAnimationBorderless(BindableObject? bindable)
        => (bool)(bindable?.GetValue(NativeAnimationBorderlessProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNativeAnimationBorderless(BindableObject? bindable, bool value)
        => bindable?.SetValue(NativeAnimationBorderlessProperty, value);

    public static ImageSource? GetNormalBackgroundImageSource(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (ImageSource?)bindable.GetValue(NormalBackgroundImageSourceProperty);
    }

    public static void SetNormalBackgroundImageSource(BindableObject? bindable, ImageSource value)
        => bindable?.SetValue(NormalBackgroundImageSourceProperty, value);

    public static ImageSource? GetHoveredBackgroundImageSource(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (ImageSource?)bindable.GetValue(HoveredBackgroundImageSourceProperty);
    }

    public static void SetHoveredBackgroundImageSource(BindableObject? bindable, ImageSource value)
        => bindable?.SetValue(HoveredBackgroundImageSourceProperty, value);

    public static ImageSource? GetPressedBackgroundImageSource(BindableObject? bindable)
    {
        return bindable == null
            ? throw new ArgumentNullException(nameof(bindable))
            : (ImageSource?)bindable.GetValue(PressedBackgroundImageSourceProperty);
    }

    public static void SetPressedBackgroundImageSource(BindableObject? bindable, ImageSource value)
        => bindable?.SetValue(PressedBackgroundImageSourceProperty, value);

    public static Aspect GetBackgroundImageAspect(BindableObject? bindable)
        => (Aspect)(bindable?.GetValue(BackgroundImageAspectProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetBackgroundImageAspect(BindableObject? bindable, Aspect value)
        => bindable?.SetValue(BackgroundImageAspectProperty, value);

    public static Aspect GetNormalBackgroundImageAspect(BindableObject? bindable)
        => (Aspect)(bindable?.GetValue(NormalBackgroundImageAspectProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetNormalBackgroundImageAspect(BindableObject? bindable, Aspect value)
        => bindable?.SetValue(NormalBackgroundImageAspectProperty, value);

    public static Aspect GetHoveredBackgroundImageAspect(BindableObject? bindable)
        => (Aspect)(bindable?.GetValue(HoveredBackgroundImageAspectProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetHoveredBackgroundImageAspect(BindableObject? bindable, Aspect value)
        => bindable?.SetValue(HoveredBackgroundImageAspectProperty, value);

    public static Aspect GetPressedBackgroundImageAspect(BindableObject? bindable)
        => (Aspect)(bindable?.GetValue(PressedBackgroundImageAspectProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetPressedBackgroundImageAspect(BindableObject? bindable, Aspect value)
        => bindable?.SetValue(PressedBackgroundImageAspectProperty, value);

    public static bool GetShouldSetImageOnAnimationEnd(BindableObject? bindable)
        => (bool)(bindable?.GetValue(ShouldSetImageOnAnimationEndProperty) ?? throw new ArgumentNullException(nameof(bindable)));

    public static void SetShouldSetImageOnAnimationEnd(BindableObject? bindable, bool value)
        => bindable?.SetValue(ShouldSetImageOnAnimationEndProperty, value);

    private static void TryGenerateEffect(BindableObject? bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement view || view.Effects.OfType<TouchEffect>().Any())
            return;
        view.Effects.Add(new TouchEffect { IsAutoGenerated = true });
    }

    private static void ForceUpdateStateAndTryGenerateEffect(BindableObject? bindable, object oldValue, object newValue)
    {
        GetFrom(bindable)?.ForceUpdateState();
        TryGenerateEffect(bindable, oldValue, newValue);
    }

    private static void ForceUpdateStateWithoutAnimationAndTryGenerateEffect(BindableObject? bindable, object oldValue, object newValue)
    {
        GetFrom(bindable)?.ForceUpdateState();
        TryGenerateEffect(bindable, oldValue, newValue);
    }

    private static void SetChildrenInputTransparentAndTryGenerateEffect(BindableObject? bindable, object oldValue, object newValue)
    {
        GetFrom(bindable)?.SetChildrenInputTransparent((bool)newValue);
        TryGenerateEffect(bindable, oldValue, newValue);
    }

    internal bool IsDisabled { get; set; }

    internal bool IsUsed { get; set; }

    internal bool IsAutoGenerated { get; set; }

    public bool IsAvailable => GetIsAvailable(Element);

    public bool ShouldMakeChildrenInputTransparent => GetShouldMakeChildrenInputTransparent(Element);

    public ICommand Command => GetCommand(Element);

    public ICommand LongPressCommand => GetLongPressCommand(Element);

    public object? CommandParameter => GetCommandParameter(Element);

    public object? LongPressCommandParameter => GetLongPressCommandParameter(Element);

    public int LongPressDuration => GetLongPressDuration(Element);

    public TouchStatus Status
    {
        get => GetStatus(Element);
        internal set => SetStatus(Element, value);
    }

    public TouchState State
    {
        get => GetState(Element);
        internal set => SetState(Element, value);
    }

    public TouchInteractionStatus InteractionStatus
    {
        get => GetInteractionStatus(Element);
        internal set => SetInteractionStatus(Element, value);
    }

    public HoverStatus HoverStatus
    {
        get => GetHoverStatus(Element);
        internal set => SetHoverStatus(Element, value);
    }

    public HoverState HoverState
    {
        get => GetHoverState(Element);
        internal set => SetHoverState(Element, value);
    }

    public int DisallowTouchThreshold => GetDisallowTouchThreshold(Element);

    public bool NativeAnimation => GetNativeAnimation(Element);

    public Color NativeAnimationColor => GetNativeAnimationColor(Element);

    public int NativeAnimationRadius => GetNativeAnimationRadius(Element);

    public int NativeAnimationShadowRadius => GetNativeAnimationShadowRadius(Element);

    public bool NativeAnimationBorderless => GetNativeAnimationBorderless(Element);

    public Mcolor NormalBackgroundColor => GetNormalBackgroundColor(Element);

    public Mcolor HoveredBackgroundColor => GetHoveredBackgroundColor(Element);

    public Mcolor PressedBackgroundColor => GetPressedBackgroundColor(Element);

    public double NormalOpacity => GetNormalOpacity(Element);

    public double HoveredOpacity => GetHoveredOpacity(Element);

    public double PressedOpacity => GetPressedOpacity(Element);

    public double NormalScale => GetNormalScale(Element);

    public double HoveredScale => GetHoveredScale(Element);

    public double PressedScale => GetPressedScale(Element);

    public double NormalTranslationX => GetNormalTranslationX(Element);

    public double HoveredTranslationX => GetHoveredTranslationX(Element);

    public double PressedTranslationX => GetPressedTranslationX(Element);

    public double NormalTranslationY => GetNormalTranslationY(Element);

    public double HoveredTranslationY => GetHoveredTranslationY(Element);

    public double PressedTranslationY => GetPressedTranslationY(Element);

    public double NormalRotation => GetNormalRotation(Element);

    public double HoveredRotation => GetHoveredRotation(Element);

    public double PressedRotation => GetPressedRotation(Element);

    public double NormalRotationX => GetNormalRotationX(Element);

    public double HoveredRotationX => GetHoveredRotationX(Element);

    public double PressedRotationX => GetPressedRotationX(Element);

    public double NormalRotationY => GetNormalRotationY(Element);

    public double HoveredRotationY => GetHoveredRotationY(Element);

    public double PressedRotationY => GetPressedRotationY(Element);

    public int AnimationDuration => GetAnimationDuration(Element);

    public Easing? AnimationEasing => GetAnimationEasing(Element);

    public int PressedAnimationDuration => GetPressedAnimationDuration(Element);

    public Easing? PressedAnimationEasing => GetPressedAnimationEasing(Element);

    public int NormalAnimationDuration => GetNormalAnimationDuration(Element);

    public Easing? NormalAnimationEasing => GetNormalAnimationEasing(Element);

    public int HoveredAnimationDuration => GetHoveredAnimationDuration(Element);

    public Easing? HoveredAnimationEasing => GetHoveredAnimationEasing(Element);

    public int PulseCount => GetPulseCount(Element);

    public bool? IsToggled
    {
        get => GetIsToggled(Element);
        internal set => SetIsToggled(Element, value);
    }

    public ImageSource? NormalBackgroundImageSource => GetNormalBackgroundImageSource(Element);

    public ImageSource? HoveredBackgroundImageSource => GetHoveredBackgroundImageSource(Element);

    public ImageSource? PressedBackgroundImageSource => GetPressedBackgroundImageSource(Element);

    public Aspect BackgroundImageAspect => GetBackgroundImageAspect(Element);

    public Aspect NormalBackgroundImageAspect => GetNormalBackgroundImageAspect(Element);

    public Aspect HoveredBackgroundImageAspect => GetHoveredBackgroundImageAspect(Element);

    public Aspect PressedBackgroundImageAspect => GetPressedBackgroundImageAspect(Element);

    public bool ShouldSetImageOnAnimationEnd => GetShouldSetImageOnAnimationEnd(Element);

    internal bool CanExecute => IsAvailable
        && (Element?.IsEnabled ?? false)
        && (Command?.CanExecute(CommandParameter) ?? true);

    internal new VisualElement? Element
    {
        get => element;
        set
        {
            if (element != null)
            {
                IsUsed = false;
                gestureManager.Reset();
                SetChildrenInputTransparent(false);
            }
            gestureManager.AbortAnimations(this);
            element = value;
            if (value != null)
            {
                SetChildrenInputTransparent(ShouldMakeChildrenInputTransparent);
                if (!IsAutoGenerated)
                {
                    IsUsed = true;
                    foreach (TouchEffect effect in value.Effects.OfType<TouchEffect>())
                        effect.IsDisabled = effect != this;
                }

                ForceUpdateState();
            }
        }
    }

    internal static TouchEffect? GetFrom(BindableObject? bindable)
    {
        IEnumerable<TouchEffect>? effects = (bindable as VisualElement)?.Effects?.OfType<TouchEffect>();
        return effects?.FirstOrDefault(x => !x.IsAutoGenerated) ?? effects?.FirstOrDefault();
    }

    internal static TouchEffect? PickFrom(BindableObject? bindable)
    {
        IEnumerable<TouchEffect>? effects = (bindable as VisualElement)?.Effects?.OfType<TouchEffect>();
        return effects?.FirstOrDefault(x => !x.IsAutoGenerated && !x.IsUsed)
            ?? effects?.FirstOrDefault(x => x.IsAutoGenerated)
            ?? effects?.FirstOrDefault();
    }

    internal void HandleTouch(TouchStatus status)
        => gestureManager.HandleTouch(this, status);

    internal void HandleUserInteraction(TouchInteractionStatus interactionStatus)
        => GestureManager.HandleUserInteraction(this, interactionStatus);

    internal void HandleHover(HoverStatus status)
        => GestureManager.HandleHover(this, status);

    internal void RaiseStateChanged()
    {
        ForceUpdateState();
        HandleLongPress();
        weakEventManager.HandleEvent(Element, new TouchStateChangedEventArgs(State), nameof(StateChanged));
    }

    internal void RaiseInteractionStatusChanged()
        => weakEventManager.HandleEvent(Element, new TouchInteractionStatusChangedEventArgs(InteractionStatus), nameof(InteractionStatusChanged));

    internal void RaiseStatusChanged()
        => weakEventManager.HandleEvent(Element, new TouchStatusChangedEventArgs(Status), nameof(StatusChanged));

    internal void RaiseHoverStateChanged()
    {
        ForceUpdateState();
        weakEventManager.HandleEvent(Element, new HoverStateChangedEventArgs(HoverState), nameof(HoverStateChanged));
    }

    internal void RaiseHoverStatusChanged()
        => weakEventManager.HandleEvent(Element, new HoverStatusChangedEventArgs(HoverStatus), nameof(HoverStatusChanged));

    internal void RaiseCompleted()
    {
        VisualElement element = Element;
        if (element == null)
            return;

        object parameter = CommandParameter;
        Command?.Execute(parameter);
        weakEventManager.HandleEvent(element, new TouchCompletedEventArgs(parameter), nameof(Completed));
    }

    internal void RaiseLongPressCompleted()
    {
        VisualElement element = Element;
        if (element == null)
            return;

        object parameter = LongPressCommandParameter ?? CommandParameter;
        LongPressCommand?.Execute(parameter);
        weakEventManager.HandleEvent(element, new LongPressCompletedEventArgs(parameter), nameof(LongPressCompleted));
    }

    internal void ForceUpdateState(bool animated = true)
    {
        if (Element == null)
            return;

        gestureManager.ChangeStateAsync(this, animated).SafeFireAndForget();
    }

    internal void HandleLongPress()
    {
        if (Element == null)
            return;

        gestureManager.HandleLongPress(this);
    }

    private void SetChildrenInputTransparent(bool value)
    {
        if (Element is not Layout layout)
            return;

        layout.ChildAdded -= OnLayoutChildAdded;
        if (!value)
            return;
        layout.InputTransparent = false;
        foreach (IView view in layout.Children)
            OnLayoutChildAdded(layout, new ElementEventArgs((Element)view));

        layout.ChildAdded += OnLayoutChildAdded;
    }

    private void OnLayoutChildAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is not Mview view)
            return;

        if (!ShouldMakeChildrenInputTransparent)
        {
            view.InputTransparent = false;
            return;
        }

        TouchEffect? effect = GetFrom(view);
        view.InputTransparent = effect?.Element == null || !effect.IsAvailable;
    }
}