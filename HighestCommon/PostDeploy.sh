#!/bin/bash
sudo ufw allow 7000:7010/tcp
nohup dotnet HighestCommon.dll </dev/null &