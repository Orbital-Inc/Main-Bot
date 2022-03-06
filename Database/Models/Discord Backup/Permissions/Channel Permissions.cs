using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.DiscordBackup.Permissions;

public class ChannelPermissions
{
    [Key]
    public int key { get; set; }
    public ulong targetId { get; set; }
    public int permissionTarget { get; set; }
    //
    // Summary:
    //     If Allowed, a user may send files.
    public PermissionValue AttachFiles { get; set; }
    //
    // Summary:
    //     If Allowed, a user may speak in a voice channel.
    public PermissionValue Speak { get; set; }
    //
    // Summary:
    //     If Allowed, a user may mute users.
    public PermissionValue MuteMembers { get; set; }
    //
    // Summary:
    //     If Allowed, a user may deafen users.
    public PermissionValue DeafenMembers { get; set; }
    //
    // Summary:
    //     If Allowed, a user may move other users between voice channels.
    public PermissionValue MoveMembers { get; set; }
    //
    // Summary:
    //     If Allowed, a user may use voice-activity-detection rather than push-to-talk.
    public PermissionValue UseVAD { get; set; }
    //
    // Summary:
    //     If Allowed, a user may use priority speaker in a voice channel.
    public PermissionValue PrioritySpeaker { get; set; }
    //
    // Summary:
    //     If Allowed, a user may go live in a voice channel.
    public PermissionValue Stream { get; set; }
    //
    // Summary:
    //     If true, a user may use slash commands in this guild.
    public PermissionValue UseApplicationCommands { get; set; }
    //
    // Summary:
    //     If True, a user may edit the webhooks for this channel.
    public PermissionValue ManageWebhooks { get; set; }
    //
    // Summary:
    //     If Allowed, a user may connect to a voice channel.
    public PermissionValue Connect { get; set; }
    //
    // Summary:
    //     If true, a user may request to speak in stage channels.
    public PermissionValue RequestToSpeak { get; set; }
    //
    // Summary:
    //     If true, a user may manage threads in this guild.
    public PermissionValue ManageThreads { get; set; }
    //
    // Summary:
    //     If true, a user may create public threads in this guild.
    public PermissionValue CreatePublicThreads { get; set; }
    //
    // Summary:
    //     If true, a user may create private threads in this guild.
    public PermissionValue CreatePrivateThreads { get; set; }
    //
    // Summary:
    //     If true, a user may use external stickers in this guild.
    public PermissionValue UseExternalStickers { get; set; }
    //
    // Summary:
    //     If Allowed, a user may adjust role permissions. This also implicitly grants all
    //     other permissions.
    public PermissionValue ManageRoles { get; set; }
    //
    // Summary:
    //     If Allowed, a user may use custom emoji from other guilds.
    public PermissionValue UseExternalEmojis { get; set; }
    //
    // Summary:
    //     If true, a user launch application activities in voice channels in this guild.
    public PermissionValue StartEmbeddedActivities { get; set; }
    //
    // Summary:
    //     If Allowed, a user may read previous messages.
    public PermissionValue ReadMessageHistory { get; set; }
    //
    // Summary:
    //     If Allowed, a user may mention @everyone.
    public PermissionValue MentionEveryone { get; set; }
    //
    // Summary:
    //     If Allowed, a user may create, delete and modify this channel.
    public PermissionValue ManageChannel { get; set; }
    //
    // Summary:
    //     If Allowed, a user may add reactions.
    public PermissionValue AddReactions { get; set; }
    //
    // Summary:
    //     If Allowed, a user may create invites.
    public PermissionValue CreateInstantInvite { get; set; }
    //
    // Summary:
    //     If Allowed, a user may send messages.
    public PermissionValue SendMessages { get; set; }
    //
    // Summary:
    //     If Allowed, a user may send text-to-speech messages.
    public PermissionValue SendTTSMessages { get; set; }
    //
    // Summary:
    //     If Allowed, a user may delete messages.
    public PermissionValue ManageMessages { get; set; }
    //
    // Summary:
    //     If Allowed, Discord will auto-embed links sent by this user.
    public PermissionValue EmbedLinks { get; set; }
    //
    // Summary:
    //     If true, a user may send messages in threads in this guild.
    public PermissionValue SendMessagesInThreads { get; set; }
    //
    // Summary:
    //     If Allowed, a user may join channels.
    public PermissionValue ViewChannel { get; set; }

    public PermissionValue useVoiceActivation { get; set; }
    public PermissionValue useSlashCommands { get; set; }
    public PermissionValue usePrivateThreads { get; set; }
}

public enum PermissionValue
{
    //
    // Summary:
    //     Allows this permission.
    Allow = 0,
    //
    // Summary:
    //     Denies this permission.
    Deny = 1,
    //
    // Summary:
    //     Inherits the permission settings.
    Inherit = 2
}
