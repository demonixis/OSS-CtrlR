#ifndef SERIALWIN_H
#define SERIALWIN_H

#define ARDUINO_WAIT_TIME 2000

#include <windows.h>
#include <iostream>
#include "SerialSource.h"

class SerialWin : public SerialSource
{
private:
	HANDLE m_serial;
	bool m_connected;
	COMSTAT m_status;
	DWORD m_errors;

public:
	SerialWin(const char *portName, int baudRate);
	~SerialWin();

	int ReadData(char *buffer, unsigned int nbChar);
	bool WriteData(const char *buffer, unsigned int nbChar);
	bool IsConnected() { return m_connected; }
};

#endif // SERIALWIN_H