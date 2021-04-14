using System;

namespace Amlakbashi.Core.Infrastructure.LocalizationHelpers
{
    public static class GeneralLocalization
    {
        public static string GetExceptionMessage(Exception exc)
        {
            return "متاسفانه عملیات با خطای فنی مواجه شد. کد خطا: " + exc.HResult;
        }
    }
}
