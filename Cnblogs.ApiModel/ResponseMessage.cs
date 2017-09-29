using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class ResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public ResponseMessage() { }
        public ResponseMessage(bool isSuccess,string  message) {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
