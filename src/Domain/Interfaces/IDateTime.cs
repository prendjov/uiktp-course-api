namespace Domain.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime FromUnixTimestamp(double unixTimestamp);
}
