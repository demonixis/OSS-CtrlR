# OSS-CtrlR : Services

### CtrlR-SharpService
This service is written in C# and can be used on Windows, Linux and Mac.

#### Start parameters

| Parameter | Values | Description |
|-----------|--------|-------------|
| BaudRate  | **115200** | The baud rate (You don't have to change that) |
| PortName  | COM8   | The port where the Arduino is connected |
| Verbose   | True or **False** | Display or not the data sent by the service |

### CtrlR-WinService
This service is a C++ Windows Only service.

#### Start parameters

| Parameter | Values | Description |
|-----------|--------|-------------|
| -b  | **115200** | The baud rate (You don't have to change that) |
| -p  | COM8   | The port where the Arduino is connected |
| -v   | True or **False** | Display or not the data sent by the service |