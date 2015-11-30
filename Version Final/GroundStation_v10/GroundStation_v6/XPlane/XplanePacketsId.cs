using System;

namespace XPlane
{
	public enum XplaneVersion
	{
		XPlane9,
		Xplane10
	}

	/// <summary>
	/// By default all xplane parameters are compatible with X-PLANE 10
	/// </summary>
	public class XplanePacketsId
	{
		public static byte TimeFactors = 0;
		public static byte Times = 1;
		public static byte Speeds = 3;
		public static byte Accelerations = 4;
        public static byte Atmosphere = 6;
		public static byte SystemPressures = 7;
		public static byte Joystick = 8;
		public static byte FlapsSpeedBrake = 13;
		public static byte GearBrake = 14;
		public static byte AngularMoments = 15;
		public static byte AngularVelocities = 16;
		public static byte AngularAccelerations = 16;
		public static byte Angles = 17;
        public static byte AnglesAoA = 18;
		public static byte Position = 20;
		public static byte VectorVelocities = 21;
		public static byte Throttle = 25;
		public static byte ActThrottle = 26;
		public static byte Reverse = 27;
		public static byte EnginePower = 34;
		public static byte EngineThrust = 35;
		public static byte EngineRPM = 37;
        public static byte PropPitch = 39;
		public static byte FuelFlow = 45;
		public static byte OilPressure = 49;
		public static byte OlitTem = 50;
		public static byte FuelPressure = 51;
		public static byte GeneratorAmperage = 52;
		public static byte BatteryAmperage = 53;
		public static byte BatteryVoltage = 54;
		public static byte FuelPump = 55;
		public static byte BatteryONOFF = 57;
		public static byte GeneratorONOFF = 58;
		public static byte FuelWeights = 62;
		public static byte GearForce = 66;
		public static byte GearDeployment = 67;
		public static byte Autopilot = 108;
		public static byte APModes = 117;
		public static byte APValues = 118;
		public static byte Miscellaneous = 112;

		public static void Load(XplaneVersion version)
		{
			if (version == XplaneVersion.XPlane9)
			{
				TimeFactors = 0;
				Times = 1;
				Speeds = 3;
				Accelerations = 4;
				SystemPressures = 7;
				Joystick = 8;
				FlapsSpeedBrake = 13;
				GearBrake = 14;
				AngularMoments = 15;
				AngularAccelerations = 16;
				AngularVelocities = 17;
				Angles = 18;
				Position = 20;
				VectorVelocities = 21;
				Throttle = 25;
				Reverse = 27;
				EnginePower = 34;
				EngineThrust = 35;
				EngineRPM = 37;
                PropPitch = 39;
				FuelFlow = 45;
				OilPressure = 49;
				OlitTem = 50;
				FuelPressure = 51;
				GeneratorAmperage = 52;
				BatteryAmperage = 53;
				BatteryVoltage = 54;
				FuelPump = 55;
				BatteryONOFF = 57;
				GeneratorONOFF = 58;
				FuelWeights = 62;
				GearForce = 66;
				GearDeployment = 67;
				Autopilot = 108;
				APModes = 117;
				APValues = 118;
				Miscellaneous = 112;
			}
			else
			{
				TimeFactors = 0;
				Times = 1;
				Speeds = 3;
				Accelerations = 4;
                Atmosphere = 6;
				SystemPressures = 7;
				Joystick = 8;
				FlapsSpeedBrake = 13;
				GearBrake = 14;
				AngularMoments = 15;
				AngularVelocities = 16;
				Angles = 17;
                AnglesAoA = 18;
				Position = 20;
				VectorVelocities = 21;
				Throttle = 25;
				ActThrottle = 26;
				Reverse = 27;
				EnginePower = 34;
				EngineThrust = 35;
				EngineRPM = 37;
                PropPitch = 39;
				FuelFlow = 45;
				OilPressure = 49;
				OlitTem = 50;
				FuelPressure = 51;
				GeneratorAmperage = 52;
				BatteryAmperage = 53;
				BatteryVoltage = 54;
				FuelPump = 55;
				BatteryONOFF = 57;
				GeneratorONOFF = 58;
				FuelWeights = 62;
				GearForce = 66;
				GearDeployment = 67;
				Autopilot = 108;
				APModes = 117;
				APValues = 118;
				Miscellaneous = 112;
			}
		}
	}
}
