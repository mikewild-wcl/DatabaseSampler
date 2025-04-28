using System.ComponentModel.DataAnnotations;

namespace DatabaseSampler.Application.ViewModels;

public class PostcodeLookupViewModel
{
    [Required(ErrorMessage = "You must enter a postcode")]
    public string Postcode { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public bool IsTerminated { get; set; }

    public short? TerminatedYear { get; set; }

    public short? TerminatedMonth { get; set; }


    public bool UseCachedResponse { get; set; }

    public bool FoundInCache { get; set; }

    public TimeSpan? ElapsedTimeForLookup { get; set; }
}
