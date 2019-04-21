#include "I2Cdev.h"
#include "MPU6050.h"
#include "Wire.h"

// class default I2C address is 0x68
// specific I2C addresses may be passed as a parameter here
// AD0 low = 0x68 (default for InvenSense evaluation board)
// AD0 high = 0x69
MPU6050 accelgyro;
//MPU6050 accelgyro(0x69); // <-- use for AD0 high

int16_t ax, ay, az;
int16_t gx, gy, gz;
bool blinkState = false;

#define LED_PIN 13

void setup()
{
	Wire.begin();

	Serial.begin(115200);
	accelgyro.initialize();
	pinMode(LED_PIN, OUTPUT);
}

void loop()
{
	// read raw accel/gyro measurements from device
	accelgyro.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

	Serial.println("b");
	Serial.print(ax); Serial.print("|");
	Serial.print(ay); Serial.print("|");
	Serial.print(az); Serial.print("|");
	Serial.print(gx); Serial.print("|");
	Serial.print(gy); Serial.print("|");
	Serial.println(gz);

	// blink LED to indicate activity
	blinkState = !blinkState;
	digitalWrite(LED_PIN, blinkState);
}
