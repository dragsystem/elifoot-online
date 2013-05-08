using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
using System.Web.Mvc;

namespace EmpreendaVc.Web.Mvc.Util
{
    public static class ModelStateDictionaryExtensions
    {
        public static ErrorDictionary GetErrorDictionary(this ModelStateDictionary modelState) {
            var errors = new ErrorDictionary();

            foreach (var model in modelState) {
                foreach (var error in model.Value.Errors) {
                    errors.Add(model.Key, error.ErrorMessage);
                }
            }

            return errors;
        }
    }
}