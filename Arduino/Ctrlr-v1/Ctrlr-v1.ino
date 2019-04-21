#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"
 
#define ANALOG_X_PIN A0
#define ANALOG_Y_PIN A1
#define ANALOG_TRIGGER_PIN A2
#define ANALOG_GRIP_PIN A3
#define DIGITAL_HOME_PIN 4
#define DIGITAL_APP_PIN 5

MPU6050 mpu;

// MPU control/status vars
bool dmpReady = false;
uint8_t mpuIntStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];
Quaternion q;

volatile bool mpuInterrupt = false;

void dmpDataReady()
{
	mpuInterrupt = true;
}

void setup()
{
  pinMode(DIGITAL_APP_PIN, INPUT);
  pinMode(DIGITAL_HOME_PIN, INPUT);
  pinMode(ANALOG_X_PIN, INPUT);
  pinMode(ANALOG_Y_PIN, INPUT);
  pinMode(ANALOG_TRIGGER_PIN, INPUT);
  pinMode(ANALOG_GRIP_PIN, INPUT);
  
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
	Wire.begin();
	TWBR = 24; // 400kHz I2C clock (200kHz if CPU is 8MHz)
#elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
	Fastwire::setup(400, true);
#endif
	Serial.begin(115200);
	while (!Serial); // wait for Leonardo enumeration, others continue immediately

	mpu.initialize();
	uint8_t devStatus = mpu.dmpInitialize();

	if (devStatus == 0)
	{
		mpu.setDMPEnabled(true);
		attachInterrupt(0, dmpDataReady, RISING);
		mpuIntStatus = mpu.getIntStatus();
		packetSize = mpu.dmpGetFIFOPacketSize();
		dmpReady = true;
	}
}

void loop()
{
	if (!dmpReady)
		return;

	// wait for MPU interrupt or extra packet(s) available
	// All other behaviours come here.
	while (!mpuInterrupt && fifoCount < packetSize)
	{  
		Serial.print("b|");
		Serial.print(digitalRead(DIGITAL_APP_PIN));
		Serial.print("|");
		Serial.print(digitalRead(DIGITAL_HOME_PIN));
		Serial.println("");

		Serial.print("a|");
		Serial.print(analogRead(ANALOG_X_PIN));
		Serial.print("|");
		Serial.print(analogRead(ANALOG_Y_PIN));
		Serial.print("|");
		Serial.print(analogRead(ANALOG_TRIGGER_PIN));
		Serial.print("|");
		Serial.print(analogRead(ANALOG_GRIP_PIN));
		Serial.println("");

		fifoCount = mpu.getFIFOCount();
	}

	mpuInterrupt = false;
	mpuIntStatus = mpu.getIntStatus();
	fifoCount = mpu.getFIFOCount();

	if (mpuIntStatus & 0x02)
	{
		// Required to continue..
		Serial.println("");

		while (fifoCount < packetSize)
			fifoCount = mpu.getFIFOCount();

		mpu.getFIFOBytes(fifoBuffer, packetSize);
		fifoCount -= packetSize;

		mpu.dmpGetQuaternion(&q, fifoBuffer);
		Serial.print("q|");
		Serial.print(q.w);
		Serial.print("|");
		Serial.print(q.x);
		Serial.print("|");
		Serial.print(q.y);
		Serial.print("|");
		Serial.print(q.z);
		Serial.println("");
	}

	// check for overflow (this should never happen unless our code is too inefficient)
	else if ((mpuIntStatus & 0x10) || fifoCount == 1024)
		mpu.resetFIFO();
}

