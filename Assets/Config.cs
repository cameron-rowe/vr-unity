using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using NLua;

public delegate bool ButtonCallback(int channel);
public delegate float AnalogCallback(int channel);
public delegate Sixdof SixdofCallback(int channel);

public class Config : MonoBehaviour
{
    private Lua state = new Lua();

    private Dictionary<string, ButtonCallback> buttonCallbacks = new Dictionary<string, ButtonCallback>();
    private Dictionary<string, AnalogCallback> analogCallbacks = new Dictionary<string, AnalogCallback>();
    private Dictionary<string, SixdofCallback> sixdofCallbacks = new Dictionary<string, SixdofCallback>();

    private Dictionary<string, LuaTable> machines = new Dictionary<string, LuaTable>();

    void Start() {
        state.DoString(string.Format("HOSTNAME = '{0}'", System.Environment.MachineName));

        Debug.Log(string.Format("HOSTNAME = '{0}'", state["HOSTNAME"]));

        //System.IO.File.WriteAllText("log.txt", string.Format("{0}  {1}  {2}  {3}", Application.dataPath, Application.persistentDataPath, Application.streamingAssetsPath, Application.temporaryCachePath));
        state.DoFile(Application.streamingAssetsPath + "/example.lua");

        var vrpn = state["vrpn"] as LuaTable;

        //System.IO.File.WriteAllText("log2.txt", string.Format("{0}  {1}", State["HOSTNAME"], vrpn));

        var buttons = vrpn["buttons"] as LuaTable;
        var analogs = vrpn["analogs"] as LuaTable;
        var sixdofs = vrpn["sixdofs"] as LuaTable;

        if(buttons != null) {
            foreach(var b in buttons.Values) {
                var button = b.ToString();
                Debug.Log("Button: " + button);
                var device = button.Split('@')[0];

                ButtonCallback callback = (int channel) => {
                    return VRPN.vrpnButton(button, channel);
                };

                buttonCallbacks.Add(device, callback);
            }
        }

        if(analogs != null) {
            foreach(var a in analogs.Values) {
                var analog = a.ToString();
                Debug.Log("Analog: " + analog);

                var device = analog.Split('@')[0];

                AnalogCallback callback = (int channel) => {
                    return (float)VRPN.vrpnAnalog(analog, channel);
                };

                analogCallbacks.Add(device, callback);
            }
        }

        if(sixdofs != null) {
            foreach(var s in sixdofs.Values) {
                var sixdof = s.ToString();
                Debug.Log("Sixdof: " + sixdof);

                var device = sixdof.Split('@')[0];

                SixdofCallback callback = (int channel) => {
                    var pos = VRPN.vrpnTrackerPos(sixdof, channel);
                    var rot = VRPN.vrpnTrackerQuat(sixdof, channel);

                    pos.z *= -1;

                    return new Sixdof {
                        Position = pos,//new Vector3(pos.x, pos.y, -pos.z),
                        Rotation = Quaternion.Inverse(rot),//new Quaternion(-rot.x, rot.z, rot.y, rot.w)
                    };
                };

                sixdofCallbacks.Add(device, callback);
            }
        }

        // machines
        var luaMachines = state["machines"] as LuaTable;
        //var self = machines["self"] as LuaTable;

        foreach(string machine in luaMachines.Keys) {
            machines[machine] = luaMachines[machine] as LuaTable;
        }
    }

    public bool GetButtonValue(string device, int channel) {
        return buttonCallbacks[device](channel);
    }

    public float GetAnalogValue(string device, int channel) {
        return analogCallbacks[device](channel);
    }

    public Sixdof GetSixdofValue(string device, int channel) {
        return sixdofCallbacks[device](channel);
    }

    public Dictionary<string, object> GetMachineConfiguration(string machine) {
        return state.GetTableDict(machines[machine]).ToDictionary(v => v.Key.ToString(), v => v.Value);
    }
}
