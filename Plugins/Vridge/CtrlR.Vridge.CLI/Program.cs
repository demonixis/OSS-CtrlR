using CtrlR.Vridge;
using System;
using System.Collections.Generic;

namespace OSSCtrlR
{
    class Program
    {
        private const string SerialPortName = "SerialPortName";
        private const string LeftHandName = "LeftHand";

        static void Main(string[] args)
        {
            var parameters = ParseCliParameters(ref args);
            var portName = string.Empty;
            var left = true;

            if (parameters.ContainsKey(SerialPortName))
                portName = parameters[SerialPortName];

            if (parameters.ContainsKey(LeftHandName))
                left = parameters[LeftHandName] == "True";

            var vridgeSocketReader = new VridgeSocketReader(portName, left);
            vridgeSocketReader.Connect();
            Console.ReadKey();
            vridgeSocketReader.Shutdown();
        }

        private static Dictionary<string, string> ParseCliParameters(ref string[] args)
        {
            var results = new Dictionary<string, string>();
            string[] tmp = null;

            for (var i = 0; i < args.Length; i++)
            {
                tmp = args[i].Split('=');

                if (tmp.Length == 2)
                {
                    var key = tmp[0].Trim();
                    var value = tmp[1].Trim();

                    if (results.ContainsKey(key))
                        results[key] = value;
                    else
                        results.Add(key, value);
                }
            }

            return results;
        }

        private static void SetInt(string str, ref int target)
        {
            if (int.TryParse(str.Trim(), out int value))
                target = value;
        }

        private static void SetBool(string str, ref bool target)
        {
            if (bool.TryParse(str.Trim(), out bool value))
                target = value;
        }
    }
}
