using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ToDoApp.Hubs
{
    [HubName("taskHub")]
    public class TasksHub : Hub
    {
        public void FromClient(string message)
        {
        }
    }
}