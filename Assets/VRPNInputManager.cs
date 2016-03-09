using UnityEngine;
using System.Collections.Generic;

public class VRPNInputManager : MonoBehaviour
{
    public Dictionary<string, string> ButtonMap = new Dictionary<string, string>();
    public Dictionary<string, string> AnalogMap = new Dictionary<string, string>();
    public Dictionary<string, string> SixdofMap = new Dictionary<string, string>();

    public Dictionary<string, bool> ButtonValues = new Dictionary<string, bool>();
    public Dictionary<string, float> AnalogValues = new Dictionary<string, float>();
    public Dictionary<string, Sixdof> SixdofValues = new Dictionary<string, Sixdof>();

    private Config config;
    // Use this for initialization
    void Start() {

        ButtonMap.Add("clear", "WiiMote0[0]"); // Home button
        ButtonMap.Add("paint", "WiiMote0[3]"); // A button
        ButtonMap.Add("rotate", "WiiMote0[16]"); // nunchuck-z button
        ButtonMap.Add("boost", "WiiMote0[17]"); // nunchuck-c button

        AnalogMap.Add("x", "WiiMote0[21]"); // nunchuck-y
        AnalogMap.Add("z", "WiiMote0[22]"); // nunchuck-z

        SixdofMap.Add("wand", "WiiMote0[0]"); // WiiMote0
        SixdofMap.Add("view", "ShortGlasses[0]"); // short glasses
        
        foreach(var button in ButtonMap.Values) {
            ButtonValues.Add(button, false);
        }

        foreach(var analog in AnalogMap.Values) {
            AnalogValues.Add(analog, 0.0f);
        }

        foreach(var sixdof in SixdofMap.Values) {
            SixdofValues.Add(sixdof, new Sixdof());
        }

        config = FindObjectOfType<Config>();
    }

    // Update is called once per frame
    void Update() {
        string device;
        int channel;

        foreach(var button in ButtonMap.Values) {
            GetDeviceAndChannel(button, out device, out channel);
            ButtonValues[button] = config.GetButtonValue(device, channel);
        }

        foreach(var analog in AnalogMap.Values) {
            GetDeviceAndChannel(analog, out device, out channel);
            AnalogValues[analog] = config.GetAnalogValue(device, channel);
        }

        foreach(var sixdof in SixdofMap.Values) {
            GetDeviceAndChannel(sixdof, out device, out channel);
            SixdofValues[sixdof] = config.GetSixdofValue(device, channel);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public bool GetButtonValue(string name) {
        return ButtonValues[ButtonMap[name]];
    }

    public float GetAnalogValue(string name) {
        return AnalogValues[AnalogMap[name]];
    }

    public Sixdof GetSixdofValue(string name) {
        return SixdofValues[SixdofMap[name]];
    }

    private static void GetDeviceAndChannel(string addr, out string device, out int channel) {
        var split = addr.Split('[');
        device = split[0].Replace("]", string.Empty);
        var channelStr = split[1].Replace("]", string.Empty);
        channel = int.Parse(channelStr);
    }
}
