
void printdata(void)
{    
      unsigned long ulRoll = convert_to_dec(ToDeg(roll)+180.0);
      unsigned long ulPitch = convert_to_dec(ToDeg(pitch)+180.0);
      unsigned long ulYaw = convert_to_dec(ToDeg(yaw)+180.0);
      byte euler[12];
      euler[0] = (byte)((ulRoll >> 24) & 0XFF) ;
      euler[1] = (byte)((ulRoll >> 16) & 0XFF) ;
      euler[2] = (byte)((ulRoll >> 8) & 0XFF);
      euler[3] = (byte)((ulRoll & 0XFF));
      
      euler[4] = (byte)((ulPitch >> 24) & 0XFF) ;
      euler[5] = (byte)((ulPitch >> 16) & 0XFF) ;
      euler[6] = (byte)((ulPitch >> 8) & 0XFF);
      euler[7] = (byte)((ulPitch & 0XFF));
      
      euler[8] = (byte)((ulYaw >> 24) & 0XFF) ;
      euler[9] = (byte)((ulYaw >> 16) & 0XFF) ;
      euler[10] = (byte)((ulYaw >> 8) & 0XFF);
      euler[11] = (byte)((ulYaw & 0XFF));
      Serial.write(euler, 12);
}

long convert_to_dec(float x)
{
  return x*10000;
}

unsigned long convert_to_dec_unsig(float x)
{
  return x*10000;
}

