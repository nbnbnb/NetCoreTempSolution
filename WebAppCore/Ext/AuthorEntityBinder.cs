using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppCore.Entities;

namespace WebAppCore.Ext
{
    public class AuthorEntityBinder : IModelBinder
    {
        // 可以使用 DI

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Specify a default argument name if none is set by ModelBinderAttribute
            // BinderModelName 是通过 public IActionResult GetById([ModelBinder(Name = "id")]Author author) 
            // 这种方式指定的
            // public IActionResult Get(Author author) 这种方式，BinderModelName 为空
            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = "authorId";
            }

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            // 设置 ValueProviderResult
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            int id = 0;
            if (!int.TryParse(value, out id))
            {
                // Non-integer arguments result in model state errors
                // 添加 ModelState 的 Error 状态                
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Author Id must be an integer.");
                return Task.CompletedTask;
            }

            // Model will be null if not found, including for 
            // out of range id values (0, -3, etc.)
            var model = new Author { Name = "KKKing", Id = id };

            // 设置 Result
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}
