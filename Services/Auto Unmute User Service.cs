using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main_Bot.Services;

internal class AutoUnmuteUserService : BackgroundService
{
    internal static HashSet<Models.MuteUserModel> _muteUsers = new();
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    private async Task AutoUnmuteUsersAsync()
    {

    }
}
