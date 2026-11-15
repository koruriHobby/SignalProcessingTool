#nullable enable

namespace SignalProcessingToolApp.Domains.Parameters;

public record IirParameter
{
    public double B0 { get; }
    public double B1 { get; }
    public double B2 { get; }
    public double A1 { get; }
    public double A2 { get; }

    public IirParameter(
        double b0,
        double b1,
        double b2,
        double a1,
        double a2)
    {
        B0 = b0;
        B1 = b1;
        B2 = b2;
        A1 = a1;
        A2 = a2;
    }
}
