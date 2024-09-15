using Hangfire;
using Hangfire.MissionControl;
using Hangfire.Server;

namespace Riton.Web.Missions;

[MissionLauncher(CategoryName = "TodoLists Missions")]
public class TodoListMission(ILogger logger, PerformingContext context)
{
    [DisableConcurrentExecution(60)]
    [JobDisplayName("EnqueueOverdueCheques{0:cdt}")]
    public async Task EnqueueOverdueCheques(string chequeQuery)
    {
        
    }

    
}
