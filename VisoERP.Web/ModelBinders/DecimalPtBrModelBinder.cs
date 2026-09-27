using VisoERP.Infra.Helper;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VisoERP.Web.ModelBinders;

public sealed class DecimalPtBrModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
        var rawValue = valueResult.FirstValue?.Trim();
        if (string.IsNullOrEmpty(rawValue))
        {
            if (bindingContext.ModelMetadata.IsNullableValueType)
                bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (DecimalPtBrParser.TryParse(rawValue, out var value))
        {
            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.AddModelError(bindingContext.ModelName,
            "Informe um valor numérico válido.");
        return Task.CompletedTask;
    }

}

public sealed class DecimalPtBrModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context) =>
        context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?)
            ? new DecimalPtBrModelBinder()
            : null;
}
