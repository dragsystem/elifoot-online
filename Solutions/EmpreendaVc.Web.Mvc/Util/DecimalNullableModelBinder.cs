using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace EmpreendaVc.Web.Mvc.Util
{
    public class DecimalNullableModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext) {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try {
                if (!string.IsNullOrEmpty(valueResult.AttemptedValue) && (valueResult.AttemptedValue != ",")) {

                    var log = log4net.LogManager.GetLogger("LogInFile");

                    log.Debug(string.Format("(DecimalNullableModelBinder) valueResult.AttemptedValue = {0}", valueResult.AttemptedValue));

                    string semPontos = valueResult.AttemptedValue.Replace(".", "");
                    string semUltimaVirgula = string.IsNullOrEmpty(semPontos) == false && semPontos.Substring(semPontos.Length - 1, 1) == "," ? semPontos.Substring(0, semPontos.Length - 1) : semPontos;

                    IFormatProvider fp = new System.Globalization.CultureInfo("pt-BR");
                    actualValue = Convert.ToDecimal(semUltimaVirgula, fp);

                    log.Debug(string.Format("(DecimalNullableModelBinder) actualValue = {0}", actualValue));
                }
            }
            catch (FormatException e) {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}