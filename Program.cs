using System;
using NDesk.Options;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace ESGamePadDetect
{
    class Program
    {
        static int Main(string[] args)
        {
            string type = string.Empty,
                deviceName = string.Empty,
                deviceGUID = string.Empty;

            bool printHelp = false,
                listMode = false;

            var p = new OptionSet() {
                { "l|list", "list all connected devices", v => listMode = true },
                { "n|deviceName=", "device name from es_input.cfg file", v => deviceName = v },
                { "g|deviceGUID=", "device guid from es_input.cfg file", v => deviceGUID = v },
                { "h|?|help", "Get help", v => printHelp = true }
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                SerialiseAndWrite(new BaseCommandLineResponse<GameControllerIdentifiers>
                {
                    ResponseCode = -1,
                    ResponseMessage = e.Message
                });
                return -1;
            }

            if (printHelp)
            {
                Console.WriteLine("https://github.com/Shawson/ESGamePadDetect");
                Console.WriteLine("Usage: ESGamePadDetect.exe n=\"My Controller\" g=a0053232000000000000504944564944");
                Console.WriteLine("Outputs in XML");
                p.WriteOptionDescriptions(Console.Out);
                return 1;
            }

            if (listMode)
            {
                var controllerIdCollection = new ESGameControllerFinder()
                    .GetXInputDevices();

                SerialiseAndWrite(new BaseCommandLineResponse<List<GameControllerIdentifiers>>
                {
                    ResponseCode = 0,
                    ResponseMessage = "Ok",
                    Data = controllerIdCollection
                });
                return 0;
            }

            if (deviceName == string.Empty)
            {
                SerialiseAndWrite(new BaseCommandLineResponse<GameControllerIdentifiers>
                {
                    ResponseCode = -2,
                    ResponseMessage = "Device name not supplied"
                });
                return -2;
            }


            if (deviceGUID == string.Empty)
            {
                SerialiseAndWrite(new BaseCommandLineResponse<GameControllerIdentifiers>
                {
                    ResponseCode = -3,
                    ResponseMessage = "Device GUID not supplied"
                });
                return -3;
            }

            var controllerIds = new ESGameControllerFinder()
                .FindController(deviceName, deviceGUID);

            if (controllerIds == null)
            {
                SerialiseAndWrite(new BaseCommandLineResponse<GameControllerIdentifiers>
                {
                    ResponseCode = -10,
                    ResponseMessage = "Unable to match controller"
                });
                return -10;
            }

            SerialiseAndWrite(new BaseCommandLineResponse<GameControllerIdentifiers>
            {
                ResponseCode = 0,
                ResponseMessage = "Ok",
                Data = controllerIds
            });
            return 0;
        }

        static void SerialiseAndWrite<T>(T obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, obj);
                    xml = sww.ToString();
                }
            }

            Console.Write(xml);
        }
    }
}
