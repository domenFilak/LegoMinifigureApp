namespace LegoMinifigureApp.Models.ViewModels.Errors
{
    public class ErrorPageViewModel
    {
        public int StatusCode { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string BrickClass { get; init; } = string.Empty; //color and tilt of the big brick
        public string? ReferenceId { get; init; } //only for crashes, to find error in logs

        //404 and other error status codes
        public static ErrorPageViewModel ForStatusCode(int statusCode)
        {
            //only real error codes; anything else treated as "not found"
            if (statusCode < 400 || statusCode > 599)
            {
                statusCode = 404;
            }

            if (statusCode == 404)
            {
                return new ErrorPageViewModel
                {
                    StatusCode = statusCode,
                    Title = "This brick doesn't exist",
                    Message = "We dug through the whole bucket, but this page isn't in it. It may have been moved, or it never existed.",
                    BrickClass = "error-brick-yellow"
                };
            }

            if (statusCode >= 500)
            {
                return ForServerError(referenceId: null, statusCode);
            }

            return new ErrorPageViewModel
            {
                StatusCode = statusCode,
                Title = "This piece doesn't fit",
                Message = "Your request couldn't be handled. Please go back and try again.",
                BrickClass = "error-brick-blue"
            };
        }

        //crash in code 500, or other server error codes
        public static ErrorPageViewModel ForServerError(string? referenceId, int statusCode = 500)
        {
            return new ErrorPageViewModel
            {
                StatusCode = statusCode,
                Title = "Oops, the build collapsed",
                Message = "Something went wrong on our side. Please try again in a moment.",
                BrickClass = "error-brick-red error-brick-tilted",
                ReferenceId = referenceId
            };
        }
    }
}
