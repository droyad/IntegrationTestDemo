#!/usr/bin/env bash
COLOR_START="\033[01;34m"
COLOR_END="\033[00m"
MSG_TIME="${MSG_TIME:-30}"

function show_msg() {
  echo -e "${COLOR_START}${@}${COLOR_END}"
}

show_msg "Adding dotnet repo"
sudo sh -c 'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main" > /etc/apt/sources.list.d/dotnetdev.list' || exit 1
sudo apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893 || exit 1

show_msg "Running 'apt-get install aptitude'..."
sudo apt-get install aptitude -y || exit 1

show_msg "Updating aptitude"
sudo aptitude update || exit 1

show_msg "Installing dotnet"
sudo aptitude install dotnet-dev-1.0.4 --assume-yes || exit 1

show_msg "dotnet installation complete"
