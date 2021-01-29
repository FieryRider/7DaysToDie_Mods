using System;
using System.Collections.Generic;

public class ConsoleCmdWeaponFov : ConsoleCmdAbstract
{
    public override bool IsExecuteOnClient{
        get
        {
            return true;
        }
    }

    public override int DefaultPermissionLevel
    {
        get
        {
            return 0;
        }
    }

    public override bool AllowedInMainMenu
    {
        get
        {
            return true;
        }
    }

    public override string[] GetCommands()
    {
        return new string[]
        {
            "wfov"
        };
    }

    public override string GetDescription()
    {
        return "Weapon camera field of view / Hands distance from the camera";
    }

    public override string GetHelp()
    {
        return "";
    }

    public override void Execute(List<string> _params, global::CommandSenderInfo _senderInfo)
    {
        CustomPrefs customPrefs = CustomPrefs.GetCustomPrefs();

        int num;
        if (_params.Count == 0)
        {
            try
            {
                global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output(customPrefs.GetCustomPref<int>("WeaponFov").ToString());
            }
            catch (ArgumentException)
            {
                global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output("WeaponFov is not set");
            }
        }
        else if (_params.Count == 1 && int.TryParse(_params[0], out num))
        {
            customPrefs.SetCustomPref<int>("WeaponFov", num);
            global::SingletonMonoBehaviour<global::SdtdConsole>.Instance.Output("Set Weapon/Hands FOV to " + num);
        }
    }
    
}
