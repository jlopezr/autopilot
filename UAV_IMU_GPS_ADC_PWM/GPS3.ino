void GPSSetup()
{
  Serial2.begin(38400);
}

unsigned short lengthMess = 0;
char message[7];

void GPSLoop()
{
  byte aux[200];
  //lengthMess = 0;
  while(true)
  {
    if(!Serial2.available())
    {
      return;
    }
    //while(!Serial2.available()){}
    byte b = Serial2.read();
    //Serial.println((char)b);
    aux[lengthMess] = b;
    lengthMess++;
    if(b == (byte)10)
    {
      /*while(!Serial2.available()){}
      b = Serial2.read();
      while(!Serial2.available()){}
      b = Serial2.read();
      while(!Serial2.available()){}
      b = Serial2.read();*/
      break;
    }
  }
  //Serial.println(lengthMess);
  //Serial.println("He sortit del while(true)");
  /*for(int i = 0; i < 6; i++)
  {
    char c = (char)aux[i];
    message[i] = c;
  }
  message[6] = '\0';*/
  //Serial.println(message);
  /*if(message[3] == 'R' && message[4] == 'M' && message[5] == 'C')
  {*/
    //Serial.println("He entrat");
    byte ansGPS[lengthMess + 6];
    ansGPS[0] = ((byte)1);
    ansGPS[1] = ((byte)1);
    ansGPS[2] = ((byte)1);
    ansGPS[3] = ((byte)8);
    ansGPS[4] = ((byte)lengthMess);
    ansGPS[5] = SetTime();
    for(int i = 0; i < lengthMess; i++)
    {
      ansGPS[6+i] = aux[i];
    }
    Serial.write(ansGPS, lengthMess+6);
  /*}*/
  lengthMess = 0;
}
