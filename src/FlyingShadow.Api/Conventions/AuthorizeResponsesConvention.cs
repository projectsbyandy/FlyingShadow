using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FlyingShadow.Api.Conventions;

public sealed class AuthorizeResponsesConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        ArgumentNullException.ThrowIfNull(application);

        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (!RequiresAuthorization(controller, action))
                    continue;

                AddResponseTypeIfMissing(action, StatusCodes.Status401Unauthorized);

                if (HasRoleOrPolicyRequirement(controller, action))
                    AddResponseTypeIfMissing(action, StatusCodes.Status403Forbidden);
            }
        }
    }

    private static bool RequiresAuthorization(ControllerModel controller, ActionModel action)
    {
        // Action-level [AllowAnonymous] always wins.
        if (HasAttribute<IAllowAnonymous>(action))
            return false;

        // If the action explicitly declares [Authorize], it overrides controller-level [AllowAnonymous].
        var actionHasAuthorize = HasAttribute<IAuthorizeData>(action);

        if (HasAttribute<IAllowAnonymous>(controller) && !actionHasAuthorize)
            return false;

        return actionHasAuthorize || HasAttribute<IAuthorizeData>(controller);
    }

    private static bool HasRoleOrPolicyRequirement(ControllerModel controller, ActionModel action)
    {
        return GetAuthorizeData(controller)
            .Concat(GetAuthorizeData(action))
            .Any(a => !string.IsNullOrEmpty(a.Roles) || !string.IsNullOrEmpty(a.Policy));
    }

    private static void AddResponseTypeIfMissing(ActionModel action, int statusCode)
    {
        var alreadyDeclared = action.Filters
            .OfType<IApiResponseMetadataProvider>()
            .Any(f => f.StatusCode == statusCode);

        if (alreadyDeclared)
            return;

        action.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), statusCode));
    }

    private static bool HasAttribute<T>(ControllerModel controller) =>
        controller.Attributes.OfType<T>().Any();

    private static bool HasAttribute<T>(ActionModel action) =>
        action.Attributes.OfType<T>().Any();

    private static IEnumerable<IAuthorizeData> GetAuthorizeData(ControllerModel controller) =>
        controller.Attributes.OfType<IAuthorizeData>();

    private static IEnumerable<IAuthorizeData> GetAuthorizeData(ActionModel action) =>
        action.Attributes.OfType<IAuthorizeData>();
}