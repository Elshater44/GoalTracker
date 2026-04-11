using GoalTracker.Common.Errors;
using GoalTracker.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Common.Results.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToProblemResult(this Result result, ControllerBase controller)
        {
            var error = result.Error!;
            return error.Type switch
            {
                ErrorType.NotFound => controller.NotFound(ToProblemDetails(error, 404)),
                ErrorType.Validation => controller.UnprocessableEntity(ToProblemDetails(error, 422)),
                ErrorType.Conflict => controller.Conflict(ToProblemDetails(error, 409)),
                ErrorType.Unauthorized => controller.Unauthorized(ToProblemDetails(error, 401)),
                _ => controller.StatusCode(500, ToProblemDetails(error, 500)),
            };
        }

        private static ProblemDetails ToProblemDetails(Error error, int status) => new()
        {
            Title = error.Id,
            Detail = error.Description,
            Status = status,
            Type = $"https://httpstatuses.com/{status}"
        };
    }
}