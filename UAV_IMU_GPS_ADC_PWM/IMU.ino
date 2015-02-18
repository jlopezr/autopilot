void IMUSetup()
{
  Serial1.begin(19200); //IMU Port
}

byte ansIMU[17];
unsigned short imuCount = 0;

void IMULoop(){
  while (true)
  {
    if(!Serial1.available()) return;  
    byte b = Serial1.read(); 
    ansIMU[5+imuCount] = b;
    imuCount++;
    if(imuCount == 12)
      break;
  }
  ansIMU[0] = 1; //IMU Header 1110
  ansIMU[1] = 1;
  ansIMU[2] = 1;
  ansIMU[3] = 0;
  ansIMU[4] = SetTime();
  //Output
  Serial.write(ansIMU, 17);
  imuCount = 0;
}
