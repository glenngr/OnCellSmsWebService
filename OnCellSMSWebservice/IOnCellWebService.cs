using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OnCellSMSWebservice
{
    [ServiceContract]
    public interface IOnCellWebService
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "invoke", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SmsTo(string phoneNumbers, string message);
    }
}
