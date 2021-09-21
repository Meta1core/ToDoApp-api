using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoApp.Hubs
{
    [HubName("taskHub")]
    public class TasksHub : Hub
    {
        public void FromClient (string message)
        {
        }
    }
} 