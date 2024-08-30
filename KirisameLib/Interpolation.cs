namespace KirisameLib;

public class Interpolation
{
    //Initializer
    private Interpolation(Func<double, double> interpolation, bool easeModeIn)
    {
        _interpolation = interpolation;
        _easeModeIn = easeModeIn;
    }

    public static Interpolation From(Func<double, double> interpolation, bool easeIn = true) => new Interpolation(interpolation, easeIn);


    //Func
    private readonly Func<double, double> _interpolation;


    //Ease mode
    private readonly bool _easeModeIn;
    private Func<double, double> Reverse() => t => 1 - _interpolation(1 - t);

    private Func<double, double> ForwardReverse() =>
        t => t < 0.5
                 ? _interpolation(2 * t) / 2
                 : 1 - _interpolation(2 - 2 * t) / 2;

    private Func<double, double> ReverseForward() =>
        t =>
            t < 0.5
                ? (1 - _interpolation(1 - 2 * t)) / 2
                : (1 + _interpolation(2 * t - 1)) / 2;

    public Func<double, double> EaseIn => _easeModeIn ? _interpolation : Reverse();
    public Func<double, double> EaseOut => _easeModeIn ? Reverse() : _interpolation;

    public Func<double, double> EaseInOut => _easeModeIn ? ForwardReverse() : ReverseForward();

    public Func<double, double> EaseOutIn => _easeModeIn ? ReverseForward() : ForwardReverse();


    //Interpolations
    public static Func<double, double> Exponential(double exponent) => t => Math.Pow(t, exponent);

    public static Interpolation Linear => From(t => t);
    public static Interpolation Quadratic => From(t => t * t);
    public static Interpolation Cubic => From(t => t * t * t);
    public static Interpolation Quartic => From(t => t * t * t * t);
    public static Interpolation Quintic => From(t => t * t * t * t * t);
    public static Interpolation Sin => From(t => Math.Sin(t * Math.PI / 2), false);
    public static Interpolation Circle => From(t => 1 - Math.Sqrt(1 - t * t));
    public static Interpolation Back => From(t => t * t * ((1.70158 + 1) * t - 1.70158));
    public static Interpolation Elastic => From(t => 1 - Math.Cos(t * Math.PI * 2 / 0.3) * Math.Exp(-7 * t), false);
    public static Interpolation Spring => From(t => throw new NotImplementedException());
    public static Interpolation Bounce => From(t => throw new NotImplementedException());
    
}