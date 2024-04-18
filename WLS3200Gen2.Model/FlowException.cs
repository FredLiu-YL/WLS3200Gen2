using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class FlowException : Exception
    {
        // 可以在建構函式中加入自訂的邏輯
        public FlowException()
        {
        }
        // 建構函式可以接收錯誤訊息參數，將錯誤訊息傳遞給基底 Exception 類別
        public FlowException(string message) : base(message)
        {
        }
        // 建構函式可以接收錯誤訊息及內部例外參數
        public FlowException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
