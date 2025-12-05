namespace DatabaseSampler.Application.Models;

public class Location
{
    public string Postcode { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
    
    public string DistrictCode { get; set; }

    public bool IsTerminated { get; set; }

    public short? TerminatedYear { get; set; }

    public short? TerminatedMonth { get; set; }
}
