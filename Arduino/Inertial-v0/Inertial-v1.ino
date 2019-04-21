#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"
#include "IMU.h"

#define MaxSensors 2
#define PinMuxS0 8;
#define PinMuxS1 9;
#define PinMuxS2 10;
 
IMU sensors[MaxSensors];
Quaternion quaternion;
int index;

const int muxChannels[16][3]={
  {0, 0, 0}, //channel 0
  {0, 0, 1}, //channel 1
  {0, 1, 0}, //channel 2
  {0, 1, 1}, //channel 3
  {1, 0, 0}, //channel 4
  {1, 0, 1}, //channel 5
  {1, 1, 0}, //channel 6
  {1, 1, 1}, //channel 7
  {0, 0, 0}, //channel 8
  {0, 0, 1}, //channel 9
  {0, 1, 0}, //channel 10
  {0, 1, 1}, //channel 11
  {1, 0, 0}, //channel 12
  {1, 0, 1}, //channel 13
  {1, 1, 0}, //channel 14
  {1, 1, 1}  //channel 15
};

void SetMuxChannel(int channel)
{
  digitalWrite(PinMuxS0, muxChannels[channel][0]);
  digitalWrite(PinMuxS1, muxChannels[channel][1]);
  digitalWrite(PinMuxS2, muxChannels[channel][2]);
}

void setup()
{ 
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
	Wire.begin();
	TWBR = 24; // 400kHz I2C clock (200kHz if CPU is 8MHz)
#elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
	Fastwire::setup(400, true);
#endif
	Serial.begin(115200);
	while (!Serial); // wait for Leonardo enumeration, others continue immediately

  pinMode(PinMuxS0, OUTPUT);
  pinMode(PinMuxS1, OUTPUT);
  pinMode(PinMuxS2, OUTPUT);

	// Setup the multiplexer
  for (index = 0; index < MaxSensors; index++)
  {
    SetMuxChannel(index);
    sensors[index].Initialize();
  }
}

void loop()
{
	for (index = 0; index < MaxSensors; index++)
  {
    SetMuxChannel(index);
    sensors[index].Update(quaternion, index);
  }
}

