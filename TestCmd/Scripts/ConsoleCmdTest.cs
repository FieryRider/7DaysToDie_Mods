using System;
using System.Collections.Generic;

public class ConsoleCmdTest : ConsoleCmdAbstract
{
	public override bool IsExecuteOnClient
	{
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
			"test"
		};
	}

	public override string GetDescription()
	{
		return "Test command";
	}

	public override string GetHelp()
	{
		return "Test help";
	}

	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Working");
	}
}
