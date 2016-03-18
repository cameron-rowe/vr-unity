using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using NLua;

public class CaVR : MonoBehaviour {

	public static bool Terminated { get; private set; }
	private Config config;
	private VRPNInputManager inputManger;

	private Thread networkThread;
	// Use this for initialization
	void Start() {
		Terminated = false;
		config = FindObjectOfType<Config>();
		inputManger = FindObjectOfType<VRPNInputManager>();

		if(config.IsMaster) {
			var machines = config.GetLuaStateValue("machines") as LuaTable;
			foreach(var machineKey in machines.Keys) {
				var machine = machineKey.ToString();
				if(machine == "master") {
					continue;
				}

				var ssh = machines[machineKey + ".ssh"].ToString();

				var cwd = Environment.CurrentDirectory;

				var command = new StringBuilder();
				command.Append("cd ").Append(cwd).Append(" &&");

				foreach(var arg in Environment.GetCommandLineArgs()) {
					command.Append(' ');
					command.Append(arg);
				}

				command.Append(" --cavr_master=").Append(config.GetLuaStateValue("machines.master.address").ToString());

				var sshOptions = new ProcessStartInfo("ssh", command.ToString());
				var process = new Process();
				process.StartInfo = sshOptions;

				if(!process.Start()) {
					UnityEngine.Debug.LogError("Unable to launch SSH");
				}

				Thread.Sleep(1000 * 10);
			}
		}

		networkThread = new Thread(() => {
			while(!Terminated) {
				if(config.IsMaster) {
					Pubsub();
				}	

				SyncInput();

				if(!config.IsMaster) {
					Pubsub();
				}
			}
		});

		networkThread.Start();
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	void Destroy() {
		Terminated = true;
	}

	private void Pubsub() {
		
	}

	private void SyncInput() {
		inputManger.Sync();
	}
}
