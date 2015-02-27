using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPlane
{
    public class Telemetrysim
    {
        public void Start()
        {
            XplanePacketsId.Load(XplaneVersion.Xplane10);
            XplaneConnection connection = new XplaneConnection();
            XplaneParser parser = new XplaneParser(connection);

            connection.OpenConnections();
            parser.Start();

            //connection.SendPacket(XplanePacketGenerator.JoystickPacket(0, 0, 0, 0));
        }
            
    }
}
