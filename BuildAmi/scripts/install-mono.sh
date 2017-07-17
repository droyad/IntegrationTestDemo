#!/usr/bin/env bash
COLOR_START="\033[01;34m"
COLOR_END="\033[00m"
MSG_TIME="${MSG_TIME:-30}"

function show_msg() {
  echo -e "${COLOR_START}${@}${COLOR_END}"
}

show_msg "Adding mono repo"
sudo apt-key update || exit 1
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF || exit 1
echo "deb http://download.mono-project.com/repo/debian wheezy/snapshots 4.6.0.245/main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list || exit 1
sudo apt-get update || exit 1

show_msg "Installing mono 4.6.1"
sudo apt-get install mono-complete --yes --force-yes || exit 1

show_msg "Mono installation complete"