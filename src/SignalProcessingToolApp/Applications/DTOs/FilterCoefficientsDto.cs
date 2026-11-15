namespace SignalProcessingToolApp.Applications.DTOs;

public record FilterCoefficientsDto(
    double B0,
    double B1,
    double B2,
    double A1,
    double A2
);
