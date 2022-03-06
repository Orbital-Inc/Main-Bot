using System.ComponentModel.DataAnnotations;

namespace MainBot.Database.Models.DiscordBackup.Permissions;

public class RolePermissions
{
    [Key]
    public int key { get; set; }
    //
    // Summary:
    //     If true, a user may speak in a voice channel.
    public bool Speak { get; set; }
    //
    // Summary:
    //     If true, a user may mute users.
    public bool MuteMembers { get; set; }
    //
    // Summary:
    //     If true, a user may deafen users.
    public bool DeafenMembers { get; set; }
    //
    // Summary:
    //     If true, a user may move other users between voice channels.
    public bool MoveMembers { get; set; }
    //
    // Summary:
    //     If true, a user may use voice-activity-detection rather than push-to-talk.
    public bool UseVAD { get; set; }
    //
    // Summary:
    //     If True, a user may use priority speaker in a voice channel.
    public bool PrioritySpeaker { get; set; }
    //
    // Summary:
    //     If True, a user may stream video in a voice channel.
    public bool Stream { get; set; }
    //
    // Summary:
    //     If true, a user may change their own nickname.
    public bool ChangeNickname { get; set; }
    //
    // Summary:
    //     If true, a user may change the nickname of other users.
    public bool ManageNicknames { get; set; }
    //
    // Summary:
    //     If true, a user may edit the emojis and stickers for this guild.
    public bool ManageEmojisAndStickers { get; set; }
    //
    // Summary:
    //     If true, a user may edit the webhooks for this guild.
    public bool ManageWebhooks { get; set; }
    //
    // Summary:
    //     If true, a user may connect to a voice channel.
    public bool Connect { get; set; }
    //
    // Summary:
    //     If true, a user may use slash commands in this guild.
    public bool UseApplicationCommands { get; set; }
    //
    // Summary:
    //     If true, a user may request to speak in stage channels.
    public bool RequestToSpeak { get; set; }
    //
    // Summary:
    //     If true, a user may create, edit, and delete events.
    public bool ManageEvents { get; set; }
    //
    // Summary:
    //     If true, a user may manage threads in this guild.
    public bool ManageThreads { get; set; }
    //
    // Summary:
    //     If true, a user may create public threads in this guild.
    public bool CreatePublicThreads { get; set; }
    //
    // Summary:
    //     If true, a user may create private threads in this guild.
    public bool CreatePrivateThreads { get; set; }
    //
    // Summary:
    //     If true, a user may use external stickers in this guild.
    public bool UseExternalStickers { get; set; }
    //
    // Summary:
    //     If true, a user may adjust roles.
    public bool ManageRoles { get; set; }
    //
    // Summary:
    //     If true, a user may use custom emoji from other guilds.
    public bool UseExternalEmojis { get; set; }
    //
    // Summary:
    //     If true, a user may send files.
    public bool AttachFiles { get; set; }
    //
    // Summary:
    //     If true, a user may read previous messages.
    public bool ReadMessageHistory { get; set; }
    //
    // Summary:
    //     If true, a user may create invites.
    public bool CreateInstantInvite { get; set; }
    //
    // Summary:
    //     If true, a user may ban users from the guild.
    public bool BanMembers { get; set; }
    //
    // Summary:
    //     If true, a user may kick users from the guild.
    public bool KickMembers { get; set; }
    //
    // Summary:
    //     If true, a user is granted all permissions, and cannot have them revoked via
    //     channel permissions.
    public bool Administrator { get; set; }
    //
    // Summary:
    //     If true, a user may mention @everyone.
    public bool MentionEveryone { get; set; }
    //
    // Summary:
    //     If true, a user may adjust guild properties.
    public bool ManageGuild { get; set; }
    //
    // Summary:
    //     If true, a user may add reactions.
    public bool AddReactions { get; set; }
    //
    // Summary:
    //     If true, a user may create, delete and modify channels.
    public bool ManageChannels { get; set; }
    //
    // Summary:
    //     If true, a user may view the guild insights.
    public bool ViewGuildInsights { get; set; }
    //
    // Summary:
    //     If True, a user may view channels.
    public bool ViewChannel { get; set; }
    //
    // Summary:
    //     If True, a user may send messages.
    public bool SendMessages { get; set; }
    //
    // Summary:
    //     If true, a user may send text-to-speech messages.
    public bool SendTTSMessages { get; set; }
    //
    // Summary:
    //     If true, a user may delete messages.
    public bool ManageMessages { get; set; }
    //
    // Summary:
    //     If true, Discord will auto-embed links sent by this user.
    public bool EmbedLinks { get; set; }
    //
    // Summary:
    //     If true, a user may send messages in threads in this guild.
    public bool SendMessagesInThreads { get; set; }
    //
    // Summary:
    //     If true, a user may view the audit log.
    public bool ViewAuditLog { get; set; }
    //
    // Summary:
    //     If true, a user launch application activities in voice channels in this guild.
    public bool StartEmbeddedActivities { get; set; }

    public bool useVoiceActivation { get; set; }
    public bool moderateMembers { get; set; }
}
