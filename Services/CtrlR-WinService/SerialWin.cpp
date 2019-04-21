#include "SerialWin.h"

SerialWin::SerialWin(const char *portName, int baudRate)
{
	m_connected = false;

	wchar_t* wString = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, portName, -1, wString, 4096);

	//Try to connect to the given port throuh CreateFile
	this->m_serial = CreateFile(wString,
		GENERIC_READ | GENERIC_WRITE,
		0,
		NULL,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		NULL);

	if (this->m_serial == INVALID_HANDLE_VALUE)
	{
		if (GetLastError() == ERROR_FILE_NOT_FOUND)
			std::cout << "ERROR: Handle was not attached. Reason:" << portName << " not available." << std::endl;
		else
			std::cout << "ERROR" << std::endl;
	}
	else
	{
		DCB dcbSerialParams = { 0 };

		//Try to get the current
		if (GetCommState(this->m_serial, &dcbSerialParams))
		{
			//Define serial connection parameters for the arduino board
			dcbSerialParams.BaudRate = baudRate;
			dcbSerialParams.ByteSize = 8;
			dcbSerialParams.StopBits = ONESTOPBIT;
			dcbSerialParams.Parity = NOPARITY;
			//Setting the DTR to Control_Enable ensures that the Arduino is properly
			//reset upon establishing a connection
			dcbSerialParams.fDtrControl = DTR_CONTROL_ENABLE;

			//Set the parameters and check for their proper application
			if (SetCommState(m_serial, &dcbSerialParams))
			{
				this->m_connected = true;
				//Flush any remaining characters in the buffers 
				PurgeComm(this->m_serial, PURGE_RXCLEAR | PURGE_TXCLEAR);
				//We wait 2s as the arduino board will be reseting
				Sleep(ARDUINO_WAIT_TIME);
			}
			else
				std::cout << "ALERT: Could not set Serial Port parameters";
		}
		else
			std::cout << "failed to get current serial parameters!";
	}
}

SerialWin::~SerialWin()
{
	if (m_connected)
	{
		m_connected = false;
		CloseHandle(m_serial);
	}
}

int SerialWin::ReadData(char *buffer, unsigned int nbChar)
{
	//Number of bytes we'll have read
	DWORD bytesRead;
	//Number of bytes we'll really ask to read
	unsigned int toRead;

	ClearCommError(m_serial, &m_errors, &m_status);

	//Check if there is something to read
	if (m_status.cbInQue > 0)
	{
		if (m_status.cbInQue > nbChar)
			toRead = nbChar;
		else
			toRead = m_status.cbInQue;

		if (ReadFile(m_serial, buffer, toRead, &bytesRead, NULL))
			return bytesRead;
	}

	return 0;
}


bool SerialWin::WriteData(const char *buffer, unsigned int nbChar)
{
	DWORD bytesSend;

	if (!WriteFile(m_serial, (void *)buffer, nbChar, &bytesSend, 0))
	{
		ClearCommError(m_serial, &m_errors, &m_status);
		return false;
	}
	else
		return true;
}