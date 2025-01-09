using SMIJobXml.Common;
using SMIJobXml.Model;
using SMIJobXml.Model.Message;

namespace SMIJobXml.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static void CustomEnsureSuccessStatusCode(this HttpResponseMessage response)
        {

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            ExceptionMessage messageError = GetException(response.Content);
            throw new BusinessLogicException(messageError.ErrorCode, messageError.Message);
        }


        private static ExceptionMessage GetException(HttpContent content)
        {
            var strContents = content.ReadAsStringAsync().Result;
            var apiResult = strContents.DeserializeObject<ApiResult>();
            var exceptionMessage = new ExceptionMessage()
            {
                ErrorCode = ResultCode.UnknownError,
                Message = strContents,
            };
            if (apiResult == null) return exceptionMessage;

            if (apiResult.Code.IsNotNullOrEmpty())
            {
                return new ExceptionMessage()
                {
                    ErrorCode = (ResultCode)apiResult.Code.ToInt(99),
                    Message = apiResult.Message,
                };
            }

            if (apiResult.Error == null) return exceptionMessage;
            return new ExceptionMessage()
            {
                ErrorCode = (ResultCode)apiResult.Error.Code.ToInt(99),
                Message = apiResult.Error.Message,
            };
        }
    }
}
