clear all;
close all;

%% Change commas to dots
Data = fileread('adc.txt');
Data = strrep(Data, ',', '.');
FID = fopen('adc.txt', 'w');
fwrite(FID, Data, 'char');
fclose(FID);

Data = fileread('imuEuler.txt');
Data = strrep(Data, ',', '.');
FID = fopen('imuEuler.txt', 'w');
fwrite(FID, Data, 'char');
fclose(FID);

Data = fileread('gpsPos.txt');
Data = strrep(Data, ',', '.');
FID = fopen('gpsPos.txt', 'w');
fwrite(FID, Data, 'char');
fclose(FID);

clearvars Data FID;

%% Read files
fileID1 = fopen('adc.txt','r');
%formatSpec = '%f';
%A = fscanf(fileID,formatSpec);
columnsADC = textscan(fileID1,'%f %f %f %f %f %f'); 
fclose(fileID1);

fileID2 = fopen('gpsPos.txt','r');
%formatSpec = '%f';
%A = fscanf(fileID,formatSpec);
columnsGPS = textscan(fileID2,'%f %f %f %f %f %f %f');
fclose(fileID2);

fileID3 = fopen('imuEuler.txt','r');
%formatSpec = '%f';
%A = fscanf(fileID,formatSpec);
columnsIMU = textscan(fileID3,'%f %f %f %f'); 
fclose(fileID3);

%% ADC DATA
timeADC = columnsADC{1};
TAS = columnsADC{5};
ALT = columnsADC{6};


%% GPS DATA
timeGPS = columnsGPS{1};
LAT = columnsGPS{2};
LON = columnsGPS{3};
GSPEED = columnsGPS{4};
TRACK = columnsGPS{5};

%% IMU DATA
timeIMU = columnsIMU{1};
ROLL = columnsIMU{2};
PITCH = columnsIMU{3};
HDG = columnsIMU{4};

plot(timeADC, TAS, 'r')
hold on;
plot(timeADC, ALT, 'b')

plot(timeGPS, GSPEED, 'k')

figure(2)

plot(timeIMU, ROLL, 'r')
hold on;
plot(timeIMU, PITCH, 'b')




