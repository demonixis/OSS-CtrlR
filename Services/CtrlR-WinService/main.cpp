#include <iostream>
#include <string>
#include "SerialWin.h"

char* getCmdOption(char **begin, char **end, const std::string &option);

int main(int argc, char* argv[])
{
	std::cout << "OSS CtrlR - Windows Service" << std::endl;

	int baudRate = 115200;
	std::string portName = "\\\\.\\COM8";

	char *param = getCmdOption(argv, argv + argc, "-b");
	if (param)
		baudRate = atoi(param);

	param = getCmdOption(argv, argv + argc, "-p");
	if (param)
		portName = std::string("\\\\.\\") + std::string(param);

	SerialWin *serial = new SerialWin(portName.c_str(), baudRate);

	if (serial->IsConnected())
		std::cout << "Connected to the Arduino board" << std::endl;

	char incomingData[256] = "";
	int dataLength = 255;
	int readResult = 0;

	while (serial->IsConnected())
	{
		readResult = serial->ReadData(incomingData, dataLength);
		incomingData[readResult] = 0;
		std::cout << incomingData << std::endl;
		Sleep(10);
	}

	delete serial;

	return EXIT_SUCCESS;
}

char* getCmdOption(char **begin, char **end, const std::string &option)
{
	char ** itr = std::find(begin, end, option);
	if (itr != end && ++itr != end)
		return *itr;
	return 0;
}