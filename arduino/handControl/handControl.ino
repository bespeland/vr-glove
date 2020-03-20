#include <SparkFun_ADS1015_Arduino_Library.h>
#include <Wire.h>

ADS1015 fingerSensor; //flex sensor 1/2
ADS1015 fingerSensorOther; //flex sensor 3/4

float connected;


void setup() {
  
  Wire.begin();
  Serial.begin(115200);
  connected = false;
  
  while(!Serial){
    
  }

  
  if (fingerSensor.begin() == false) {
     while (1);
  } 
  
  
  if (fingerSensorOther.begin(ADS1015_ADDRESS_VDD, Wire) == false) {
     while (1);
  } 
  fingerSensor.setGain(ADS1015_CONFIG_PGA_TWOTHIRDS); // Gain of 2/3 to works well with flex glove board voltage swings
  fingerSensorOther.setGain(ADS1015_CONFIG_PGA_TWOTHIRDS);
}

double normalize_data(double data, int finger){
  double max;
  double min;
  switch(finger){
    case 0:
      min = 840;
      max = 950;
      break;
    case 1:
      min = 840;
      max = 915;
      break;
    case 2:
      min = 860;
      max = 950;
      break;
    case 3:
      min = 800;
      max = 923;
      break;  
    default:
      min = 0;
      max = 100;
      break;
  }
  if(data < min){
    data = min;
  }
  if(data > max){
    data = max;
  }
  
  return 100.0-(100.0*(data-min))/(max-min);
}

double getFinger(int finger){
  double data = 0;
  switch(finger){
    case 0:
      data = fingerSensor.getAnalogData(finger);
      break;
    case 1:
      data = fingerSensor.getAnalogData(finger);
      break;
    case 2:
      data = fingerSensorOther.getAnalogData(0);
      break;
    case 3:
      data = fingerSensorOther.getAnalogData(1);
      break;
    default:
      return -1.0;
      break;
  }
  return data;
  
}

void loop() {

  if(connected){
    int numFingers = 4;
    for(int i = 0; i < numFingers; i++){
      double data;
      data = getFinger(i);
      data = normalize_data(data, i);
      Serial.print(data);
      if(i < numFingers-1){
        Serial.print(',');
      }
    }
    Serial.print("\r\n");

    delay(10);
    
  } else {
	 //wait for unity to send character before streaming readings
    if(Serial.available() > 0){
      Serial.read();
      connected = true;
    }
  }

  
}
