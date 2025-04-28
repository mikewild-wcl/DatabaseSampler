using System.Net;
using DatabaseSampler.Application.Messages;

namespace DatabaseSampler.Application.Tests.TestHttpFactories
{
    public class PostcodesTestHttpClientFactory : TestHttpClientFactory
    {
        public HttpClient Get(string requestPostcode,
            bool isValidPostcode,
            PostcodeLookupResultData postcodeResponseData,
            bool isTerminatedPostcode,
            TerminatedPostcodeLookupResultData terminatedPostcodeResponseData)
        {
            var requests = new List<RequestWrapper>();

            if (isValidPostcode && postcodeResponseData != null)
            {
                requests.Add(new RequestWrapper
                {
                    Uri = $"https://api.postcodes.io/postcodes/{Uri.EscapeDataString(requestPostcode)}",
                    ResponseObject = new PostcodeLookupResponse
                    {
                        Result = postcodeResponseData,
                        Status = (int)HttpStatusCode.OK
                    }
                });
            }

            if (isTerminatedPostcode && terminatedPostcodeResponseData != null)
            {
                requests.Add(new RequestWrapper
                {
                    Uri = $"https://api.postcodes.io/terminated_postcodes/{Uri.EscapeDataString(requestPostcode)}",
                    ResponseObject = new TerminatedPostcodeLookupResponse
                    {
                        Result = terminatedPostcodeResponseData,
                        Status = (int)HttpStatusCode.OK
                    } 
                });
            }

            return CreateClient(requests);
        }
    }
}