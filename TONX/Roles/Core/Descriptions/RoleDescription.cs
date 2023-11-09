using System.Text;

namespace TONX.Roles.Core.Descriptions;

public abstract class RoleDescription
{
    public RoleDescription(SimpleRoleInfo roleInfo)
    {
        RoleInfo = roleInfo;
    }

    public SimpleRoleInfo RoleInfo { get; }
    /// <summary>イントロなどで表示される短い文</summary>
    public abstract string Blurb { get; }
    /// <summary>
    /// ヘルプコマンドで使用される長い説明文<br/>
    /// AmongUs2023.7.12時点で，Impostor, Crewmateに関してはバニラ側でロング説明文が未実装のため「タスクを行う」と表示される
    /// </summary>
    public abstract string Description { get; }
    public string FullFormatHelp
    {
        get
        {
            var builder = new StringBuilder(256);
            builder.AppendFormat("<size={0}>\n", BlankLineSize);
            // 职业名
            builder.AppendFormat("<size={0}>{1}", FirstHeaderSize, Translator.GetRoleString(RoleInfo.RoleName.ToString()).Color(RoleInfo.RoleColor.ToReadableColor()));
            // 职业阵营 / 原版职业
            var roleTeam = RoleInfo.CustomRoleType;
            builder.AppendFormat("<size={0}> ({1}, {2})\n", BodySize, Translator.GetString($"Team{roleTeam}"), Translator.GetString("BaseOn") + Translator.GetString(RoleInfo.BaseRoleType.Invoke().ToString()));
            builder.AppendFormat("<size={0}>{1}\n", BodySize, Description);
            // 职业设定
            if (Options.CustomRoleSpawnChances.TryGetValue(RoleInfo.RoleName, out var opt))
                Utils.ShowChildrenSettings(opt, ref builder, forChat: true);

            return builder.ToString();
        }
    }
    public string GetFullFormatHelpWithAddons(PlayerControl player)
    {
        var builder = new StringBuilder(512);
        builder.Append(FullFormatHelp);

        var subRoles = player.GetCustomSubRoles();
        if (CustomRoles.Ntr.IsExist() && !subRoles.Contains(CustomRoles.Lovers) && !player.Is(CustomRoles.GM) && !player.Is(CustomRoles.Ntr))
        {
            subRoles.Add(CustomRoles.Lovers);
        }

        foreach (var subRole in subRoles)
        {
            builder.AppendFormat("<size={0}>{1}\n", FirstHeaderSize, Translator.GetRoleString(subRole.ToString()).Color(subRole.GetRoleInfo().RoleColor.ToReadableColor()));
            builder.AppendFormat("<size={0}>{1}\n", BodySize, subRole.GetRoleInfo().Description.Description);
        }

        return builder.ToString();
    }

    public const string FirstHeaderSize = "130%";
    public const string SecondHeaderSize = "100%";
    public const string BodySize = "70%";
    public const string BlankLineSize = "30%";
}
