using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.DataClass
{
    public class MessageDetail
    {
    }

    public class Message
    {
        public string MessageNo { get; set; }
        public string SendBy { get; set; }
        public string SendByName { get; set; }
        public string SendDateTime { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Reciever { get; set; }
        public string RecieverName { get; set; }
        public string Flag { get; set; }
        public string SMSFlag { get; set; }
    }
}