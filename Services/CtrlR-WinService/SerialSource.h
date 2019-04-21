#ifndef SERIALSOURCE_H
#define SERIALSOURCE_H

class SerialSource
{
public:
	// Read data in a buffer, returns -1 when nothing could be read.
	virtual int ReadData(char *buffer, unsigned int nbChar) = 0;

	// Writes data from a buffer, returns true on success.
	virtual bool WriteData(const char *buffer, unsigned int nbChar) = 0;

	virtual bool IsConnected() = 0;
};

#endif // SERIALSOURCE_H