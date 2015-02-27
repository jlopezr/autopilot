using System;
using System.Linq;

namespace XPlane
{
	/// <summary>
	/// Utilities to build X-plane packets
	/// </summary>
	public static class XplanePacketGenerator
	{
		/// <summary>
		/// Genera las cabeceras del paquete de datos del X-Plane
		/// </summary>
		/// <param name="data">Array de Bytes generados por el metodo CreateData</param>
		/// <returns>Devuelve el array de byte que forman el paquete junto con las cabeceras</returns>
		public static byte[] CreatePacket(byte[] data)
		{
			byte[] packet = new byte[5];

			// 0,1,2,3 are ASCII Code for "DATA". It tells Xplane that this program is trying to write data into it.
			packet[0] = (byte)'D';
			packet[1] = (byte)'A';
			packet[2] = (byte)'T';
			packet[3] = (byte)'A';

			// This is a random number. Could also be used if more than one plane is being controlled through
			// Xplane. In such a case the value of this should be from 0-9 according to the number of the
			// plane being controlled.
			packet[4] = 0;

			return packet.Concat(data).ToArray();
		}

		/// <summary>
		/// Genera la estructura de datos de paquete de entrada de datos del XPLANE
		/// </summary>
		/// <param name="varnumber">Numero de registro que se quiere modificar</param>
		/// <param name="val1">Nuevo valor para el campo 1. Ver interfaz del X-Plane</param>
		/// <param name="val2">Nuevo valor para el campo 2. Ver interfaz del X-Plane</param>
		/// <param name="val3">Nuevo valor para el campo 3. Ver interfaz del X-Plane</param>
		/// <param name="val4">Nuevo valor para el campo 4. Ver interfaz del X-Plane</param>
		/// <param name="val5">Nuevo valor para el campo 5. Ver interfaz del X-Plane</param>
		/// <param name="val6">Nuevo valor para el campo 6. Ver interfaz del X-Plane</param>
		/// <param name="val7">Nuevo valor para el campo 7. Ver interfaz del X-Plane</param>
		/// <param name="val8">Nuevo valor para el campo 8. Ver interfaz del X-Plane</param>
		/// <returns>Devuelve el array de byte que forman el paquete sin las cabeceras</returns>
		public static byte[] CreateData(
			byte varnumber, float val1, float val2, float val3, float val4, float val5, float val6, float val7, float val8)
		{
			byte[] aux = new byte[4];
			aux[0] = varnumber;
			aux[1] = 0;
			aux[2] = 0;
			aux[3] = 0;

			byte[] packet = aux.Concat(BitConverter.GetBytes(Convert.ToSingle(val1))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val2))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val3))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val4))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val5))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val6))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val7))).ToArray();
			packet = packet.Concat(BitConverter.GetBytes(Convert.ToSingle(val8))).ToArray();

			return packet;
		}

		/// <summary>
		/// Genera el paquete para cambiar los valores de elevator, throttle, ruder, aileron
		/// </summary>
		/// <param name="throttle">Valor de throttle entre 0 y 1</param>
		/// <param name="aileron">Valor de aileron entre 1 y -1</param>
		/// <param name="ruder">Valor de ruder entre 1 y -1</param>
		/// <param name="elevator">Valor de elevator entre 1 y -1</param>
		/// <returns>Devuelve el array de byte que forman el paquete junto con las cabeceras</returns>
		public static byte[] JoystickPacket(float throttle, float aileron, float ruder, float elevator)
		{
			byte[] data = XplanePacketGenerator.CreateData((byte)XplanePacketsId.Joystick, elevator, aileron, ruder, -999, -999, -999, -999, -999);
			data = data.Concat(XplanePacketGenerator.CreateData((byte)XplanePacketsId.Throttle, throttle, throttle, -999, -999, -999, -999, -999, -999)).ToArray();

			return XplanePacketGenerator.CreatePacket(data);
		}
	}
}