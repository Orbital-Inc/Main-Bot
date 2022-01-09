using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Main_Bot.Commands.Slash_Commands.User_Commands;

public class ProfileCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("profile", "Display details about your account.")]
    public async Task ViewProfile(IUser? user = null)
    {
        //finish command later.
        await Task.CompletedTask;
    }
}
