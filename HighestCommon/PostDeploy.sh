#!/bin/bash
sudo ufw allow 7000:7010/tcp
screen -d -m -S "HighestCommon" dotnet HighestCommon.dll
exit