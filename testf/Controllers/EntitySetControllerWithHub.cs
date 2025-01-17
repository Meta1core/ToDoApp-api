﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class EntitySetControllerWithHub<THub> : ApiController
        where THub : IHub
    {
        Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
            );

        protected IHubContext Hub
        {
            get { return hub.Value; }

        }
    }
}