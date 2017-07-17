#!/bin/bash
sudo ufw allow 7000:7010/tcp
nohup /bin/bash dotnet HighestCommon.dll </dev/null >/dev/null &
exit