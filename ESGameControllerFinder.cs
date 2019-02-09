using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESGamePadDetect
{
    public class ESGameControllerFinder
    {
        public ESGameControllerFinder()
        {

        }

        public List<GameControllerIdentifiers> GetXInputDevices(int? index = null)
        {
            var controllers = new List<GameControllerIdentifiers>();

            using (var directInput = new DirectInput())
            {
                var joystickGuid = Guid.Empty;

                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = deviceInstance.InstanceGuid;

                    using (var js = new Joystick(directInput, joystickGuid))
                    {
                        if (index == null || js.Properties.JoystickId == (index - 1))
                        {
                            controllers.Add(new GameControllerIdentifiers
                            {
                                PID = js.Properties.ProductId,
                                VID = js.Properties.VendorId,
                                DeviceName = js.Properties.ProductName,
                                ControllerIndex = js.Properties.JoystickId
                            });

                            /*
                            if (js.Properties.InterfacePath.ToLower().Contains("&ig_"))
                                Console.WriteLine("XINPUT Controller");
                                */
                        }
                    }

                }
            }

            return controllers;
        }

        public GameControllerIdentifiers FindController(string deviceName, string deviceGUID)
        {
            /*
            deviceGUID = "78696e70757401000000000000000000";
            deviceName = "#1";
            deviceGUID = "a0053232000000000000504944564944";
            */

            var guidByteArray = FromHex(deviceGUID);
            GameControllerIdentifiers controllerIds = null;

            if (System.Text.Encoding.UTF8.GetString(guidByteArray).StartsWith("xinput"))
            {
                int joystickIndex = 1; // set a default - if there is a no player number supplied, we can guess it's port 0
                
                if (deviceName.Contains("#"))
                {
                    var strJoyStickIndex = deviceName.Split('#')
                        .ToList()
                        .Last();

                    if (!int.TryParse(strJoyStickIndex, out joystickIndex))
                    {
                        return null;
                    }
                }

                controllerIds = GetXInputDevices(joystickIndex)
                    .FirstOrDefault();
            }
            else
            {
                // this is not an xinput controller!- all the info was given to us up fronf and just needs extracting
                controllerIds = new GameControllerIdentifiers
                {
                    VID = BitConverter.ToInt16(guidByteArray, 0),
                    PID = BitConverter.ToInt16(guidByteArray, 2),
                    DeviceName = deviceName
                };
            }

            return controllerIds;
        }

        private static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
