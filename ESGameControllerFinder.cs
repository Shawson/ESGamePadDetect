using SharpDX.DirectInput;
using System;
using System.Linq;

namespace ESGamePadDetect
{
    public class ESGameControllerFinder
    {
        public ESGameControllerFinder()
        {

        }

        public GameControllerIdentifiers FindController(string deviceName, string deviceGUID)
        {
            int joystickIndex = -1;
            /*
                        deviceGUID = "78696e70757401000000000000000000";
                        deviceName = "#1";
                        deviceGUID = "a0053232000000000000504944564944";
                        */

            var guidByteArray = FromHex(deviceGUID);
            GameControllerIdentifiers controllerIds = null;

            if (System.Text.Encoding.UTF8.GetString(guidByteArray).StartsWith("xinput"))
            {
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

                controllerIds = GetXInputDevice(joystickIndex);
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

        private static GameControllerIdentifiers GetXInputDevice(int index)
        {
            using (var directInput = new DirectInput())
            {
                var joystickGuid = Guid.Empty;

                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = deviceInstance.InstanceGuid;

                    using (var js = new Joystick(directInput, joystickGuid))
                    {
                        if (js.Properties.JoystickId == (index - 1))
                        {
                            return new GameControllerIdentifiers
                            {
                                PID = js.Properties.ProductId,
                                VID = js.Properties.VendorId,
                                DeviceName = js.Properties.ProductName
                            };

                            /*
                            if (js.Properties.InterfacePath.ToLower().Contains("&ig_"))
                                Console.WriteLine("XINPUT Controller");
                                */
                        }
                    }

                }
            }

            return null;
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
