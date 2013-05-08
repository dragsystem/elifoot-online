using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace EmpreendaVc.Web.Mvc.Util
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext) {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try {
                string semPontos = valueResult.AttemptedValue.Replace(".", "");
                string semUltimaVirgula = string.IsNullOrEmpty(semPontos) == false && semPontos.Substring(semPontos.Length - 1, 1) == "," ? semPontos.Substring(0, semPontos.Length - 1) : semPontos;

                IFormatProvider fp = new System.Globalization.CultureInfo("pt-BR");
                actualValue = Convert.ToDecimal(semUltimaVirgula, fp);
            }
            catch (FormatException e) {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}