#ifndef MPU_H
#define MPU_H
#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

class IMU
{
private:
	MPU6050 mpu;
	volatile bool mpuInterrupt = false;
	bool dmpReady = false;
	uint8_t mpuIntStatus;
	uint16_t packetSize;
	uint16_t fifoCount;
	uint8_t fifoBuffer[64];

private:
	void dmpDataReady()
	{
		mpuInterrupt = true;
	}

public:
  void Initialize()
  {
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

	void Update(Quaternion &q, int index)
	{
		if (!dmpReady)
			return;

		// wait for MPU interrupt or extra packet(s) available
		// All other behaviours come here.
		while (!mpuInterrupt && fifoCount < packetSize)
			fifoCount = mpu.getFIFOCount();

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
			Serial.print(index);
			Serial.print("|");
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
};
#endif // MPU_H
