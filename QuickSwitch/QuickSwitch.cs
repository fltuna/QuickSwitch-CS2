using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;

namespace QuickSwitch;

public class QuickSwitch: BasePlugin
{
    public override string ModuleAuthor => "faketuna";
    public override string ModuleName => "QuickSwitch";
    public override string ModuleDescription => "Simply removes attack cooldown when switching to knife.";
    public override string ModuleVersion => "0.0.2";

    public readonly FakeConVar<bool> IsEnabled = new("css_quick_switch_enabled", "Enables the quick switch feature.", true);
    
    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventItemEquip>(OnWeaponSelectPost, HookMode.Post);
    }

    public override void Unload(bool hotReload)
    {
        DeregisterEventHandler<EventItemEquip>(OnWeaponSelectPost, HookMode.Post);
    }
    
        
    private HookResult OnWeaponSelectPost(EventItemEquip @event, GameEventInfo info)
    {
        if (!IsEnabled.Value)
            return HookResult.Continue;
        
        var player = @event.Userid;

        if (player == null)
            return HookResult.Continue;

        if (player.PlayerPawn.Value?.WeaponServices?.ActiveWeapon == null)
            return HookResult.Continue;

        var playerWeapon = player.PlayerPawn.Value.WeaponServices.ActiveWeapon.Value;
        
        if (playerWeapon == null)
            return HookResult.Continue;
        
        if (!playerWeapon.DesignerName.Contains("knife"))
            return HookResult.Continue;
        
        var nextAttackTick = Server.TickCount;

        playerWeapon.NextPrimaryAttackTick = nextAttackTick;
        playerWeapon.NextSecondaryAttackTick = nextAttackTick;

        Utilities.SetStateChanged(playerWeapon, "CBasePlayerWeapon", "m_nNextPrimaryAttackTick");
        Utilities.SetStateChanged(playerWeapon, "CBasePlayerWeapon", "m_nNextSecondaryAttackTick");
        return HookResult.Continue;
    }
}